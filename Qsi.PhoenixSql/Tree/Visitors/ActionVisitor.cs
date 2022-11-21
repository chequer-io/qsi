using System.Linq;
using PhoenixSql;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PhoenixSql.Tree
{
    internal static class ActionVisitor
    {
        public static QsiActionNode Visit(IDMLStatement statement)
        {
            switch (statement)
            {
                case UpsertStatement upsertStatement:
                    return VisitUpsertStatement(upsertStatement);

                case DeleteStatement deleteStatement:
                    return VisitDeleteStatement(deleteStatement);

                default:
                    throw TreeHelper.NotSupportedTree(statement);
            }
        }

        public static QsiActionNode VisitUpsertStatement(UpsertStatement node)
        {
            var insertAction = new PUpsertActionNode
            {
                Hints = node.Hint?.Hints
            };

            if (TableVisitor.VisitNamedTableNode(node.Table) is QsiTableReferenceNode tableReferenceNode)
            {
                insertAction.Target.SetValue(tableReferenceNode);
            }
            else
            {
                throw new QsiException(QsiError.Syntax);
            }

            if (node.Columns.Any())
            {
                insertAction.Columns = node.Columns
                    .Select(IdentifierVisitor.Visit)
                    .ToArray();
            }

            if (node.Values.Any())
            {
                var row = new QsiRowValueExpressionNode();
                row.ColumnValues.AddRange(node.Values.Select(ExpressionVisitor.Visit));

                insertAction.Values.Add(row);
            }
            else if (node.Select != null)
            {
                insertAction.ValueTable.SetValue(TableVisitor.VisitSelectStatement(node.Select));
            }

            if (node.OnDupKeyIgnore)
            {
                insertAction.ConflictBehavior = QsiDataConflictBehavior.Ignore;
            }
            else if (node.OnDupKeyPairs.Any())
            {
                var conflictAction = new QsiDataConflictActionNode();
                conflictAction.SetValues.AddRange(node.OnDupKeyPairs.Select(VisitDupKeyPair));

                insertAction.ConflictBehavior = QsiDataConflictBehavior.Update;
                insertAction.ConflictAction.SetValue(conflictAction);
            }

            PTree.RawNode[insertAction] = node;

            return insertAction;
        }

        private static QsiSetColumnExpressionNode VisitDupKeyPair(UpsertStatement.Types.Pair_ColumnName_ParseNode pair)
        {
            return TreeHelper.Create<QsiSetColumnExpressionNode>(n =>
            {
                n.Target = IdentifierVisitor.Visit(pair.First);
                n.Value.SetValue(ExpressionVisitor.Visit(pair.Second));
            });
        }

        public static QsiActionNode VisitDeleteStatement(DeleteStatement node)
        {
            var table = TableVisitor.VisitTableNode(node.Table);

            if (node.Where != null || node.OrderBy.Any() || node.Limit != null)
            {
                var derivedTable = new QsiDerivedTableNode();

                derivedTable.Columns.SetValue(TreeHelper.CreateAllVisibleColumnsDeclaration());
                derivedTable.Source.SetValue(table);

                if (node.Where != null)
                    derivedTable.Where.SetValue(ExpressionVisitor.VisitWhere(node.Where));

                if (node.OrderBy.Any())
                    derivedTable.Order.SetValue(ExpressionVisitor.VisitOrderBy(node.OrderBy));

                if (node.Limit != null)
                    derivedTable.Limit.SetValue(ExpressionVisitor.VisitLimitOffset(node.Limit, null));

                table = derivedTable;
            }

            var deleteAction = new PDeleteActionNode
            {
                Hints = node.Hint?.Hints
            };

            deleteAction.Target.SetValue(table);

            PTree.RawNode[deleteAction] = node;

            return deleteAction;
        }

        public static QsiChangeSearchPathActionNode VisitUseSchemaStatement(UseSchemaStatement context)
        {
            var schemaName = context.SchemaName;

            if (string.IsNullOrEmpty(schemaName))
                schemaName = "DEFAULT";

            var identifier = new QsiIdentifier(schemaName, IdentifierUtility.IsEscaped(schemaName));

            var node = new QsiChangeSearchPathActionNode
            {
                Identifiers = new[]
                {
                    new QsiQualifiedIdentifier(identifier)
                }
            };

            PTree.RawNode[node] = context;

            return node;
        }
    }
}
