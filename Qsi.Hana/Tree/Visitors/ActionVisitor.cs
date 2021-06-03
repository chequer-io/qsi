using System;
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
        public static QsiDataDeleteActionNode VisitDeleteStatement(DeleteStatementContext context)
        {
            throw new NotImplementedException();
        }

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

        public static QsiDataInsertActionNode VisitReplaceStatement(ReplaceStatementContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiDataInsertActionNode VisitSelectIntoStatement(SelectIntoStatementContext context)
        {
            var columnListClause = context.columnListClause();
            var hintClause = context.hintClause();

            var node = new HanaDataInsertActionNode();

            node.ValueTable.SetValue(TableVisitor.VisitSelectStatement(context.selectStatement()));

            if (!context.TryGetRuleContext<TableRefContext>(out var tableRef))
                throw TreeHelper.NotSupportedFeature("VARIABLE in SELECT INTO Statement");

            var tableNode = TableVisitor.VisitTableRef(tableRef);

            switch (tableNode)
            {
                case HanaTableReferenceNode tableRefNode:
                {
                    node.Target.SetValue(tableRefNode);
                    break;
                }

                // <table> AS <alias> <hint>?
                //         ^~Remove~^ 
                case HanaDerivedTableNode derivedTableNode:
                {
                    var tableRefNode = (HanaTableReferenceNode)derivedTableNode.Source.Value;

                    if (!derivedTableNode.Sampling.IsEmpty)
                        tableRefNode.Sampling.SetValue(derivedTableNode.Sampling.Value);

                    node.Target.SetValue(tableRefNode);
                    break;
                }

                default:
                    throw new InvalidOperationException();
            }

            if (columnListClause != null)
            {
                node.Columns = columnListClause.list
                    .Select(x => new QsiQualifiedIdentifier(x[^1]))
                    .ToArray();
            }

            if (hintClause != null)
                node.Hint.SetValue(TreeHelper.Fragment(hintClause.GetInputText()));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiDataUpdateActionNode VisitUpdateStatement(UpdateStatementContext context)
        {
            throw new NotImplementedException();
        }
    }
}
