﻿using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.MySql.Data;
using Qsi.MySql.Tree.Common;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class TableVisitor
    {
        public static QsiTableNode VisitSelectStatement(SelectStatementContext context)
        {
            switch (context.children[0])
            {
                case QueryExpressionContext queryExpression:
                    var commonContext = new CommonQueryContext(queryExpression, context.lockingClauseList());
                    return VisitQueryExpression(commonContext);

                case QueryExpressionParensContext queryExpressionParens:
                    return VisitQueryExpressionParens(queryExpressionParens);

                case SelectStatementWithIntoContext selectStatementWithInto:
                    return VisitSelectStatementWithInto(selectStatementWithInto);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiTableNode VisitQueryExpression(in CommonQueryContext context)
        {
            var source = context.QueryExpressionBody != null ?
                VisitQueryExpressionBody(context.QueryExpressionBody) :
                VisitQueryExpressionParens(context.QueryExpressionParens);

            var contexts = new object[]
            {
                context.WithClause,
                context.OrderClause,
                context.LimitClause,
                context.ProcedureAnalyseClause,
                context.LockingClauseList
            };

            if (contexts.All(c => c == null))
                return source;

            var node = new MySqlDerivedTableNode();

            if (context.WithClause != null)
                node.Directives.SetValue(VisitWithClause(context.WithClause));

            node.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());

            node.Source.SetValue(source);

            if (context.OrderClause != null)
                node.Order.SetValue(ExpressionVisitor.VisitOrderClause(context.OrderClause));

            if (context.LimitClause != null)
                node.Limit.SetValue(ExpressionVisitor.VisitLimitClause(context.LimitClause));

            if (context.ProcedureAnalyseClause != null)
                node.ProcedureAnalyse.SetValue(VisitProcedureAnalyseClause(context.ProcedureAnalyseClause));

            if (context.LockingClauseList != null)
            {
                LockingClauseContext[] lockingClauses = context.LockingClauseList.lockingClause();
                node.Lockings.AddRange(lockingClauses.Select(VisitLockingClause));
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static MySqlSelectOptionNode VisitSelectOption(SelectOptionContext context)
        {
            var node = new MySqlSelectOptionNode();

            if (context.children[0] is ITerminalNode terminalNode)
            {
                switch (terminalNode.Symbol.Type)
                {
                    case SQL_NO_CACHE_SYMBOL:
                        node.Option = MySqlSelectOption.SqlNoCache;
                        break;

                    case SQL_CACHE_SYMBOL:
                        node.Option = MySqlSelectOption.SqlCache;
                        break;

                    case MAX_STATEMENT_TIME_SYMBOL:
                        node.Option = MySqlSelectOption.MaxStatementTime;
                        node.MaxStatementTime.SetValue(ExpressionVisitor.VisitRealUlongNumber(context.real_ulong_number()));
                        break;
                }
            }
            else
            {
                var querySpecOption = context.querySpecOption();
                var token = (ITerminalNode)querySpecOption.children[0];

                switch (token.Symbol.Type)
                {
                    case ALL_SYMBOL:
                        node.Option = MySqlSelectOption.All;
                        break;

                    case DISTINCT_SYMBOL:
                        node.Option = MySqlSelectOption.Distinct;
                        break;

                    case STRAIGHT_JOIN_SYMBOL:
                        node.Option = MySqlSelectOption.StraightJoin;
                        break;

                    case HIGH_PRIORITY_SYMBOL:
                        node.Option = MySqlSelectOption.HighPriority;
                        break;

                    case SQL_SMALL_RESULT_SYMBOL:
                        node.Option = MySqlSelectOption.SqlSmallResult;
                        break;

                    case SQL_BIG_RESULT_SYMBOL:
                        node.Option = MySqlSelectOption.SqlBigResult;
                        break;

                    case SQL_BUFFER_RESULT_SYMBOL:
                        node.Option = MySqlSelectOption.SqlBufferResult;
                        break;

                    case SQL_CALC_FOUND_ROWS_SYMBOL:
                        node.Option = MySqlSelectOption.SqlCalcFoundRows;
                        break;
                }
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiColumnsDeclarationNode VisitSelectItemList(SelectItemListContext context)
        {
            var node = new QsiColumnsDeclarationNode();

            foreach (var child in context.children)
            {
                switch (child)
                {
                    case ITerminalNode { Symbol: { Type: MULT_OPERATOR } }:
                        node.Columns.Add(new QsiAllColumnNode());
                        break;

                    case SelectItemContext selectItem:
                        node.Columns.Add(VisitSelectItem(selectItem));
                        break;
                }
            }

            return node;
        }

        public static QsiColumnNode VisitSelectItem(SelectItemContext context)
        {
            var child = context.children[0];

            if (child is TableWildContext tableWild)
                return VisitTableWild(tableWild);

            var node = VisitExpr((ExprContext)child);
            var alias = context.selectAlias();

            if (alias == null)
                return node;

            if (node is not QsiDerivedColumnNode derivedColumnNode)
            {
                derivedColumnNode = new QsiDerivedColumnNode();
                derivedColumnNode.Column.SetValue(node);
            }

            derivedColumnNode.Alias.SetValue(VisitSelectAlias(alias));
            MySqlTree.PutContextSpan(derivedColumnNode, context);

            return derivedColumnNode;
        }

        public static QsiAliasNode VisitSelectAlias(SelectAliasContext context)
        {
            var node = new QsiAliasNode
            {
                Name = context.identifier() != null ?
                    IdentifierVisitor.VisitIdentifier(context.identifier()) :
                    IdentifierVisitor.VisitTextStringLiteral(context.textStringLiteral())
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiColumnNode VisitTableWild(TableWildContext context)
        {
            var node = new QsiAllColumnNode
            {
                Path = new QsiQualifiedIdentifier(context.identifier().Select(IdentifierVisitor.VisitIdentifier))
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiColumnNode VisitExpr(ExprContext context)
        {
            var expressionNode = ExpressionVisitor.VisitExpr(context);

            if (expressionNode is QsiColumnExpressionNode columnExpression)
                return columnExpression.Column.Value;

            var node = new QsiDerivedColumnNode();

            node.Expression.SetValue(expressionNode);
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitFromClause(FromClauseContext context)
        {
            if (context.HasToken(DUAL_SYMBOL))
                return null;

            return VisitTableReferenceList(context.tableReferenceList());
        }

        public static QsiTableNode VisitTableReferenceList(TableReferenceListContext context)
        {
            QsiTableNode[] sources = context.tableReference()
                .Select(VisitTableReference)
                .ToArray();

            if (sources.Length == 1)
                return sources[0];

            var anchor = sources[0];

            for (int i = 1; i < sources.Length; i++)
            {
                var join = new QsiJoinedTableNode
                {
                    JoinType = QsiJoinType.Inner
                };

                join.Left.SetValue(anchor);
                join.Right.SetValue(sources[i]);

                var leftSpan = MySqlTree.Span[join.Left.Value];
                var rightSpan = MySqlTree.Span[join.Right.Value];

                MySqlTree.Span[join] = new Range(leftSpan.Start, rightSpan.End);

                anchor = join;
            }

            return anchor;
        }

        public static QsiTableNode VisitTableReference(TableReferenceContext context)
        {
            if (context.children[0] is not TableFactorContext tableFactor)
                throw TreeHelper.NotSupportedFeature("ODBC Join");

            var source = VisitTableFactor(tableFactor);

            foreach (var joinedTable in context.joinedTable())
                source = VisitJoinedTable(joinedTable, source);

            return source;
        }

        public static QsiTableNode VisitEscapedTableReference(EscapedTableReferenceContext context)
        {
            var source = VisitTableFactor(context.tableFactor());

            foreach (var joinedTable in context.joinedTable())
                source = VisitJoinedTable(joinedTable, source);

            return source;
        }

        public static QsiTableNode VisitTableFactor(TableFactorContext context)
        {
            var child = context.children[0];

            switch (child)
            {
                case SingleTableContext singleTable:
                    return VisitSingleTable(singleTable);

                case SingleTableParensContext singleTableParens:
                    return VisitSingleTableParens(singleTableParens);

                case DerivedTableContext derivedTable:
                    return VisitDerivedTable(derivedTable);

                case TableReferenceListParensContext tableReferenceListParens:
                    return VisitTableReferenceListParens(tableReferenceListParens);

                case TableFunctionContext tableFunction:
                    return VisitTableFunction(tableFunction);

                default:
                    throw TreeHelper.NotSupportedTree(child);
            }
        }

        public static QsiTableNode VisitSingleTable(SingleTableContext context)
        {
            var tableAccess = VisitTableRef(context.tableRef());
            var alias = context.tableAlias();

            // TODO: usePartition
            // TODO: indexHintList

            if (alias == null)
                return tableAccess;

            var derivedTableNode = new QsiDerivedTableNode();

            derivedTableNode.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
            derivedTableNode.Source.SetValue(tableAccess);
            derivedTableNode.Alias.SetValue(VisitTableAlias(alias));

            MySqlTree.PutContextSpan(derivedTableNode, context);

            return derivedTableNode;
        }

        public static QsiTableAccessNode VisitTableRef(TableRefContext context)
        {
            var node = new QsiTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitTableRef(context)
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiAliasNode VisitTableAlias(TableAliasContext context)
        {
            var node = new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(context.identifier())
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static SingleTableParensContext UnwrapSingleTableParens(SingleTableParensContext context)
        {
            do
            {
                var nestedContext = context.singleTableParens();

                if (nestedContext == null)
                    break;

                context = nestedContext;
            } while (true);

            return context;
        }

        public static QsiTableNode VisitSingleTableParens(SingleTableParensContext context)
        {
            var c = UnwrapSingleTableParens(context);
            var node = VisitSingleTable(c.singleTable());

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitDerivedTable(DerivedTableContext context)
        {
            var subqueryNode = VisitSubquery(context.subquery());
            var tableAlias = context.tableAlias();

            if (tableAlias == null)
                return subqueryNode;

            var node = new QsiDerivedTableNode();
            var columnInternalRefList = context.columnInternalRefList();

            node.Source.SetValue(subqueryNode);
            node.Alias.SetValue(VisitTableAlias(tableAlias));

            node.Columns.SetValue(columnInternalRefList == null ?
                TreeHelper.CreateAllColumnsDeclaration() :
                CreateSequentialColumns(columnInternalRefList));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static TableReferenceListParensContext UnwrapTableReferenceListParens(TableReferenceListParensContext context)
        {
            do
            {
                var nestedContext = context.tableReferenceListParens();

                if (nestedContext == null)
                    break;

                context = nestedContext;
            } while (true);

            return context;
        }

        public static QsiTableNode VisitTableReferenceListParens(TableReferenceListParensContext context)
        {
            var c = UnwrapTableReferenceListParens(context);
            var node = VisitTableReferenceList(c.tableReferenceList());

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitTableFunction(TableFunctionContext context)
        {
            throw TreeHelper.NotSupportedFeature("Table function");
        }

        public static QsiJoinedTableNode VisitJoinedTable(JoinedTableContext context, QsiTableNode left)
        {
            var node = new QsiJoinedTableNode();
            var child = context.children[0];

            node.Left.SetValue(left);

            switch (child)
            {
                case InnerJoinTypeContext innerJoinType:
                    node.JoinType = VisitInnerJoinType(innerJoinType);
                    node.Right.SetValue(VisitTableReference(context.tableReference()));
                    break;

                case OuterJoinTypeContext outerJoinType:
                    node.JoinType = VisitOuterJoinType(outerJoinType);
                    node.Right.SetValue(VisitTableReference(context.tableReference()));
                    break;

                case NaturalJoinTypeContext naturalJoinType:
                    node.JoinType = VisitNaturalJoinType(naturalJoinType);
                    node.Right.SetValue(VisitTableFactor(context.tableFactor()));
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(child);
            }

            var usingList = context.identifierListWithParentheses();

            if (usingList != null)
                node.PivotColumns.SetValue(VisitIdentifierListWithParentheses(usingList));

            var leftSpan = MySqlTree.Span[node.Left.Value];
            var rightSpan = MySqlTree.Span[node.Right.Value];

            MySqlTree.Span[node] = new Range(leftSpan.Start, rightSpan.End);

            return node;
        }

        public static QsiJoinType VisitOuterJoinType(OuterJoinTypeContext context)
        {
            var outer = context.HasToken(OUTER_SYMBOL);

            if (context.HasToken(LEFT_SYMBOL))
            {
                if (outer)
                    return QsiJoinType.LeftOuter;

                return QsiJoinType.Left;
            }

            if (outer)
                return QsiJoinType.RightOuter;

            return QsiJoinType.Right;
        }

        public static QsiJoinType VisitInnerJoinType(InnerJoinTypeContext context)
        {
            if (context.HasToken(STRAIGHT_JOIN_SYMBOL))
                return QsiJoinType.Straight;

            if (context.HasToken(CROSS_SYMBOL))
                return QsiJoinType.Cross;

            return QsiJoinType.Inner;
        }

        public static QsiJoinType VisitNaturalJoinType(NaturalJoinTypeContext context)
        {
            if (context.HasToken(INNER_SYMBOL))
                return QsiJoinType.NaturalInner;

            var left = context.HasToken(LEFT_SYMBOL);

            if (context.HasToken(OUTER_SYMBOL))
                return left ? QsiJoinType.NaturalLeft : QsiJoinType.NaturalRight;

            return left ? QsiJoinType.NaturalLeftOuter : QsiJoinType.NaturalRightOuter;
        }

        public static QsiColumnsDeclarationNode VisitIdentifierListWithParentheses(IdentifierListWithParenthesesContext context)
        {
            var node = new QsiColumnsDeclarationNode();

            IEnumerable<QsiIdentifier> identifiers = context
                .identifierList().identifier()
                .Select(IdentifierVisitor.VisitIdentifier);

            node.Columns.AddRange(identifiers.Select(i =>
                new QsiDeclaredColumnNode
                {
                    Name = new QsiQualifiedIdentifier(i)
                }));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitQueryExpressionBody(QueryExpressionBodyContext context)
        {
            QsiTableNode[] sources = context.children
                .Where(c => c is QueryPrimaryContext || c is QueryExpressionParensContext)
                .Select(c =>
                {
                    if (c is QueryExpressionParensContext queryExpressionParens)
                        return VisitQueryExpressionParens(queryExpressionParens);

                    return VisitQueryPrimary((QueryPrimaryContext)c);
                })
                .ToArray();

            if (sources.Length == 1)
                return sources[0];

            var node = new QsiCompositeTableNode();

            node.Sources.AddRange(sources);
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitQueryPrimary(QueryPrimaryContext context)
        {
            switch (context.children[0])
            {
                case QuerySpecificationContext querySpecification:
                    return VisitQuerySpecification(querySpecification);

                case TableValueConstructorContext tableValueConstructor:
                    return VisitTableValueConstructor(tableValueConstructor);

                case ExplicitTableContext explicitTable:
                    return VisitExplicitTable(explicitTable);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiTableNode VisitQuerySpecification(QuerySpecificationContext context)
        {
            var node = new MySqlDerivedTableNode();

            foreach (var child in context.children)
            {
                switch (child)
                {
                    case SelectOptionContext selectOption:
                        node.SelectOptions.Add(VisitSelectOption(selectOption));
                        break;

                    case SelectItemListContext selectItemList:
                        node.Columns.SetValue(VisitSelectItemList(selectItemList));
                        break;

                    case FromClauseContext fromClause:
                        var source = VisitFromClause(fromClause);

                        if (source != null)
                            node.Source.SetValue(source);

                        break;

                    case WhereClauseContext whereClause:
                        node.Where.SetValue(ExpressionVisitor.VisitWhereClause(whereClause));
                        break;

                    case GroupByClauseContext groupByClause:
                        node.Grouping.SetValue(ExpressionVisitor.VisitGroupByClause(groupByClause));
                        break;

                    case HavingClauseContext havingClause when !node.Grouping.IsEmpty:
                        node.Grouping.Value.Having.SetValue(ExpressionVisitor.VisitHavingClause(havingClause));
                        break;

                    case IntoClauseContext:
                    case WindowClauseContext:
                    case ITerminalNode:
                        // Skip
                        break;

                    default:
                        throw new QsiException(QsiError.Syntax);
                }
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitTableValueConstructor(TableValueConstructorContext context)
        {
            var node = new QsiInlineDerivedTableNode();

            node.Rows.AddRange(context.rowValueExplicit().Select(VisitRowValueExplicit));
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiRowValueExpressionNode VisitRowValueExplicit(RowValueExplicitContext context)
        {
            var node = new QsiRowValueExpressionNode();

            foreach (var value in context.values().children)
            {
                switch (value)
                {
                    case ITerminalNode terminalNode when terminalNode.Symbol.Type == COMMA_SYMBOL:
                        continue;

                    case ExprContext expr:
                        node.ColumnValues.Add(ExpressionVisitor.VisitExpr(expr));
                        break;

                    default:
                        node.ColumnValues.Add(TreeHelper.CreateDefaultLiteral());
                        break;
                }
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitExplicitTable(ExplicitTableContext context)
        {
            var node = new MySqlExplicitTableAccessNode
            {
                Identifier = IdentifierVisitor.VisitTableRef(context.tableRef())
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QueryExpressionParensContext UnwrapQueryExpressionParens(QueryExpressionParensContext context)
        {
            do
            {
                var nestedContext = context.queryExpressionParens();

                if (nestedContext == null)
                    break;

                context = nestedContext;
            } while (true);

            return context;
        }

        public static QsiTableNode VisitQueryExpressionParens(QueryExpressionParensContext context)
        {
            var parens = UnwrapQueryExpressionParens(context);
            var commonContext = new CommonQueryContext(parens.queryExpression(), parens.lockingClauseList());
            var node = VisitQueryExpression(commonContext);

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitSelectStatementWithInto(SelectStatementWithIntoContext context)
        {
            throw TreeHelper.NotSupportedFeature("select into");
        }

        public static QsiTableDirectivesNode VisitWithClause(WithClauseContext context)
        {
            var node = new QsiTableDirectivesNode
            {
                IsRecursive = context.HasToken(RECURSIVE_SYMBOL)
            };

            node.Tables.AddRange(context.commonTableExpression().Select(VisitCommonTableExpression));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiDerivedTableNode VisitCommonTableExpression(CommonTableExpressionContext context)
        {
            var node = new QsiDerivedTableNode();
            var columnInternalRefList = context.columnInternalRefList();

            node.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(context.identifier())
            });

            node.Columns.SetValue(columnInternalRefList == null ?
                TreeHelper.CreateAllColumnsDeclaration() :
                CreateSequentialColumns(columnInternalRefList));

            node.Source.SetValue(VisitSubquery(context.subquery()));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiColumnsDeclarationNode CreateSequentialColumns(ColumnInternalRefListContext context)
        {
            var node = new QsiColumnsDeclarationNode();

            node.Columns.AddRange(context.columnInternalRef().Select(VisitSequentialColumn));
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiSequentialColumnNode VisitSequentialColumn(ColumnInternalRefContext context)
        {
            var node = new QsiSequentialColumnNode();

            node.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.VisitIdentifier(context.identifier())
            });

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTableNode VisitSubquery(SubqueryContext context)
        {
            return VisitQueryExpressionParens(context.queryExpressionParens());
        }

        public static MySqlProcedureAnalyseNode VisitProcedureAnalyseClause(ProcedureAnalyseClauseContext context)
        {
            var node = new MySqlProcedureAnalyseNode();
            ITerminalNode[] numbers = context.INT_NUMBER();

            node.MaxElements = int.Parse(numbers[0].GetText());

            if (numbers.Length == 2)
                node.MaxMemory = int.Parse(numbers[1].GetText());

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static MySqlLockingNode VisitLockingClause(LockingClauseContext context)
        {
            var node = new MySqlLockingNode();
            var lockStrengh = context.lockStrengh();

            if (lockStrengh != null)
            {
                node.TableLockType = lockStrengh.HasToken(UPDATE_SYMBOL) ?
                    MySqlTableLockType.Update : MySqlTableLockType.Share;

                node.Tables = context.tableAliasRefList()?
                    .tableRefWithWildcard()
                    .Select(IdentifierVisitor.VisitTableRefWithWildcard)
                    .ToArray();

                var lockedRowAction = context.lockedRowAction();

                if (lockedRowAction != null)
                {
                    node.RowLockType = lockedRowAction.HasToken(SKIP_SYMBOL) ?
                        MySqlRowLockType.SkipLocked : MySqlRowLockType.NoWait;
                }
            }
            else
            {
                node.TableLockType = MySqlTableLockType.ShareMode;
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
    }
}
