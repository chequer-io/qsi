using System.Linq;
using Qsi.Data;
using Qsi.MySql.Tree.Common;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class ActionVisitor
    {
        public static QsiActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            var derivedNode = new QsiDerivedTableNode();

            var withClause = context.withClause();
            var whereClause = context.whereClause();
            var tableAliasRefList = context.tableAliasRefList();

            if (withClause != null)
                derivedNode.Directives.SetValue(TableVisitor.VisitWithClause(withClause));

            if (whereClause != null)
                derivedNode.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));

            if (tableAliasRefList != null)
            {
                var tableReferenceList = context.tableReferenceList();

                (QsiQualifiedIdentifier Identifier, bool Wildcard)[] aliases = tableAliasRefList.tableRefWithWildcard()
                    .Select(IdentifierVisitor.VisitTableRefWithWildcard)
                    .Select(i =>
                    {
                        if (i[^1] == QsiIdentifier.Wildcard)
                            return (i.SubIdentifier(..^1), true);

                        return (i, false);
                    })
                    .ToArray();

                var columns = new QsiColumnsDeclarationNode();
                var isUsing = context.HasToken(USING_SYMBOL);

                foreach (var (identifier, wildcard) in aliases)
                {
                    if (isUsing || wildcard)
                    {
                        columns.Columns.Add(new QsiAllColumnNode
                        {
                            Path = identifier
                        });
                    }
                    else
                    {
                        columns.Columns.Add(new QsiDeclaredColumnNode
                        {
                            Name = identifier
                        });
                    }
                }

                derivedNode.Columns.SetValue(columns);
                derivedNode.Source.SetValue(TableVisitor.VisitTableReferenceList(tableReferenceList));
            }
            else
            {
                var tableRef = context.tableRef();
                var partitionDelete = context.partitionDelete(); // TODO: implement
                var orderClause = context.orderClause();
                var simpleLimitClause = context.simpleLimitClause();

                derivedNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedNode.Source.SetValue(TableVisitor.VisitTableRef(tableRef));

                if (orderClause != null)
                    derivedNode.Order.SetValue(ExpressionVisitor.VisitOrderClause(orderClause));

                if (simpleLimitClause != null)
                    derivedNode.Limit.SetValue(ExpressionVisitor.VisitSimpleLimitClause(simpleLimitClause));
            }

            var node = new QsiDataDeleteActionNode();
            node.Target.SetValue(derivedNode);

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiActionNode VisitReplaceStatement(ReplaceStatementContext context)
        {
            return VisitCommonInsert(new CommonInsertContext(context));
        }

        public static QsiActionNode VisitInsertStatement(InsertStatementContext context)
        {
            return VisitCommonInsert(new CommonInsertContext(context));
        }

        public static QsiActionNode VisitCommonInsert(CommonInsertContext context)
        {
            var node = new QsiDataInsertActionNode
            {
                ConflictBehavior = context.ConflictBehavior
            };

            node.Target.SetValue(TableVisitor.VisitTableRef(context.TableRef));
            // TODO: implement context.UsePartition

            if (context.InsertFromConstructor != null)
            {
                node.Columns = context.InsertFromConstructor
                    .fields()?
                    .insertIdentifier()?
                    .Select(IdentifierVisitor.VisitInsertIdentifier)
                    .ToArray();

                node.Values.AddRange(ExpressionVisitor.VisitInsertValues(context.InsertFromConstructor.insertValues()));

                // TODO: implement context.ValuesReference
            }
            else if (context.UpdateList != null)
            {
                node.SetValues.AddRange(ExpressionVisitor.VisitUpdateList(context.UpdateList));

                // TODO: implement context.ValuesReference
            }
            else
            {
                node.Columns = context.InsertQueryExpression
                    .fields()?
                    .insertIdentifier()?
                    .Select(IdentifierVisitor.VisitInsertIdentifier)
                    .ToArray();

                node.ValueTable.SetValue(
                    TableVisitor.VisitQueryExpressionOrParens(context.InsertQueryExpression.queryExpressionOrParens()));
            }

            if (context.InsertUpdateList != null)
                node.ConflictAction.SetValue(VisitInsertUpdateList(context.InsertUpdateList));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiDataConflictActionNode VisitInsertUpdateList(InsertUpdateListContext context)
        {
            var node = new QsiDataConflictActionNode();

            node.SetValues.AddRange(ExpressionVisitor.VisitUpdateList(context.updateList()));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            var tableNode = TableVisitor.VisitTableReferenceList(context.tableReferenceList());
            var withClause = context.withClause();
            var whereClause = context.whereClause();
            var orderClause = context.orderClause();
            var simpleLimitClause = context.simpleLimitClause();

            if (withClause != null || whereClause != null || orderClause != null || simpleLimitClause != null)
            {
                if (tableNode is not QsiDerivedTableNode derivedTableNode)
                {
                    derivedTableNode = new QsiDerivedTableNode();
                    derivedTableNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                    derivedTableNode.Source.SetValue(tableNode);
                }

                if (withClause != null)
                    derivedTableNode.Directives.SetValue(TableVisitor.VisitWithClause(withClause));

                if (whereClause != null)
                    derivedTableNode.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));

                if (orderClause != null)
                    derivedTableNode.Order.SetValue(ExpressionVisitor.VisitOrderClause(orderClause));

                if (simpleLimitClause != null)
                    derivedTableNode.Limit.SetValue(ExpressionVisitor.VisitSimpleLimitClause(simpleLimitClause));

                tableNode = derivedTableNode;
            }

            var node = new QsiDataUpdateActionNode();

            node.Target.SetValue(tableNode);
            node.SetValues.AddRange(ExpressionVisitor.VisitUpdateList(context.updateList()));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
    }
}
