using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Impala.Common;
using Qsi.Impala.Internal;
using Qsi.Impala.Utilities;
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
                Comparison_predicate6Context context => VisitComparisonPredicate6(context),
                Comparison_predicate7Context context => VisitComparisonPredicate7(context),
                Comparison_predicate8Context context => VisitComparisonPredicate8(context),
                Comparison_predicate9Context context => VisitComparisonPredicate9(context),
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
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l);
            node.Operator = context.KW_LOGICAL_OR().GetText();
            node.Right.Value = VisitExpr(context.r);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitPredicate2(Predicate2Context context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            string func = context.KW_NOT() is not null ?
                ImpalaKnownFunction.IsNotNull :
                ImpalaKnownFunction.IsNull;

            node.Member.Value = TreeHelper.CreateFunction(func);
            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitLikePredicate(Like_predicateContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l);
            node.Right.Value = VisitExpr(context.r);

            node.Operator = CreateOperator(context.KW_NOT(), context.children[^2]);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitInPredicateSubquery(In_predicate_subqueryContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.expr());
            node.Right.Value = VisitSubquery(context.subquery());

            node.Operator = CreateOperator(context.KW_NOT(), context.KW_IN());

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitInPredicate(In_predicateContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.expr());

            var multipleNode = ImpalaTree.CreateWithSpan<QsiMultipleExpressionNode>(context.expr_list());
            multipleNode.Elements.AddRange(VisitExprList(context.expr_list()));

            node.Right.Value = multipleNode;

            node.Operator = CreateOperator(context.KW_NOT(), context.KW_IN());

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitExistsPredicate(Exists_predicateContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiUnaryExpressionNode>(context);

            node.Operator = context.KW_EXISTS().GetText();
            node.Expression.Value = VisitSubquery(context.subquery());

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCompoundPredicate1(Compound_predicate1Context context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiUnaryExpressionNode>(context);

            node.Operator = context.children[0].GetText();
            node.Expression.Value = VisitExpr(context.expr());

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCompoundPredicate2(Compound_predicate2Context context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l);
            node.Right.Value = VisitExpr(context.r);

            node.Operator = context.children[1].GetText();

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode CreateComparisonPredicate(ExprContext left, string op, ExprContext right)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(left.Start, right.Stop);

            node.Left.Value = VisitExpr(left);
            node.Operator = op;
            node.Right.Value = VisitExpr(right);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate1(Comparison_predicate1Context context)
        {
            var op = context.HasToken(NOT) ? "!=" : "=";
            return CreateComparisonPredicate(context.l, op, context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate2(Comparison_predicate2Context context)
        {
            return CreateComparisonPredicate(context.l, "!=", context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate3(Comparison_predicate3Context context)
        {
            return CreateComparisonPredicate(context.l, "<>", context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate4(Comparison_predicate4Context context)
        {
            return CreateComparisonPredicate(context.l, "<=", context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate5(Comparison_predicate5Context context)
        {
            return CreateComparisonPredicate(context.l, ">=", context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate6(Comparison_predicate6Context context)
        {
            return CreateComparisonPredicate(context.l, "<", context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate7(Comparison_predicate7Context context)
        {
            return CreateComparisonPredicate(context.l, ">", context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate8(Comparison_predicate8Context context)
        {
            return CreateComparisonPredicate(context.l, "<=>", context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitComparisonPredicate9(Comparison_predicate9Context context)
        {
            var op = context.HasToken(NOT) ?
                "IS NOT DISTINCT FROM" :
                "IS DISTINCT FROM";

            return CreateComparisonPredicate(context.l, op, context.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitBoolTestExpr(Bool_test_exprContext context)
        {
            QsiExpressionNode literalNode;

            switch (context.children[^1])
            {
                case ITerminalNode { Symbol: { Type: KW_UNKNOWN } } terminalNode:
                    literalNode = VisitUnknown(terminalNode);
                    break;

                case ITerminalNode { Symbol: { Type: KW_TRUE } } terminalNode:
                    literalNode = VisitTrue(terminalNode);
                    break;

                case ITerminalNode { Symbol: { Type: KW_FALSE } } terminalNode:
                    literalNode = VisitFalse(terminalNode);
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context.children[^1]);
            }

            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.expr());
            node.Right.Value = literalNode;

            node.Operator = CreateOperator(context.KW_IS(), context.KW_NOT());

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitBetweenPredicate(Between_predicateContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(
                context.HasToken(KW_NOT) ?
                    ImpalaKnownFunction.NotBetween :
                    ImpalaKnownFunction.Between
            );

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
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
            var node = ImpalaTree.CreateWithSpan<QsiUnaryExpressionNode>(context);

            node.Operator = context.children[0].GetText();
            node.Expression.Value = VisitExpr(context.expr());

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitLiteral(LiteralContext context)
        {
            switch (context.children[0])
            {
                case Numeric_literalContext numericLiteralContext:
                    return VisitNumericLiteral(numericLiteralContext);

                case ITerminalNode { Symbol: { Type: KW_NULL } } terminalNode:
                    return VisitNull(terminalNode);

                case ITerminalNode { Symbol: { Type: KW_TRUE } } terminalNode:
                    return VisitTrue(terminalNode);

                case ITerminalNode { Symbol: { Type: KW_FALSE } } terminalNode:
                    return VisitFalse(terminalNode);

                case ITerminalNode { Symbol: { Type: STRING_LITERAL } } terminalNode:
                    return VisitStringLiteral(terminalNode);

                case Date_literalContext dateLiteralContext:
                    return VisitDateLiteral(dateLiteralContext);
            }

            throw TreeHelper.NotSupportedTree(context.children[0]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitUnknown(ITerminalNode terminalNode)
        {
            TreeHelper.VerifyTokenType(terminalNode.Symbol, KW_UNKNOWN);

            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(terminalNode.Symbol);

            node.Type = QsiDataType.Constant;
            node.Value = "UNKNOWN";

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitNull(ITerminalNode terminalNode)
        {
            TreeHelper.VerifyTokenType(terminalNode.Symbol, KW_NULL);

            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(terminalNode.Symbol);

            node.Type = QsiDataType.Null;

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitTrue(ITerminalNode terminalNode)
        {
            TreeHelper.VerifyTokenType(terminalNode.Symbol, KW_TRUE);

            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(terminalNode.Symbol);

            node.Type = QsiDataType.Boolean;
            node.Value = true;

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitFalse(ITerminalNode terminalNode)
        {
            TreeHelper.VerifyTokenType(terminalNode.Symbol, KW_FALSE);

            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(terminalNode.Symbol);

            node.Type = QsiDataType.Boolean;
            node.Value = false;

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitStringLiteral(ITerminalNode terminalNode)
        {
            TreeHelper.VerifyTokenType(terminalNode.Symbol, STRING_LITERAL);

            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(terminalNode.Symbol);

            node.Type = QsiDataType.String;
            node.Value = terminalNode.GetText()[1..^1];

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitDateLiteral(Date_literalContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

            var valueSymbol = ((ITerminalNode)context.children[1]).Symbol;
            var valueText = valueSymbol.Text[1..^1].Trim();

            if (!DateTime.TryParseExact(valueText, "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
                throw new QsiException(QsiError.SyntaxError, $"Invalid date literal: {valueSymbol.Text}");

            node.Type = QsiDataType.Date;
            node.Value = value;

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitNumericLiteral(Numeric_literalContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            var literalSymbol = ((ITerminalNode)context.children[0]).Symbol;

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

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitFunctionCallExpr(Function_call_exprContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(context.children[0].GetText());

            if (context.expr_list() is not null)
            {
                node.Parameters.AddRange(context.expr_list().expr().Select(VisitExpr));
            }
            else
            {
                var functionParams = context.function_params();

                if (functionParams is not null)
                {
                    if (functionParams.HasToken(KW_DISTINCT))
                    {
                        node.Parameters.Option = functionParams.KW_DISTINCT().GetText();
                        node.Parameters.AddRange(functionParams.expr_list().expr().Select(VisitExpr));
                    }
                    else if (functionParams.HasToken(STAR))
                    {
                        var columnExpression = new QsiColumnExpressionNode();
                        columnExpression.Column.Value = new QsiAllColumnNode();

                        ImpalaTree.PutContextSpan(columnExpression, functionParams.STAR().Symbol);

                        node.Parameters.Add(columnExpression);
                    }
                    else if (functionParams.HasToken(KW_IGNORE))
                    {
                        node.Parameters.Option = CreateOperator(functionParams.KW_IGNORE(), functionParams.KW_NULLS());
                    }

                    if (functionParams.HasToken(KW_ALL))
                        node.Parameters.Option = functionParams.KW_ALL().GetText();

                    if (functionParams.TryGetRuleContext<Expr_listContext>(out var exprList))
                        node.Parameters.AddRange(exprList.expr().Select(VisitExpr));
                }
            }

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitCastExpr(Cast_exprContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(ImpalaKnownFunction.Cast);

            node.Parameters.Add(VisitExpr(context.expr()));
            node.Parameters.Add(VisitTypeDef(context.type_def()));

            // cast_format_val ignored

            return node;
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
            var node = ImpalaTree.CreateWithSpan<QsiSwitchExpressionNode>(context);

            if (context.expr() != null)
                node.Value.Value = VisitExpr(context.expr());

            node.Cases.AddRange(VisitCaseWhenClauseList(context.case_when_clause_list()));

            if (context.case_else_clause() != null)
            {
                var elseClause = context.case_else_clause();
                var elseNode = ImpalaTree.CreateWithSpan<QsiSwitchCaseExpressionNode>(elseClause);

                elseNode.Consequent.Value = VisitExpr(elseClause.expr());

                node.Cases.Add(elseNode);
            }

            return node;
        }

        private static IEnumerable<QsiSwitchCaseExpressionNode> VisitCaseWhenClauseList(Case_when_clause_listContext context)
        {
            return context.case_when_clause().Select(VisitCaseWhenClause);
        }

        private static QsiSwitchCaseExpressionNode VisitCaseWhenClause(Case_when_clauseContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiSwitchCaseExpressionNode>(context);

            node.Condition.Value = VisitExpr(context.w);
            node.Consequent.Value = VisitExpr(context.t);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitAnalyticExpr(Analytic_exprContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(ImpalaKnownFunction.Analytic);
            node.Parameters.Add(VisitFunctionCallExpr(context.function_call_expr()));

            // partition_clause ignored
            // order_by_clause ignored
            // opt_window_clause ignored

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitTimestampArithmeticExpr1(Timestamp_arithmetic_expr1Context context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitInterval(context.l);
            node.Operator = context.op.Text;
            node.Right.Value = VisitExpr(context.r);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitTimestampArithmeticExpr2(Timestamp_arithmetic_expr2Context context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l.expr(0));
            node.Operator = context.op.GetInputText();
            node.Right.Value = VisitInterval(context.r);

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitTimestampArithmeticExpr3(Timestamp_arithmetic_expr3Context context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l);
            node.Operator = context.op.Text;
            node.Right.Value = VisitInterval(context.r);

            return node;
        }

        private static QsiExpressionNode VisitInterval(IntervalContext context)
        {
            var timeUnitText = context.u.Text;

            if (timeUnitText.EndsWith("S", StringComparison.OrdinalIgnoreCase))
                timeUnitText = timeUnitText[..^1];

            if (!Enum.TryParse<ImpalaTimeUnit>(timeUnitText, true, out _))
            {
                throw new QsiException(
                    QsiError.SyntaxError,
                    $"Invalid time unit '{context.u.Text}' in timestamp/date arithmetic expression '{context.GetInputText()}'."
                );
            }

            var node = ImpalaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(ImpalaKnownFunction.Interval);
            node.Parameters.Add(VisitExpr(context.v));
            node.Parameters.Add(TreeHelper.CreateConstantLiteral(context.u.Text));

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitArithmeticExpr(Arithmetic_exprContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l);
            node.Right.Value = VisitExpr(context.r);

            node.Operator = context.children[1].GetText();

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitArithmeticExprFactorial(Arithmetic_expr_factorialContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(ImpalaKnownFunction.Factorial);
            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static QsiExpressionNode VisitArithmeticExprBitnot(Arithmetic_expr_bitnotContext context)
        {
            var node = TreeHelper.CreateUnary(context.BITNOT().GetText(), VisitExpr(context.expr()));

            ImpalaTree.PutContextSpan(node, context);

            return node;
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

            if (orderParam is not null)
            {
                node.Order = orderParam.children[0] is ITerminalNode { Symbol: { Type: KW_DESC } } ?
                    QsiSortOrder.Descending :
                    QsiSortOrder.Ascending;
            }

            if (nullsOrderParam is not null)
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

            if (aliasClause is not null)
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
                .Where(node => node is not null)
                .Select(node => node.GetText());

            return string.Join(" ", nodeTexts);
        }

        public static ImpalaPartitionExpressionNode VisitPartitionClause(Partition_clauseContext context)
        {
            var node = ImpalaTree.CreateWithSpan<ImpalaPartitionExpressionNode>(context);
            node.Elements.AddRange(VisitPartitionKeyValueList(context.list));
            return node;
        }

        public static IEnumerable<QsiExpressionNode> VisitPartitionKeyValueList(Partition_key_value_listContext context)
        {
            return context._items.Select(VisitPartitionKeyValue);
        }

        public static QsiExpressionNode VisitPartitionKeyValue(Partition_key_valueContext context)
        {
            if (context.children[0] is Static_partition_key_valueContext staticPartitionKeyValue)
                return VisitStaticPartitionKeyValue(staticPartitionKeyValue);

            return CreateColumnExpression((Ident_or_defaultContext)context.children[0]);
        }

        private static QsiBinaryExpressionNode VisitStaticPartitionKeyValue(Static_partition_key_valueContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = CreateColumnExpression(context.key);
            node.Operator = "=";
            node.Right.Value = VisitExpr(context.value);

            return node;
        }

        private static QsiColumnExpressionNode CreateColumnExpression(Ident_or_defaultContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiColumnExpressionNode>(context);
            var columnNode = ImpalaTree.CreateWithSpan<QsiColumnReferenceNode>(context);

            columnNode.Name = new QsiQualifiedIdentifier(IdentifierVisitor.VisitIdentOrDefault(context));
            node.Column.Value = columnNode;

            return node;
        }

        public static IEnumerable<QsiSetColumnExpressionNode> VisitUpdateSetExprList(Update_set_expr_listContext context)
        {
            return context._items.Select(VisitUpdateSetExpr);
        }

        private static QsiSetColumnExpressionNode VisitUpdateSetExpr(Update_set_exprContext context)
        {
            var node = ImpalaTree.CreateWithSpan<QsiSetColumnExpressionNode>(context);
            node.Target = IdentifierVisitor.VisitDottedPath(context.slot.dotted_path());
            node.Value.Value = VisitExpr(context.e);
            return node;
        }
    }
}
