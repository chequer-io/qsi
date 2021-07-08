using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Impala.Common;
using Qsi.Impala.Internal;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Impala.Tree.Visitors
{
    using static ImpalaParserInternal;

    internal static class ExpressionVisitor
    {
        #region Expr
        public static QsiExpressionNode VisitExpr(ExprContext expr)
        {
            while (expr is Expr_parensContext parens)
                expr = parens.expr();

            return expr switch
            {
                Predicate1Context context => VisitPredicate1(context),
                Predicate2Context context => VisitPredicate2(context),
                Like_predicateContext context => VisitLikePredicate(context),
                In_predicate_subqueryContext context => VisitInPredicateSubquery(context),
                In_predicateContext context => VisitInPredicate(context),
                Exists_predicateContext context => VisitExistsPredicate(context),
                Compound_predicate1Context context => VisitCompoundPredicate1(context),
                Compound_predicate2Context context => VisitCompoundPredicate2(context),
                Comparison_predicate1Context context => VisitComparisonPredicate1(context),
                Comparison_predicate2Context context => VisitComparisonPredicate2(context),
                Comparison_predicate3Context context => VisitComparisonPredicate3(context),
                Comparison_predicate4Context context => VisitComparisonPredicate4(context),
                Comparison_predicate5Context context => VisitComparisonPredicate5(context),
                Bool_test_exprContext context => VisitBoolTestExpr(context),
                Between_predicateContext context => VisitBetweenPredicate(context),
                Slot_ref_Context context => VisitSlotRef(context.slot_ref()),
                Sign_chain_exprContext context => VisitSignChainExpr(context),
                Literal_Context context => VisitLiteral(context.literal()),
                Function_call_expr_Context context => VisitFunctionCallExpr(context.function_call_expr()),
                Cast_expr_Context context => VisitCastExpr(context.cast_expr()),
                Case_expr_Context context => VisitCaseExpr(context.case_expr()),
                Analytic_expr_Context context => VisitAnalyticExpr(context.analytic_expr()),
                Timestamp_arithmetic_expr1Context context => VisitTimestampArithmeticExpr1(context),
                Timestamp_arithmetic_expr2Context context => VisitTimestampArithmeticExpr2(context),
                Timestamp_arithmetic_expr3Context context => VisitTimestampArithmeticExpr3(context),
                Arithmetic_exprContext context => VisitArithmeticExpr(context),
                Arithmetic_expr_factorialContext context => VisitArithmeticExprFactorial(context),
                Arithmetic_expr_bitnotContext context => VisitArithmeticExprBitnot(context),
                Subquery_Context context => VisitSubquery(context.subquery()),
                _ => throw TreeHelper.NotSupportedTree(expr)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitPredicate1(Predicate1Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitPredicate2(Predicate2Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitLikePredicate(Like_predicateContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitInPredicateSubquery(In_predicate_subqueryContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitInPredicate(In_predicateContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitExistsPredicate(Exists_predicateContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCompoundPredicate1(Compound_predicate1Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCompoundPredicate2(Compound_predicate2Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate1(Comparison_predicate1Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate2(Comparison_predicate2Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate3(Comparison_predicate3Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate4(Comparison_predicate4Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate5(Comparison_predicate5Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitBoolTestExpr(Bool_test_exprContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitBetweenPredicate(Between_predicateContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitSlotRef(Slot_refContext context)
        {
            var columnNode = ImpalaTree.CreateWithSpan<QsiColumnReferenceNode>(context);
            columnNode.Name = IdentifierVisitor.VisitDottedPath(context.dotted_path());

            var node = ImpalaTree.CreateWithSpan<QsiColumnExpressionNode>(context);
            node.Column.Value = columnNode;

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitSignChainExpr(Sign_chain_exprContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitLiteral(LiteralContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

            switch (context.children[0])
            {
                case Numeric_literalContext numericLiteral:
                {
                    var literalSymbol = ((ITerminalNode)numericLiteral.children[0]).Symbol;

                    if (literalSymbol is { Type: INTEGER_LITERAL })
                    {
                        if (ulong.TryParse(literalSymbol.Text, out var value))
                        {
                            node.Type = QsiDataType.Numeric;
                            node.Value = value;
                        }
                        else
                        {
                            node.Type = QsiDataType.Raw;
                            node.Value = literalSymbol.Text;
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(literalSymbol.Text, out var value))
                        {
                            node.Type = QsiDataType.Decimal;
                            node.Value = value;
                        }
                        else
                        {
                            node.Type = QsiDataType.Raw;
                            node.Value = literalSymbol.Text;
                        }
                    }

                    break;
                }

                case ITerminalNode { Symbol: { Type: KW_NULL } }:
                    node.Type = QsiDataType.Null;
                    break;

                case ITerminalNode { Symbol: { Type: KW_TRUE } }:
                    node.Type = QsiDataType.Boolean;
                    node.Value = true;
                    break;

                case ITerminalNode { Symbol: { Type: KW_FALSE } }:
                    node.Type = QsiDataType.Boolean;
                    node.Value = false;
                    break;

                case ITerminalNode { Symbol: { Type: STRING_LITERAL } } terminalNode:
                    node.Type = QsiDataType.String;
                    node.Value = terminalNode.GetText()[1..^1];
                    break;

                case ITerminalNode { Symbol: { Type: KW_DATE } }:
                {
                    var valueSymbol = ((ITerminalNode)context.children[1]).Symbol;
                    var valueText = valueSymbol.Text[1..^1].Trim();

                    if (!DateTime.TryParseExact(valueText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
                        throw new QsiException(QsiError.SyntaxError, $"Invalid date literal: {valueSymbol.Text}");

                    node.Type = QsiDataType.Date;
                    node.Value = value;
                    break;
                }
            }

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitFunctionCallExpr(Function_call_exprContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCastExpr(Cast_exprContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCaseExpr(Case_exprContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitAnalyticExpr(Analytic_exprContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitTimestampArithmeticExpr1(Timestamp_arithmetic_expr1Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitTimestampArithmeticExpr2(Timestamp_arithmetic_expr2Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitTimestampArithmeticExpr3(Timestamp_arithmetic_expr3Context context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitArithmeticExpr(Arithmetic_exprContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitArithmeticExprFactorial(Arithmetic_expr_factorialContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitArithmeticExprBitnot(Arithmetic_expr_bitnotContext context)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitSubquery(SubqueryContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiTableExpressionNode>(context);
            node.Table.Value = TableVisitor.VisitSubquery(context);
            return node;
        }
        #endregion

        public static QsiMultipleOrderExpressionNode VisitOrderByClause(Opt_order_by_clauseContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(context);
            node.Orders.AddRange(VisitOrderByElements(context.order_by_elements()));
            return node;
        }

        private static IEnumerable<QsiOrderExpressionNode> VisitOrderByElements(Order_by_elementsContext context)
        {
            return context.order_by_element().Select(VisitOrderByElement);
        }

        public static ImpalaOrderExpressionNode VisitOrderByElement(Order_by_elementContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaOrderExpressionNode>(context);
            var orderParam = context.opt_order_param();
            var nullsOrderParam = context.opt_nulls_order_param();

            node.Expression.Value = VisitExpr(context.expr());

            if (orderParam != null)
            {
                node.Order = orderParam.children[0] is ITerminalNode { Symbol: { Type: KW_DESC } } ?
                    QsiSortOrder.Descending :
                    QsiSortOrder.Ascending;
            }

            if (nullsOrderParam != null)
            {
                node.NullsOrder = nullsOrderParam.children[1] is ITerminalNode { Symbol: { Type: KW_LAST } } ?
                    ImpalaNullsOrder.Last :
                    ImpalaNullsOrder.First;
            }

            return node;
        }

        public static QsiLimitExpressionNode VisitLimitOffsetClause(Opt_limit_offset_clauseContext context)
        {
            if (context.limit is null && context.offset is null)
                return null;

            var node = ImpalaTree.CreateWithSpan<QsiLimitExpressionNode>(context);

            if (context.limit is not null)
                node.Limit.Value = VisitExpr(context.limit.expr());

            if (context.offset is not null)
                node.Offset.Value = VisitExpr(context.offset.expr());

            return node;
        }

        public static IEnumerable<QsiRowValueExpressionNode> VisitValuesOperandList(Values_operand_listContext context)
        {
            return context.values_operand().Select(VisitValuesOperand);
        }

        public static QsiRowValueExpressionNode VisitValuesOperand(Values_operandContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiRowValueExpressionNode>(context);
            node.ColumnValues.AddRange(VisitSelectList(context.select_list()));
            return node;
        }

        public static IEnumerable<QsiExpressionNode> VisitSelectList(Select_listContext context)
        {
            return context._items.Select(VisitSelectListItem);
        }

        public static QsiExpressionNode VisitSelectListItem(Select_list_itemContext context)
        {
            if (context.children[0] is Star_exprContext starExpr)
            {
                var node = ImpalaTree.CreateWithSpan<QsiColumnExpressionNode>(context);
                node.Column.Value = TableVisitor.VisitStarExpr(starExpr);
                return node;
            }

            var aliasClause = context.alias_clause();
            var expr = VisitExpr(context.expr());

            if (aliasClause != null)
            {
                var node = ImpalaTree.CreateWithSpan<QsiColumnExpressionNode>(context);

                node.Column.Value = new QsiDerivedColumnNode
                {
                    Alias =
                    {
                        Value = TableVisitor.VisitAliasClause(aliasClause)
                    },
                    Expression =
                    {
                        Value = expr
                    }
                };

                return node;
            }

            return expr;
        }

        public static QsiWhereExpressionNode VisitWhereClause(Where_clauseContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiGroupingExpressionNode VisitGroupByClause(Group_by_clauseContext context)
        {
            throw new NotImplementedException();
        }

        // TODO: Impl after adding QsiHavingExpressionNode
        // public static QsiHavingExpressionNode VisitHavingClause(Having_clauseContext context)
        // {
        //     throw new NotImplementedException();
        // }
    }
}
