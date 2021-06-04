using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Extensions;
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
            var partitionRestriction = context.partitionRestriction();
            var whereClause = context.whereClause();
            var hintClause = context.hintClause();

            QsiTableNode tableNode = new HanaTableReferenceNode
            {
                Identifier = context.tableName().qqi
            };

            if (partitionRestriction != null)
                ((HanaTableReferenceNode)tableNode).Partition.Value = TreeHelper.Fragment(partitionRestriction.GetInputText());

            if (whereClause != null || hintClause != null)
            {
                var derivedTableNode = new HanaDerivedTableNode
                {
                    Columns = { Value = TreeHelper.CreateAllColumnsDeclaration() },
                    Source = { Value = tableNode }
                };

                if (whereClause != null)
                    derivedTableNode.Where.Value = ExpressionVisitor.VisitWhereClause(whereClause);

                if (hintClause != null)
                    derivedTableNode.Hint.Value = TreeHelper.Fragment(hintClause.GetInputText());

                tableNode = derivedTableNode;
            }

            var node = new QsiDataDeleteActionNode
            {
                Target = { Value = tableNode }
            };

            HanaTree.PutContextSpan(node, context);

            return node;
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
                node.Values.Add(VisitValueListClause(valueListClause));
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

        public static QsiActionNode VisitReplaceStatement(ReplaceStatementContext context)
        {
            var partitionRestriction = context.partitionRestriction();
            var columnListClause = context.columnListClause();
            var valueListClause = context.valueListClause();
            var whereClause = valueListClause != null ? context.whereClause() : null;

            var tableNode = new HanaTableReferenceNode
            {
                Identifier = context.tableName().qqi
            };

            if (partitionRestriction != null)
                tableNode.Partition.SetValue(TreeHelper.Fragment(partitionRestriction.GetInputText()));

            if (whereClause != null)
            {
                // UPDATE

                var derivedTableNode = new HanaDerivedTableNode
                {
                    Source = { Value = tableNode },
                    Where = { Value = ExpressionVisitor.VisitWhereClause(whereClause) }
                };

                derivedTableNode.Columns.SetValue(
                    columnListClause != null ?
                        TableVisitor.VisitColumnListClause(columnListClause, null) :
                        TreeHelper.CreateAllColumnsDeclaration()
                );

                var node = new HanaDataUpdateActionNode
                {
                    Target = { Value = derivedTableNode }
                };

                node.Value.SetValue(VisitValueListClause(valueListClause));

                HanaTree.PutContextSpan(node, context);

                return node;
            }
            else
            {
                // INSERT

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
                    node.Values.Add(VisitValueListClause(valueListClause));
                }
                else
                {
                    node.ValueTable.SetValue(TableVisitor.VisitSelectStatement(context.selectStatement()));
                }

                HanaTree.PutContextSpan(node, context);

                return node;
            }
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
            var topClause = context.topClause();
            var alias = context.alias();
            var partitionRestriction = context.partitionRestriction();
            var fromClause = context.fromClause();
            var whereClause = context.whereClause();
            var hintClause = context.hintClause();

            QsiTableNode tableNode;
            QsiColumnsDeclarationNode columns = null;

            if (fromClause != null)
            {
                tableNode = TableVisitor.VisitFromClause(fromClause);

                columns = new QsiColumnsDeclarationNode
                {
                    Columns =
                    {
                        new QsiAllColumnNode
                        {
                            Path = alias != null ?
                                new QsiQualifiedIdentifier(alias.node.Name) :
                                context.tableName().qqi
                        }
                    }
                };

                if (tableNode is HanaDerivedTableNode derivedTableNode &&
                    (derivedTableNode.Columns.IsEmpty || derivedTableNode.Columns.Value.IsAllColumnNode()))
                {
                    derivedTableNode.Columns.SetValue(columns);
                    columns = null;
                }
            }
            else
            {
                tableNode = new HanaTableReferenceNode
                {
                    Identifier = context.tableName().qqi
                };

                if (partitionRestriction != null)
                    ((HanaTableReferenceNode)tableNode).Partition.SetValue(TreeHelper.Fragment(partitionRestriction.GetInputText()));

                if (alias != null)
                    tableNode = TreeHelper.CreateAliasedTableNode(tableNode, alias.node);
            }

            if (topClause != null || whereClause != null || hintClause != null || columns != null)
            {
                var derivedTableNode = new HanaDerivedTableNode
                {
                    Columns = { Value = columns ?? TreeHelper.CreateAllColumnsDeclaration() },
                    Source = { Value = tableNode }
                };

                if (topClause != null)
                    derivedTableNode.Top = TableVisitor.VisitTopClause(topClause);

                if (whereClause != null)
                    derivedTableNode.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));

                if (hintClause != null)
                    derivedTableNode.Hint.SetValue(TreeHelper.Fragment(hintClause.GetInputText()));

                tableNode = derivedTableNode;
            }

            var node = new HanaDataUpdateActionNode
            {
                Target = { Value = tableNode }
            };

            node.SetValues.AddRange(context.setClause()._elements.Select(ExpressionVisitor.VisitSetElement));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiRowValueExpressionNode VisitValueListClause(ValueListClauseContext context)
        {
            var node = new QsiRowValueExpressionNode();
            node.ColumnValues.AddRange(context.expressionList()._list.Select(ExpressionVisitor.VisitExpression));
            HanaTree.PutContextSpan(node, context);
            return node;
        }

        public static QsiChangeSearchPathActionNode VisitSetSchemaStatement(SetSchemaStatementContext context)
        {
            var node = new QsiChangeSearchPathActionNode
            {
                Identifiers = new[]
                {
                    context.identifier().qi
                }
            };

            HanaTree.PutContextSpan(node, context);

            return node;
        }
    }
}
