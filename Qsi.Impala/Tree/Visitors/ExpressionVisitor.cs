using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Impala.Common;
using Qsi.Impala.Internal;
using Qsi.Shared.Extensions;
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
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Operator = context.KW_LOGICAL_OR().GetText();
                n.Right.SetValue(VisitExpr(context.expr(1)));

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitPredicate2(Predicate2Context context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                string func = context.KW_NOT() != null ?
                    ImpalaKnownFunction.IsNotNull :
                    ImpalaKnownFunction.IsNull;

                n.Member.SetValue(TreeHelper.CreateFunction(func));
                n.Parameters.Add(VisitExpr(context.expr()));

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitLikePredicate(Like_predicateContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Right.SetValue(VisitExpr(context.expr(1)));

                n.Operator = CreateOperator(context.KW_NOT(), context.children[^2]);

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitInPredicateSubquery(In_predicate_subqueryContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr()));
                n.Right.SetValue(VisitSubquery(context.subquery()));

                n.Operator = CreateOperator(context.KW_NOT(), context.KW_IN());

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitInPredicate(In_predicateContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr()));

                n.Right.SetValue(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                {
                    mn.Elements.AddRange(VisitExprList(context.expr_list()));

                    ImpalaTree.PutContextSpan(mn, context.expr_list());
                }));

                n.Operator = CreateOperator(context.KW_NOT(), context.KW_IN());

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitExistsPredicate(Exists_predicateContext context)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = context.KW_EXISTS().GetText();
                n.Expression.SetValue(VisitSubquery(context.subquery()));

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCompoundPredicate1(Compound_predicate1Context context)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = context.children[0].GetText();
                n.Expression.SetValue(VisitExpr(context.expr()));

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCompoundPredicate2(Compound_predicate2Context context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Right.SetValue(VisitExpr(context.expr(1)));

                n.Operator = context.children[1].GetText();

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate1(Comparison_predicate1Context context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Right.SetValue(VisitExpr(context.expr(1)));

                n.Operator = context.children[1].GetText();

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate2(Comparison_predicate2Context context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Right.SetValue(VisitExpr(context.expr(1)));

                n.Operator = CreateOperator(context.children[1], context.EQUAL());

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate3(Comparison_predicate3Context context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Right.SetValue(VisitExpr(context.expr(1)));

                n.Operator = CreateOperator(context.LESSTHAN(), context.EQUAL(), context.GREATERTHAN());

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate4(Comparison_predicate4Context context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Right.SetValue(VisitExpr(context.expr(1)));

                n.Operator = CreateOperator(context.KW_IS(), context.KW_NOT(), context.KW_DISTINCT(), context.KW_FROM());

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate5(Comparison_predicate5Context context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Right.SetValue(VisitExpr(context.expr(1)));

                n.Operator = CreateOperator(context.GREATERTHAN(), context.EQUAL());

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitBoolTestExpr(Bool_test_exprContext context)
        {
            QsiExpressionNode literalNode;

            switch (context.children[^1])
            {
                case ITerminalNode { Symbol: { Type: KW_UNKNOWN } }:
                    literalNode = TreeHelper.CreateConstantLiteral("UNKNOWN");
                    ImpalaTree.PutContextSpan(literalNode, context.KW_UNKNOWN().Symbol);
                    break;

                case ITerminalNode { Symbol: { Type: KW_TRUE } }:
                    literalNode = TreeHelper.CreateLiteral(true);
                    ImpalaTree.PutContextSpan(literalNode, context.KW_TRUE().Symbol);
                    break;

                case ITerminalNode { Symbol: { Type: KW_FALSE } }:
                    literalNode = TreeHelper.CreateLiteral(false);
                    ImpalaTree.PutContextSpan(literalNode, context.KW_FALSE().Symbol);
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context.children[^1]);
            }

            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr()));
                n.Right.SetValue(literalNode);

                n.Operator = CreateOperator(context.KW_IS(), context.KW_NOT());

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitBetweenPredicate(Between_predicateContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));

                n.Right.SetValue(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                {
                    mn.Elements.Add(VisitExpr(context.expr(1)));
                    mn.Elements.Add(VisitExpr(context.expr(2)));

                    ImpalaTree.PutContextSpan(mn, context.expr(1).Start, context.expr(2).Stop);
                }));

                n.Operator = CreateOperator(context.KW_NOT(), context.KW_BETWEEN());

                ImpalaTree.PutContextSpan(n, context);
            });
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
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = context.children[0].GetText();
                n.Expression.SetValue(VisitExpr(context.expr()));

                ImpalaTree.PutContextSpan(n, context);
            });
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
            return TreeHelper.Create<ImpalaInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.children[0].GetText()));

                if (context.expr_list() != null)
                {
                    n.Parameters.AddRange(context.expr_list().expr().Select(VisitExpr));
                }
                else
                {
                    var functionParams = context.function_params();

                    if (functionParams != null)
                    {
                        if (functionParams.HasToken(KW_DISTINCT))
                        {
                            n.Parameters.Option = functionParams.KW_DISTINCT().GetText();
                            n.Parameters.AddRange(functionParams.expr_list().expr().Select(VisitExpr));
                        }
                        else if (functionParams.HasToken(STAR))
                        {
                            var columnExpression = new QsiColumnExpressionNode();
                            columnExpression.Column.SetValue(new QsiAllColumnNode());

                            ImpalaTree.PutContextSpan(columnExpression, functionParams.STAR().Symbol);

                            n.Parameters.Add(columnExpression);
                        }
                        else if (functionParams.HasToken(KW_IGNORE))
                        {
                            n.Parameters.Option = CreateOperator(functionParams.KW_IGNORE(), functionParams.KW_NULLS());
                        }

                        if (functionParams.HasToken(KW_ALL))
                            n.Parameters.Option = functionParams.KW_ALL().GetText();

                        // TODO: test
                        if (functionParams.TryGetRuleContext<Expr_listContext>(out var exprList))
                            n.Parameters.AddRange(exprList.expr().Select(VisitExpr));
                    }
                }

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCastExpr(Cast_exprContext context)
        {
            return TreeHelper.Create<ImpalaInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(ImpalaKnownFunction.Cast));

                n.Parameters.Add(VisitExpr(context.expr()));
                n.Parameters.Add(VisitTypeDef(context.type_def()));

                // cast_format_val ignored

                ImpalaTree.PutContextSpan(n, context);
            });
        }

        private static QsiTypeExpressionNode VisitTypeDef(Type_defContext context)
        {
            var node = new QsiTypeExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };

            ImpalaTree.PutContextSpan(node, context);

            return node;
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

        public static IEnumerable<QsiExpressionNode> VisitExprList(Expr_listContext context)
        {
            return context.expr().Select(VisitExpr);
        }

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

        public static ImpalaWhereExpressionNode VisitWhereClause(Where_clauseContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaWhereExpressionNode>(context);

            if (context.hint is not null)
                node.PlanHints = TableVisitor.VisitPlanHints(context.hint);

            node.Expression.Value = VisitExpr(context.expr());

            return node;
        }

        public static ImpalaGroupingExpressionNode VisitGroupByClause(Group_by_clauseContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaGroupingExpressionNode>(context);

            switch (context)
            {
                case GroupBy1Context groupBy:
                    // ROLLUP | CUBE
                    node.GroupingSetsType = groupBy.HasToken(KW_ROLLUP) ?
                        ImpalaGroupingSetsType.Rollup :
                        ImpalaGroupingSetsType.Cube;

                    node.Items.AddRange(VisitExprList(groupBy.expr_list()));
                    break;

                case GroupBy2Context groupBy:
                    node.GroupingSetsType = ImpalaGroupingSetsType.Sets;
                    node.Items.AddRange(VisitGroupingSets(groupBy.grouping_sets()));
                    break;

                case GroupBy3Context groupBy:
                    // NONE | ROLLUP | CUBE
                    if (groupBy.HasToken(KW_ROLLUP))
                        node.GroupingSetsType = ImpalaGroupingSetsType.Rollup;
                    else if (groupBy.HasToken(KW_CUBE))
                        node.GroupingSetsType = ImpalaGroupingSetsType.Cube;

                    node.Items.AddRange(VisitExprList(groupBy.expr_list()));
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            return node;
        }

        public static IEnumerable<QsiExpressionNode> VisitGroupingSets(Grouping_setsContext context)
        {
            return context.grouping_set().Select(VisitGroupingSet);
        }

        public static QsiExpressionNode VisitGroupingSet(Grouping_setContext context)
        {
            if (context.children[0] is ITerminalNode)
            {
                var node = ImpalaTree.CreateWithSpan<QsiMultipleExpressionNode>(context);
                var exprList = context.expr_list();

                if (exprList is not null)
                    node.Elements.AddRange(VisitExprList(exprList));

                return node;
            }

            return VisitExpr(context.expr());
        }

        // TODO: Impl after adding QsiHavingExpressionNode
        // public static QsiHavingExpressionNode VisitHavingClause(Having_clauseContext context)
        // {
        //     throw new NotImplementedException();
        // }

        private static string CreateOperator(params IParseTree[] nodes)
        {
            IEnumerable<string> nodeTexts = nodes
                .Where(node => node != null)
                .Select(node => node.GetText());

            return string.Join(" ", nodeTexts);
        }
    }
}
