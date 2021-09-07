using System.Linq;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Trino.Internal;
using Qsi.Utilities;

namespace Qsi.Trino.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ActionVisitor
    {
        public static QsiDataInsertActionNode VisitInsertInto(InsertIntoContext context)
        {
            var name = context.qualifiedName();
            var columnAliases = context.columnAliases();
            var query = context.query();

            var node = TrinoTree.CreateWithSpan<QsiDataInsertActionNode>(context);

            var targetNode = TableVisitor.VisitQualifiedName(name);
            node.Target.Value = targetNode;

            if (columnAliases is not null)
                node.Columns.AddRange(columnAliases.identifier().Select(identifierContext => new QsiQualifiedIdentifier(identifierContext.qi)));

            node.ValueTable.Value = TableVisitor.VisitQuery(query);

            return node;
        }

        public static QsiDataUpdateActionNode VisitUpdate(UpdateContext context)
        {
            var name = context.qualifiedName();
            UpdateAssignmentContext[] updateAssignment = context.updateAssignment();
            var where = context.where;

            var node = TrinoTree.CreateWithSpan<QsiDataUpdateActionNode>(context);

            var tableNode = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);
            tableNode.Source.Value = TableVisitor.VisitQualifiedName(name);
            tableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            if (where is not null)
                tableNode.Where.Value = ExpressionVisitor.VisitWhere(where, context.WHERE().Symbol);

            node.SetValues.AddRange(updateAssignment.Select(ExpressionVisitor.VisitUpdateAssignment));

            node.Target.Value = tableNode;

            return node;
        }

        public static QsiDataDeleteActionNode VisitDelete(DeleteContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiDataDeleteActionNode>(context);

            var tableNode = TrinoTree.CreateWithSpan<QsiDerivedTableNode>(context);
            tableNode.Source.Value = TableVisitor.VisitQualifiedName(context.qualifiedName());
            tableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            if (context.HasToken(WHERE))
                tableNode.Where.Value = ExpressionVisitor.VisitWhere(context.booleanExpression(), context.WHERE().Symbol);

            node.Target.Value = tableNode;

            return node;
        }
    }
}
