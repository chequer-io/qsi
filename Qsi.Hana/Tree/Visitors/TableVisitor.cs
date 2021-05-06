using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Shared.Utilities;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Hana.Internal.HanaParserInternal;

namespace Qsi.Hana.Tree.Visitors
{
    internal static class TableVisitor
    {
        public static HanaDerivedTableNode VisitSelectStatement(SelectStatementContext context)
        {
            var withClause = context.withClause();
            var subquery = context.subquery();
            var forClause = context.forClause();
            var timeTravel = context.timeTravel();
            var hintClause = context.hintClause();

            var subqueryNode = VisitSubquery(subquery);

            if (withClause is not null)
                subqueryNode.Directives.SetValue(VisitWithClause(withClause));

            if (forClause is not null)
                subqueryNode.Behavior.SetValue(VisitForClause(forClause));

            if (timeTravel is not null)
                subqueryNode.TimeTravel = timeTravel.GetInputText();

            if (hintClause is not null)
                subqueryNode.Hint = hintClause.GetInputText();

            return subqueryNode;
        }

        private static QsiTableDirectivesNode VisitWithClause(WithClauseContext context)
        {
            var node = new QsiTableDirectivesNode();

            node.Tables.AddRange(context._elements.Select(VisitWithListElement));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiDerivedTableNode VisitWithListElement(WithListElementContext context)
        {
            var node = new QsiDerivedTableNode();

            node.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(context.name)
            });

            var columnListClause = context.columnListClause();

            node.Columns.SetValue(
                columnListClause != null ?
                    VisitColumnListClause(columnListClause) :
                    TreeHelper.CreateAllColumnsDeclaration()
            );

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnsDeclarationNode VisitColumnListClause(ColumnListClauseContext context)
        {
            var node = VisitColumnList(context.list);
            HanaTree.PutContextSpan(node, context);
            return node;
        }

        private static QsiColumnsDeclarationNode VisitColumnList(ColumnListContext context)
        {
            var node = new QsiColumnsDeclarationNode();

            node.Columns.AddRange(context._columns.Select(VisitColumnName));
            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnNode VisitColumnName(ColumnNameContext context)
        {
            var node = new QsiDeclaredColumnNode
            {
                Name = new QsiQualifiedIdentifier(IdentifierVisitor.VisitColumnName(context))
            };

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnNode VisitFieldName(FieldNameContext context)
        {
            var node = new QsiDeclaredColumnNode
            {
                Name = new QsiQualifiedIdentifier(IdentifierVisitor.VisitFieldName(context))
            };

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static HanaDerivedTableNode VisitSubquery(SubqueryContext context)
        {
            if (context.inner != null)
                return VisitSelectStatement(context.inner);

            var node = new HanaDerivedTableNode();

            node.Columns.SetValue(VisitSelectClause(node, context.select));
            node.Source.SetValue(VisitFromClause(context.from));

            if (context.where != null)
                node.Where.SetValue(VisitWhereClause(context.where));

            if (context.groupBy != null)
                node.Grouping.SetValue(VisitGroupByClause(context.groupBy));

            // TODO: set

            if (context.orderBy != null)
                node.Order.SetValue(VisitOrderByClause(context.orderBy));

            if (context.limit != null)
                node.Limit.SetValue(VisitLimitClause(context.limit));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnsDeclarationNode VisitSelectClause(HanaDerivedTableNode tableNode, SelectClauseContext context)
        {
            var offset = 1;
            var topClause = context.topClause();

            if (topClause != null)
            {
                tableNode.Top = VisitTopClause(topClause);
                offset++;
            }

            switch (context.children[offset])
            {
                case ITerminalNode { Symbol: { Type: K_ALL } }:
                    tableNode.Operation = HanaResultSetOperation.All;
                    break;

                case ITerminalNode { Symbol: { Type: K_DISTINCT } }:
                    tableNode.Operation = HanaResultSetOperation.Distinct;
                    break;
            }

            return VisitSelectList(context.selectList());
        }

        private static QsiColumnsDeclarationNode VisitSelectList(SelectListContext context)
        {
            var node = new QsiColumnsDeclarationNode();

            node.Columns.AddRange(context._items.Select(VisitSelectItem));
            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnNode VisitSelectItem(SelectItemContext context)
        {
            switch (context)
            {
                case ExprItemContext exprItem:
                    return VisitExprItem(exprItem);

                case AssociationExprItemContext associationExprItem:
                    return VisitAssociationExprItem(associationExprItem);

                case WildcardItemContext wildcardItem:
                    return VisitWildcardItem(wildcardItem);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiColumnNode VisitExprItem(ExprItemContext context)
        {
            var expressionNode = ExpressionVisitor.VisitExpression(context.expression());
            var aliasNode = context.alias() != null ? VisitAlias(context.alias()) : null;
            var node = CreateSelectItem(context.expression(), expressionNode, aliasNode);

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnNode VisitAssociationExprItem(AssociationExprItemContext context)
        {
            var expressionNode = ExpressionVisitor.VisitAssociationExpression(context.associationExpression());
            var aliasNode = context.alias() != null ? VisitAlias(context.alias()) : null;
            var node = CreateSelectItem(context.associationExpression(), expressionNode, aliasNode);

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnNode CreateSelectItem(ParserRuleContext context, QsiExpressionNode expressionNode, QsiAliasNode aliasNode)
        {
            if (expressionNode is QsiColumnExpressionNode columnExpression)
            {
                if (aliasNode == null)
                    return columnExpression.Column.Value;

                var node = new QsiDerivedColumnNode();

                node.Alias.SetValue(aliasNode);
                node.Column.SetValue(columnExpression.Column.Value);

                return node;
            }
            else
            {
                var node = new QsiDerivedColumnNode();

                if (aliasNode != null)
                    node.Alias.SetValue(aliasNode);
                else
                    node.InferredName = GetInferredName(context);

                node.Expression.SetValue(expressionNode);

                return node;
            }
        }

        private static QsiColumnNode VisitWildcardItem(WildcardItemContext context)
        {
            var node = new QsiAllColumnNode();
            var tableName = context.tableName();

            if (tableName != null)
                node.Path = IdentifierVisitor.VisitTableName(tableName);

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static ulong VisitTopClause(TopClauseContext context)
        {
            return ulong.Parse(context.top.Text);
        }

        private static QsiTableNode VisitFromClause(FromClauseContext context)
        {
            QsiTableNode[] sources = context._tables
                .Select(VisitTableExpression)
                .ToArray();

            if (sources.Length == 1)
                return sources[0];

            var anchor = sources[0];

            for (int i = 1; i < sources.Length; i++)
            {
                var join = new QsiJoinedTableNode
                {
                    IsComma = true
                };

                join.Left.SetValue(anchor);
                join.Right.SetValue(sources[i]);

                var leftSpan = HanaTree.Span[join.Left.Value];
                var rightSpan = HanaTree.Span[join.Right.Value];

                HanaTree.Span[join] = new Range(leftSpan.Start, rightSpan.End);

                anchor = join;
            }

            return anchor;
        }

        #region TableExpression
        private static QsiTableNode VisitTableExpression(TableExpressionContext context)
        {
            var child = context.children[0];

            switch (child)
            {
                case TableRefContext tableRef:
                    return VisitTableRef(tableRef);

                case SubqueryTableExpressionContext subqueryTableExpression:
                    return VisitSubqueryTableExpression(subqueryTableExpression);

                case CaseJoinContext caseJoin:
                    return VisitCaseJoin(caseJoin);

                case LateralTableExpressionContext lateralTableExpression:
                    return VisitLateralTableExpression(lateralTableExpression);

                case CollectionDerivedTableContext collectionDerivedTable:
                    return VisitCollectionDerivedTable(collectionDerivedTable);

                case TableFunctionExpressionContext tableFunctionExpression:
                    return VisitTableFunctionExpression(tableFunctionExpression);

                case VariableTableContext variableTable:
                    return VisitVariableTable(variableTable);

                case AssociationTableExpressionContext associationTableExpression:
                    return VisitAssociationTableExpression(associationTableExpression);

                case TableExpressionContext:
                {
                    var node = new QsiJoinedTableNode
                    {
                        JoinType = StringUtility.JoinNotNullOrEmpty(
                            " ",
                            context.joinType()?.GetText(),
                            context.joinCardinality()?.GetText(),
                            "JOIN"
                        )
                    };

                    node.Left.SetValue(VisitTableExpression(context.left));
                    node.Right.SetValue(VisitTableExpression(context.right));

                    HanaTree.PutContextSpan(node, context);

                    return node;
                }

                default:
                    throw TreeHelper.NotSupportedTree(child);
            }
        }

        private static QsiTableNode VisitTableRef(TableRefContext context)
        {
            QsiTableNode node = new HanaTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitTableName(context.tableName()),
                ForSystemTime = context.forSystemTime()?.GetInputText(),
                ForApplicationTime = context.forApplicationTimePeriod()?.GetInputText(),
                Partition = context.partitionRestriction()?.GetInputText()
            };

            var alias = context.alias();
            var sampling = context.tableSampleClause();

            if (alias != null || sampling != null)
            {
                HanaTree.PutContextSpan(node, context.Start, alias?.Start ?? sampling?.Start);

                var derivedNode = new HanaDerivedTableNode();
                derivedNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
                derivedNode.Source.SetValue(node);

                if (alias != null)
                    derivedNode.Alias.SetValue(VisitAlias(alias));

                if (sampling != null)
                    derivedNode.Sampling = sampling.GetInputText();

                node = derivedNode;
            }

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiAliasNode VisitAlias(AliasContext context)
        {
            var node = new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(context.name)
            };

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiTableNode VisitSubqueryTableExpression(SubqueryTableExpressionContext context)
        {
            var node = VisitSubquery(context.subquery());
            var alias = context.alias() != null ? VisitAlias(context.alias()) : null;

            if (alias != null)
                node.Alias.SetValue(alias);

            return node;
        }

        private static HanaCaseJoinTableNode VisitCaseJoin(CaseJoinContext context)
        {
            var node = new HanaCaseJoinTableNode();
            var elseClause = context.caseJoinElseClause();
            var alias = context.alias();

            node.Source.SetValue(VisitTableRef(context.tableRef()));
            node.WhenSources.AddRange(context.caseJoinWhenClause().Select(VisitCaseJoinWhenClause));

            if (elseClause != null)
                node.ElseSource.SetValue(VisitCaseJoinElseClause(elseClause));

            if (alias != null)
                node.Alias.SetValue(VisitAlias(alias));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static HanaCaseJoinWhenTableNode VisitCaseJoinWhenClause(CaseJoinWhenClauseContext context)
        {
            var node = new HanaCaseJoinWhenTableNode();

            node.Condition.SetValue(ExpressionVisitor.VisitCondition(context.condition()));
            node.Columns.SetValue(VisitColumnListClause(context.columnListClause()));
            node.Source.SetValue(VisitTableRef(context.tableRef()));
            node.Predicate.SetValue(ExpressionVisitor.VisitPredicate(context.predicate()));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static HanaCaseJoinElseTableNode VisitCaseJoinElseClause(CaseJoinElseClauseContext context)
        {
            var node = new HanaCaseJoinElseTableNode();

            node.Columns.SetValue(VisitColumnListClause(context.columnListClause()));
            node.Source.SetValue(VisitTableRef(context.tableRef()));
            node.Predicate.SetValue(ExpressionVisitor.VisitPredicate(context.predicate()));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiTableNode VisitLateralTableExpression(LateralTableExpressionContext context)
        {
            var subquery = context.subquery();
            var functionExpression = context.functionExpression();

            if (functionExpression != null)
                throw TreeHelper.NotSupportedFeature("Table function");

            var node = new HanaLateralTableNode();

            node.Source.SetValue(VisitSubquery(subquery));

            if (context.alias() != null)
                node.Alias.SetValue(VisitAlias(context.alias()));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiTableNode VisitCollectionDerivedTable(CollectionDerivedTableContext context)
        {
            throw new NotImplementedException();
        }

        private static QsiTableNode VisitTableFunctionExpression(TableFunctionExpressionContext context)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        private static QsiTableNode VisitVariableTable(VariableTableContext context)
        {
            throw TreeHelper.NotSupportedFeature("Table variable");
        }

        private static QsiTableNode VisitAssociationTableExpression(AssociationTableExpressionContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

        private static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
        {
            var node = new QsiWhereExpressionNode();

            node.Expression.SetValue(ExpressionVisitor.VisitCondition(context.condition()));
            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiGroupingExpressionNode VisitGroupByClause(GroupByClauseContext context)
        {
            var node = new QsiGroupingExpressionNode();
            var groupByExpressionList = context.groupByExpressionList();

            foreach (var child in groupByExpressionList.children.OfType<ParserRuleContext>())
            {
                switch (child)
                {
                    case TableExpressionContext:
                    case GroupingSetContext:
                    {
                        var expressionNode = new QsiLiteralExpressionNode
                        {
                            Type = QsiDataType.Raw,
                            Value = child.GetInputText()
                        };

                        HanaTree.PutContextSpan(expressionNode, child);
                        node.Items.Add(expressionNode);
                        break;
                    }
                }
            }

            if (context.having != null)
                node.Having.SetValue(ExpressionVisitor.VisitCondition(context.having));

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiMultipleOrderExpressionNode VisitOrderByClause(TableOrderByClauseContext context)
        {
            var node = new QsiMultipleOrderExpressionNode();

            node.Orders.AddRange(context._orders.Select(VisitTableOrderByExpression));
            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static HanaOrderByExpressionNode VisitTableOrderByExpression(TableOrderByExpressionContext context)
        {
            var node = new HanaOrderByExpressionNode();
            var collateClause = context.collateClause();

            if (context.field != null)
            {
                var expressioNode = new QsiColumnExpressionNode();

                expressioNode.Column.SetValue(VisitFieldName(context.field));
                HanaTree.PutContextSpan(expressioNode, context.field);

                node.Expression.SetValue(expressioNode);
            }
            else
            {
                node.Expression.SetValue(ExpressionVisitor.VisitUnsignedInteger(context.position));
            }

            if (collateClause != null)
                node.Collate.SetValue(ExpressionVisitor.VisitCollateClause(collateClause));

            node.Order = context.HasToken(K_ASC) ? QsiSortOrder.Ascending : QsiSortOrder.Descending;

            if (context.children[^1] is ITerminalNode terminalNode)
            {
                node.NullBehavior = terminalNode switch
                {
                    { Symbol: { Type: K_FIRST } } => HanaOrderByNullBehavior.NullsFirst,
                    { Symbol: { Type: K_LAST } } => HanaOrderByNullBehavior.NullsLast,
                    _ => null
                };
            }

            HanaTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiLimitExpressionNode VisitLimitClause(LimitClauseContext context)
        {
            throw new NotImplementedException();
        }

        private static HanaTableBehaviorNode VisitForClause(ForClauseContext context)
        {
            throw new NotImplementedException();
        }

        // TODO: case expression contains comment
        private static QsiIdentifier GetInferredName(ParserRuleContext context)
        {
            var text = Regex.Replace(context.GetInputText(), @"\s+", string.Empty);
            return new QsiIdentifier(text.ToUpper(), false);
        }
    }
}
