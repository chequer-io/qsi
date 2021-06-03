using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Hana.Internal.HanaParserInternal;

namespace Qsi.Hana.Tree.Visitors
{
    internal static class ActionVisitor
    {
        public static QsiDataInsertActionNode VisitInsertStatement(InsertStatementContext context)
        {
            var partitionRestriction = context.partitionRestriction();
            var columnListClause = context.columnListClause();
            var hintClause = context.hintClause();
            var valueListClause = context.valueListClause();

            var tableNode = new HanaTableReferenceNode
            {
                Identifier = context.tableName().qqi
            };

            if (partitionRestriction != null)
                tableNode.Partition.SetValue(TreeHelper.Fragment(partitionRestriction.GetInputText()));

            var node = new HanaDataInsertActionNode
            {
                Target = { Value = tableNode }
            };

            if (columnListClause != null)
            {
                node.Columns = columnListClause.list
                    .Select(x => new QsiQualifiedIdentifier(x[^1]))
                    .ToArray();
            }

            if (valueListClause != null)
            {
                IList<ExpressionContext> values = valueListClause.expressionList()._list;

                var rowValue = new QsiRowValueExpressionNode();
                rowValue.ColumnValues.AddRange(values.Select(ExpressionVisitor.VisitExpression));
                HanaTree.PutContextSpan(rowValue, valueListClause);

                node.Values.Add(rowValue);
            }
            else
            {
                var overridingClause = context.overridingClause();

                if (overridingClause != null)
                    node.Overriding.SetValue(TreeHelper.Fragment(overridingClause.GetInputText()));

                node.ValueTable.SetValue(TableVisitor.VisitSelectStatement(context.selectStatement()));
            }

            if (hintClause != null)
                node.Hint.SetValue(TreeHelper.Fragment(hintClause.GetInputText()));

            HanaTree.PutContextSpan(node, context);

            return node;
        }
    }
}
