using System.Collections.Generic;
using System.Linq;
using Qsi.Athena.Internal;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Tree.Definition;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ActionVisitor
    {
        public static QsiViewDefinitionNode VisitCreateView(CreateViewContext context)
        {
            var node = AthenaTree.CreateWithSpan<QsiViewDefinitionNode>(context);

            if (context.HasToken(REPLACE))
                node.ConflictBehavior = QsiDefinitionConflictBehavior.Replace;

            node.Identifier = context.qualifiedName().qqi;
            node.Source.Value = TableVisitor.VisitQuery(context.query());

            return node;
        }

        public static QsiDataInsertActionNode VisitInsertInto(InsertIntoContext context)
        {
            var name = context.qualifiedName();
            var columnAliases = context.columnAliases();
            var query = context.query();

            var node = AthenaTree.CreateWithSpan<QsiDataInsertActionNode>(context);

            var targetNode = TableVisitor.VisitQualifiedName(name);
            node.Target.Value = targetNode;

            if (columnAliases is not null)
            {
                IEnumerable<QsiQualifiedIdentifier> columnIdentifiers = columnAliases
                    .identifier()
                    .Select(i => new QsiQualifiedIdentifier(i.qi));

                node.Columns.AddRange(columnIdentifiers);
            }

            node.ValueTable.Value = TableVisitor.VisitQuery(query);

            return node;
        }

        public static QsiDataDeleteActionNode VisitDelete(DeleteContext context)
        {
            var tableNode = AthenaTree.CreateWithSpan<QsiDerivedTableNode>(context);

            tableNode.Source.Value = TableVisitor.VisitQualifiedName(context.qualifiedName());
            tableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            if (context.HasToken(WHERE))
            {
                tableNode.Where.Value = ExpressionVisitor.VisitWhere(
                    context.booleanExpression(),
                    context.WHERE()
                );
            }

            var node = AthenaTree.CreateWithSpan<QsiDataDeleteActionNode>(context);
            node.Target.Value = tableNode;

            return node;
        }

        public static QsiChangeSearchPathActionNode VisitUse(UseContext context)
        {
            var path = context.catalog is not null
                ? new QsiQualifiedIdentifier(context.catalog.qi, context.schema.qi)
                : new QsiQualifiedIdentifier(context.schema.qi);

            var node = AthenaTree.CreateWithSpan<QsiChangeSearchPathActionNode>(context);
            node.Identifiers = new[] { path };

            return node;
        }
    }
}
