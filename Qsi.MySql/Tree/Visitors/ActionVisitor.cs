using System.Linq;
using Qsi.Data;
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
                // ** Single table delete.
                // DELETE FROM table

                var tableRef = context.tableRef();
                var partitionDelete = context.partitionDelete();
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

            return node;
        }

        public static QsiActionNode VisitReplaceStatement(ReplaceStatementContext context)
        {
            throw new System.NotImplementedException();
        }

        public static QsiActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
