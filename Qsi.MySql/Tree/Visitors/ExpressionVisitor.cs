using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Antlr4.Runtime;
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
    internal static class ExpressionVisitor
    {
        #region UdfExpr
        public static QsiExpressionNode VisitUdfExpr(UdfExprContext context)
        {
            var expr = VisitExpr(context.expr());

            if (context.selectAlias() == null)
                return expr;

            return TreeHelper.Create<MySqlAliasedExpressionNode>(n =>
            {
                n.Expression.SetValue(expr);
                n.Alias.SetValue(VisitSelectAlias(context.selectAlias()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static IEnumerable<QsiExpressionNode> VisitUdfExprList(UdfExprListContext context)
        {
            return context.udfExpr().Select(VisitUdfExpr);
        }
        #endregion

        #region Expr
        public static QsiExpressionNode VisitSimpleExprWithParentheses(SimpleExprWithParenthesesContext context)
        {
            return VisitSimpleExpr(context.simpleExpr());
        }

        public static QsiExpressionNode VisitExprWithParentheses(ExprWithParenthesesContext context)
        {
            return VisitExpr(context.expr());
        }

        public static IEnumerable<QsiExpressionNode> VisitExprListWithParentheses(ExprListWithParenthesesContext context)
        {
            return VisitExprList(context.exprList());
        }

        public static IEnumerable<QsiExpressionNode> VisitExprList(ExprListContext context)
        {
            return context.expr().Select(VisitExpr);
        }
        #endregion

        #region Expr
        public static QsiExpressionNode VisitExpr(ExprContext context)
        {
            switch (context)
            {
                case ExprIsContext exprIs:
                    return VisitExprIs(exprIs);

                case ExprNotContext exprNot:
                    return VisitExprNot(exprNot);

                case ExprAndContext exprAnd:
                    return VisitExprAnd(exprAnd);

                case ExprXorContext exprXor:
                    return VisitExprXor(exprXor);

                case ExprOrContext exprOr:
                    return VisitExprOr(exprOr);
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitExprIs(ExprIsContext context)
        {
            var node = VisitBoolPri(context.boolPri());

            if (!context.HasToken(IS_SYMBOL))
                return node;

            bool isNot = context.notRule() != null;

            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(node);
                n.Operator = isNot ? "IS NOT" : "IS";
                n.Right.SetValue(VisitLiteralFromToken(context.type));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiUnaryExpressionNode VisitExprNot(ExprNotContext context)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = context.NOT_SYMBOL().GetText();
                n.Expression.SetValue(VisitExpr(context.expr()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitExprAnd(ExprAndContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Operator = context.op.Text;
                n.Right.SetValue(VisitExpr(context.expr(1)));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitExprXor(ExprXorContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Operator = context.XOR_SYMBOL().GetText();
                n.Right.SetValue(VisitExpr(context.expr(1)));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitExprOr(ExprOrContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpr(context.expr(0)));
                n.Operator = context.op.Text;
                n.Right.SetValue(VisitExpr(context.expr(1)));

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region BoolPri
        public static QsiExpressionNode VisitBoolPri(BoolPriContext context)
        {
            switch (context)
            {
                case PrimaryExprPredicateContext primaryExprPredicate:
                    return VisitPredicate(primaryExprPredicate.predicate());

                case PrimaryExprIsNullContext primaryExprIsNull:
                    return VisitPrimaryExprIsNull(primaryExprIsNull);

                case PrimaryExprCompareContext primaryExprCompare:
                    return VisitPrimaryExprCompare(primaryExprCompare);

                case PrimaryExprAllAnyContext primaryExprAllAny:
                    return VisitPrimaryExprAllAny(primaryExprAllAny);
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPrimaryExprIsNull(PrimaryExprIsNullContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                bool isNot = context.notRule() != null;

                n.Member.SetValue(TreeHelper.CreateFunction(isNot ? MySqlKnownFunction.IsNotNull : MySqlKnownFunction.IsNull));
                n.Parameters.Add(VisitBoolPri(context.boolPri()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitPrimaryExprCompare(PrimaryExprCompareContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitBoolPri(context.boolPri()));
                n.Operator = context.compOp().GetText();
                n.Right.SetValue(VisitPredicate(context.predicate()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitPrimaryExprAllAny(PrimaryExprAllAnyContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitBoolPri(context.boolPri()));
                n.Operator = context.compOp().GetText() + " " + context.children[2].GetText();
                n.Right.SetValue(VisitSubquery(context.subquery()));

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Predicate
        public static QsiExpressionNode VisitPredicate(PredicateContext context)
        {
            // <bitExpr> <not>? <predicateOperations>
            if (context.predicateOperations() != null)
            {
                return VisitPredicateWithOperations(context);
            }

            // <bitExpr> MEMBER OF? <simpleExprWithParentheses>
            if (context.HasToken(MEMBER_SYMBOL))
            {
                return VisitPredicateWithMember(context);
            }

            // <bitExpr> SOUNDS LIKE <bitExpr>
            if (context.HasToken(SOUNDS_SYMBOL))
            {
                return VisitPredicateWithSoundsLike(context);
            }

            // <bitExpr>
            return VisitBitExpr(context.bitExpr(0));
        }

        public static QsiBinaryExpressionNode VisitPredicateWithOperations(PredicateContext context)
        {
            QsiBinaryExpressionNode node;
            bool isNot = context.notRule() != null;

            switch (context.predicateOperations())
            {
                case PredicateExprInContext exprIn:
                    node = TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                    {
                        n.Operator = exprIn.IN_SYMBOL().GetText();

                        if (exprIn.subquery() != null)
                        {
                            n.Right.SetValue(VisitSubquery(exprIn.subquery()));
                        }
                        else
                        {
                            n.Right.SetValue(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                            {
                                mn.Elements.AddRange(VisitExprList(exprIn.exprList()));

                                MySqlTree.PutContextSpan(mn, exprIn.exprList());
                            }));
                        }
                    });

                    break;

                case PredicateExprBetweenContext exprBetween:
                    node = TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                    {
                        n.Operator = exprBetween.BETWEEN_SYMBOL().GetText();

                        n.Right.SetValue(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                        {
                            mn.Elements.Add(VisitBitExpr(exprBetween.bitExpr()));
                            mn.Elements.Add(VisitPredicate(exprBetween.predicate()));

                            MySqlTree.PutContextSpan(mn, exprBetween.bitExpr().Start, exprBetween.predicate().Stop);
                        }));
                    });

                    break;

                case PredicateExprLikeContext exprLike:
                    node = TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                    {
                        n.Operator = exprLike.LIKE_SYMBOL().GetText();

                        n.Right.SetValue(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                        {
                            mn.Elements.Add(VisitSimpleExpr(exprLike.simpleExpr(0)));

                            if (exprLike.HasToken(ESCAPE_SYMBOL))
                            {
                                mn.Elements.Add(VisitSimpleExpr(exprLike.simpleExpr(1)));
                                MySqlTree.PutContextSpan(mn, exprLike.simpleExpr(0).Start, exprLike.simpleExpr(1).Stop);
                            }
                            else
                            {
                                MySqlTree.PutContextSpan(mn, exprLike.simpleExpr(0));
                            }
                        }));
                    });

                    break;

                case PredicateExprRegexContext exprRegex:
                    node = TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                    {
                        n.Operator = exprRegex.REGEXP_SYMBOL().GetText();
                        n.Right.SetValue(VisitBitExpr(exprRegex.bitExpr()));
                    });

                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            if (isNot)
                node.Operator = "NOT " + node.Operator;

            node.Left.SetValue(VisitBitExpr(context.bitExpr(0)));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiInvokeExpressionNode VisitPredicateWithMember(PredicateContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(MySqlKnownFunction.MemberOf));

                n.Parameters.Add(VisitBitExpr(context.bitExpr(0)));
                n.Parameters.Add(VisitSimpleExprWithParentheses(context.simpleExprWithParentheses()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitPredicateWithSoundsLike(PredicateContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitBitExpr(context.bitExpr(0)));
                n.Operator = context.SOUNDS_SYMBOL().GetText() + " " + context.LIKE_SYMBOL().GetText();
                n.Right.SetValue(VisitBitExpr(context.bitExpr(1)));

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        public static QsiExpressionNode VisitBitExpr(BitExprContext context)
        {
            // <bitExpr> operator INTERVAL <expr> <interval>
            if (context.HasToken(INTERVAL_SYMBOL))
            {
                return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                {
                    n.Left.SetValue(VisitBitExpr(context.bitExpr(0)));
                    n.Operator = context.op.Text + " " + context.INTERVAL_SYMBOL().GetText();

                    n.Right.SetValue(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                    {
                        mn.Elements.Add(VisitExpr(context.expr()));
                        mn.Elements.Add(VisitInterval(context.interval()));

                        MySqlTree.PutContextSpan(n, context.expr().Start, context.interval().Stop);
                    }));

                    MySqlTree.PutContextSpan(n, context);
                });
            }

            // <bitExpr> operator <bitExpr>
            if (context.op != null)
            {
                return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                {
                    n.Left.SetValue(VisitBitExpr(context.bitExpr(0)));
                    n.Operator = context.op.Text;
                    n.Right.SetValue(VisitBitExpr(context.bitExpr(1)));
                });
            }

            return VisitSimpleExpr(context.simpleExpr());
        }

        public static QsiExpressionNode VisitSimpleExpr(SimpleExprContext context)
        {
            switch (context)
            {
                case SimpleExprVariableContext simpleExprVariable:
                    return VisitSimpleExprVariable(simpleExprVariable);

                case SimpleExprColumnRefContext simpleExprColumnRef:
                    return VisitSimpleExprColumnRef(simpleExprColumnRef);

                case SimpleExprRuntimeFunctionContext simpleExprRuntimeFunction:
                    return VisitSimpleExprRuntimeFunction(simpleExprRuntimeFunction);

                case SimpleExprFunctionContext simpleExprFunction:
                    return VisitSimpleExprFunction(simpleExprFunction);

                case SimpleExprCollateContext simpleExprCollate:
                    return VisitSimpleExprCollate(simpleExprCollate);

                case SimpleExprLiteralContext simpleExprLiteral:
                    return VisitSimpleExprLiteral(simpleExprLiteral);

                case SimpleExprParamMarkerContext simpleExprParamMarker:
                    return VisitSimpleExprParamMarker(simpleExprParamMarker);

                case SimpleExprSumContext simpleExprSum:
                    return VisitSimpleExprSum(simpleExprSum);

                case SimpleExprGroupingOperationContext simpleExprGroupingOperation:
                    return VisitSimpleExprGroupingOperation(simpleExprGroupingOperation);

                case SimpleExprWindowingFunctionContext simpleExprWindowingFunction:
                    return VisitSimpleExprWindowingFunction(simpleExprWindowingFunction);

                case SimpleExprConcatContext simpleExprConcat:
                    return VisitSimpleExprConcat(simpleExprConcat);

                case SimpleExprUnaryContext simpleExprUnary:
                    return VisitSimpleExprUnary(simpleExprUnary);

                case SimpleExprNotContext simpleExprNot:
                    return VisitSimpleExprNot(simpleExprNot);

                case SimpleExprListContext simpleExprList:
                    return VisitSimpleExprList(simpleExprList);

                case SimpleExprSubQueryContext simpleExprSubQuery:
                    return VisitSimpleExprSubQuery(simpleExprSubQuery);

                case SimpleExprOdbcContext:
                    throw TreeHelper.NotSupportedFeature("ODBC Expression");

                case SimpleExprMatchContext simpleExprMatch:
                    return VisitSimpleExprMatch(simpleExprMatch);

                case SimpleExprBinaryContext simpleExprBinary:
                    return VisitSimpleExprBinary(simpleExprBinary);

                case SimpleExprCastContext simpleExprCast:
                    return VisitSimpleExprCast(simpleExprCast);

                case SimpleExprCaseContext simpleExprCase:
                    return VisitSimpleExprCase(simpleExprCase);

                case SimpleExprConvertContext simpleExprConvert:
                    return VisitSimpleExprConvert(simpleExprConvert);

                case SimpleExprConvertUsingContext simpleExprConvertUsing:
                    return VisitSimpleExprConvertUsing(simpleExprConvertUsing);

                case SimpleExprDefaultContext simpleExprDefault:
                    return VisitSimpleExprDefault(simpleExprDefault);

                case SimpleExprValuesContext simpleExprValues:
                    return VisitSimpleExprValues(simpleExprValues);

                case SimpleExprIntervalContext simpleExprInterval:
                    return VisitSimpleExprInterval(simpleExprInterval);
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        #region SimpleExpr
        public static QsiExpressionNode VisitSimpleExprVariable(SimpleExprVariableContext context)
        {
            var variable = VisitVariable(context.variable());

            if (context.equal() == null)
                return variable;

            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(variable);
                n.Operator = context.equal().GetText();
                n.Right.SetValue(VisitExpr(context.expr()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitSimpleExprColumnRef(SimpleExprColumnRefContext context)
        {
            var columnNode = TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(VisitColumnRef(context.columnRef()));

                MySqlTree.PutContextSpan(n, context.columnRef());
            });

            if (context.jsonOperator() == null)
                return columnNode;

            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                var jsonOperator = context.jsonOperator();

                n.Left.SetValue(columnNode);
                n.Operator = jsonOperator.children[0].GetText();
                n.Right.SetValue(VisitTextStringLiteral(jsonOperator.textStringLiteral()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiDeclaredColumnNode VisitColumnRef(ColumnRefContext context)
        {
            return TreeHelper.Create<QsiDeclaredColumnNode>(n =>
            {
                n.Name = IdentifierVisitor.VisitFieldIdentifier(context.fieldIdentifier());

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitSimpleExprRuntimeFunction(SimpleExprRuntimeFunctionContext context)
        {
            return VisitRuntimeFunctionCall(context.runtimeFunctionCall());
        }

        public static QsiInvokeExpressionNode VisitSimpleExprFunction(SimpleExprFunctionContext context)
        {
            return VisitFunctionCall(context.functionCall());
        }

        public static MySqlCollationExpressionNode VisitSimpleExprCollate(SimpleExprCollateContext context)
        {
            return TreeHelper.Create<MySqlCollationExpressionNode>(n =>
            {
                n.Expression.SetValue(VisitSimpleExpr(context.simpleExpr()));
                n.Collate = VisitTextOrIdentifier(context.textOrIdentifier());

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitSimpleExprLiteral(SimpleExprLiteralContext context)
        {
            return VisitLiteral(context.literal());
        }

        public static QsiColumnExpressionNode VisitSimpleExprParamMarker(SimpleExprParamMarkerContext context)
        {
            var terminalNode = context.PARAM_MARKER();

            var contextWrapper = new ParserRuleContextWrapper<ITerminalNode>(
                terminalNode,
                terminalNode.Symbol,
                terminalNode.Symbol
            );

            return VisitParamMarker(contextWrapper);
        }

        private static QsiColumnExpressionNode VisitParamMarker(ParserRuleContextWrapper<ITerminalNode> context)
        {
            var node = new QsiColumnExpressionNode();

            node.Column.SetValue(new QsiBindingColumnNode()
            {
                Id = context.Value.Symbol.Text
            });

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiInvokeExpressionNode VisitSimpleExprSum(SimpleExprSumContext context)
        {
            return VisitSumExpr(context.sumExpr());
        }

        public static QsiInvokeExpressionNode VisitSimpleExprGroupingOperation(SimpleExprGroupingOperationContext context)
        {
            return VisitGroupingOperation(context.groupingOperation());
        }

        public static QsiExpressionNode VisitSimpleExprWindowingFunction(SimpleExprWindowingFunctionContext context)
        {
            return VisitWindowFunctionCall(context.windowFunctionCall());
        }

        public static QsiBinaryExpressionNode VisitSimpleExprConcat(SimpleExprConcatContext context)
        {
            var node = TreeHelper.CreateBinaryExpression(
                context.children[1].GetText(),
                (SimpleExprContext)context.children[0],
                (SimpleExprContext)context.children[1],
                VisitSimpleExpr
            );

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiUnaryExpressionNode VisitSimpleExprUnary(SimpleExprUnaryContext context)
        {
            var node = TreeHelper.CreateUnary(
                context.op.Text,
                VisitSimpleExpr(context.simpleExpr())
            );

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiUnaryExpressionNode VisitSimpleExprNot(SimpleExprNotContext context)
        {
            var node = TreeHelper.CreateUnary(
                context.not2Rule().GetText(),
                VisitSimpleExpr(context.simpleExpr())
            );

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitSimpleExprList(SimpleExprListContext context)
        {
            IEnumerable<QsiExpressionNode> parameters = VisitExprList(context.exprList());
            QsiExpressionNode node;

            if (context.HasToken(ROW_SYMBOL))
            {
                node = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                {
                    n.Member.SetValue(TreeHelper.CreateFunction(MySqlKnownFunction.Row));
                    n.Parameters.AddRange(parameters);
                });
            }
            else
            {
                node = TreeHelper.Create<QsiMultipleExpressionNode>(n =>
                {
                    n.Elements.AddRange(parameters);
                });
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitSimpleExprSubQuery(SimpleExprSubQueryContext context)
        {
            var node = VisitSubquery(context.subquery());

            if (!context.HasToken(EXISTS_SYMBOL))
                return node;

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.EXISTS_SYMBOL().GetText()));
                n.Parameters.Add(node);

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitSimpleExprMatch(SimpleExprMatchContext context)
        {
            // fulltextOptions ignored
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.MATCH_SYMBOL().GetText()));

                n.Parameters.Add(TreeHelper.Create<QsiMultipleExpressionNode>(mn =>
                {
                    mn.Elements.AddRange(VisitIdentListArg(context.identListArg()));

                    MySqlTree.PutContextSpan(mn, context.identListArg());
                }));

                n.Parameters.Add(VisitBitExpr(context.bitExpr()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiUnaryExpressionNode VisitSimpleExprBinary(SimpleExprBinaryContext context)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = context.BINARY_SYMBOL().GetText();
                n.Expression.SetValue(VisitSimpleExpr(context.simpleExpr()));
            });
        }

        public static QsiInvokeExpressionNode VisitSimpleExprCast(SimpleExprCastContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(MySqlKnownFunction.Cast));

                n.Parameters.Add(VisitExpr(context.expr()));
                n.Parameters.Add(VisitCastType(context.castType()));

                // arrayCast ignored

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiSwitchExpressionNode VisitSimpleExprCase(SimpleExprCaseContext context)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                if (context.expr() != null)
                    n.Value.SetValue(VisitExpr(context.expr()));

                for (int i = 0; i < context.whenExpression().Length; i++)
                {
                    var whenExpression = context.whenExpression(i);
                    var thenExpression = context.thenExpression(i);

                    n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(cn =>
                    {
                        cn.Condition.SetValue(VisitWhenExpression(whenExpression));
                        cn.Consequent.SetValue(VisitThenExpression(thenExpression));

                        MySqlTree.PutContextSpan(cn, whenExpression.Start, thenExpression.Stop);
                    }));
                }

                if (context.elseExpression() != null)
                {
                    n.Cases.Add(TreeHelper.Create<QsiSwitchCaseExpressionNode>(cn =>
                    {
                        cn.Consequent.SetValue(VisitElseExpression(context.elseExpression()));

                        MySqlTree.PutContextSpan(cn, context.elseExpression());
                    }));
                }

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitSimpleExprConvert(SimpleExprConvertContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.CONVERT_SYMBOL().GetText()));

                n.Parameters.Add(VisitExpr(context.expr()));
                n.Parameters.Add(VisitCastType(context.castType()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitSimpleExprConvertUsing(SimpleExprConvertUsingContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.CONVERT_SYMBOL().GetText()));

                n.Parameters.Add(VisitExpr(context.expr()));
                n.Parameters.Add(VisitCharsetName(context.charsetName()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitSimpleExprDefault(SimpleExprDefaultContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.DEFAULT_SYMBOL().GetText()));

                n.Parameters.Add(VisitSimpleIdentifier(context.simpleIdentifier()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitSimpleExprValues(SimpleExprValuesContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.VALUES_SYMBOL().GetText()));

                n.Parameters.Add(VisitSimpleIdentifier(context.simpleIdentifier()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitSimpleExprInterval(SimpleExprIntervalContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.INTERVAL_SYMBOL().GetText()));

                n.Parameters.Add(VisitExpr(context.expr(0)));
                n.Parameters.Add(VisitInterval(context.interval()));
                n.Parameters.Add(VisitExpr(context.expr(1)));

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Runtime Function Call
        public static QsiInvokeExpressionNode VisitRuntimeFunctionCall(RuntimeFunctionCallContext context)
        {
            var member = TreeHelper.CreateFunction(context.name?.Text ?? string.Empty);

            switch (context.name?.Type)
            {
                case CHAR_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);
                        n.Parameters.AddRange(VisitExprList(context.exprList()));

                        // charsetName ignored

                        MySqlTree.PutContextSpan(n, context);
                    });

                // FUNC() or FUNC
                case CURRENT_USER_SYMBOL:
                case USER_SYMBOL:
                case CURDATE_SYMBOL:
                case UTC_DATE_SYMBOL:
                case DATABASE_SYMBOL:
                case ROW_COUNT_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        MySqlTree.PutContextSpan(n, context);
                    });

                // FUNC(n)
                case DATE_SYMBOL:
                case DAY_SYMBOL:
                case HOUR_SYMBOL:
                case MINUTE_SYMBOL:
                case MONTH_SYMBOL:
                case SECOND_SYMBOL:
                case TIME_SYMBOL:
                case VALUES_SYMBOL:
                case YEAR_SYMBOL:
                case ASCII_SYMBOL:
                case CHARSET_SYMBOL:
                case COLLATION_SYMBOL:
                case MICROSECOND_SYMBOL:
                case PASSWORD_SYMBOL:
                case QUARTER_SYMBOL:
                case REVERSE_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);
                        n.Parameters.Add(VisitExprWithParentheses(context.exprWithParentheses()));

                        MySqlTree.PutContextSpan(n, context);
                    });

                // FUNC(n..)
                case INSERT_SYMBOL:
                case INTERVAL_SYMBOL:
                case LEFT_SYMBOL:
                case RIGHT_SYMBOL:
                case TIMESTAMP_SYMBOL:
                case IF_SYMBOL:
                case FORMAT_SYMBOL:
                case MOD_SYMBOL:
                case REPEAT_SYMBOL:
                case REPLACE_SYMBOL:
                case TRUNCATE_SYMBOL:
                case WEEK_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.AddRange(context.expr().Select(VisitExpr));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case ADDDATE_SYMBOL:
                case SUBDATE_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.expr(0)));

                        if (context.INTERVAL_SYMBOL() != null)
                        {
                            n.Parameters.Add(VisitExpr(context.expr(1)));
                        }
                        else
                        {
                            n.Parameters.Add(BuildInterval(
                                context.INTERVAL_SYMBOL().Symbol,
                                context.expr(1),
                                context.interval())
                            );
                        }

                        MySqlTree.PutContextSpan(n, context);
                    });

                // FUNC(fractionalPrecision)
                case CURTIME_SYMBOL:
                case NOW_SYMBOL:
                case SYSDATE_SYMBOL:
                case UTC_TIME_SYMBOL:
                case UTC_TIMESTAMP_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);
                        n.Parameters.AddRange(VisitTimeFunctionParameters(context.timeFunctionParameters()));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case DATE_ADD_SYMBOL:
                case DATE_SUB_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.expr(0)));

                        n.Parameters.Add(BuildInterval(
                            context.INTERVAL_SYMBOL().Symbol,
                            context.expr(1),
                            context.interval())
                        );

                        MySqlTree.PutContextSpan(n, context);
                    });

                // EXTRACT(<interval> FROM <expr>)
                case EXTRACT_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitInterval(context.interval()));
                        n.Parameters.Add(VisitExpr(context.expr(0)));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case GET_FORMAT_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitDateTimeTtype(context.dateTimeTtype()));
                        n.Parameters.Add(VisitExpr(context.expr(0)));

                        MySqlTree.PutContextSpan(n, context);
                    });

                // POSITION(<bitExpr> IN <expr>)
                case POSITION_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitBitExpr(context.bitExpr()));
                        n.Parameters.Add(VisitExpr(context.expr(0)));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case COALESCE_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.AddRange(VisitExprListWithParentheses(context.exprListWithParentheses()));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case OLD_PASSWORD_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitTextLiteral(context.textLiteral()));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case WEIGHT_STRING_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.expr(0)));

                        // <expr> AS CHAR <wsNumCodePoints> <weightStringLevels>
                        if (context.HasToken(CHAR_SYMBOL))
                        {
                            n.Parameters.Add(VisitRealUlongNumber(context.wsNumCodepoints().real_ulong_number()));

                            if (context.weightStringLevels() != null)
                            {
                                n.Parameters.AddRange(VisitWeightStringLevels(context.weightStringLevels()));
                            }
                        }
                        // <expr> AS BINARY <wsNumCodepoints>
                        else if (context.HasToken(BINARY_SYMBOL))
                        {
                            n.Parameters.Add(VisitRealUlongNumber(context.wsNumCodepoints().real_ulong_number()));
                        }
                        // <expr>, <ulong>, <ulong>, <ulong>
                        else
                        {
                            n.Parameters.AddRange(context.ulong_number().Select(VisitUlongNumber));
                        }

                        MySqlTree.PutContextSpan(n, context);
                    });

                default:
                {
                    if (context.trimFunction() != null)
                        return VisitTrimFunction(context.trimFunction());

                    if (context.substringFunction() != null)
                        return VisitSubstringFunction(context.substringFunction());

                    if (context.geometryFunction() != null)
                        return VisitGeometryFunction(context.geometryFunction());

                    break;
                }
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitTrimFunction(TrimFunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(MySqlKnownFunction.Trim));

                if (context.HasToken(LEADING_SYMBOL))
                {
                    n.Parameters.Add(TreeHelper.CreateLiteral("LEADING"));
                }
                else if (context.HasToken(TRAILING_SYMBOL))
                {
                    n.Parameters.Add(TreeHelper.CreateLiteral("TRAILING"));
                }
                else if (context.HasToken(BOTH_SYMBOL))
                {
                    n.Parameters.Add(TreeHelper.CreateLiteral("BOTH"));
                }

                n.Parameters.AddRange(context.expr().Select(VisitExpr));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitSubstringFunction(SubstringFunctionContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(MySqlKnownFunction.Substring));

                n.Parameters.AddRange(context.expr().Select(VisitExpr));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiInvokeExpressionNode VisitGeometryFunction(GeometryFunctionContext context)
        {
            var member = TreeHelper.CreateFunction(context.name?.Text ?? string.Empty);

            switch (context.name?.Type)
            {
                case CONTAINS_SYMBOL:
                case POINT_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.AddRange(context.expr().Select(VisitExpr));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case GEOMETRYCOLLECTION_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.AddRange(VisitExprList(context.exprList()));

                        MySqlTree.PutContextSpan(n, context);
                    });

                case LINESTRING_SYMBOL:
                case MULTILINESTRING_SYMBOL:
                case MULTIPOINT_SYMBOL:
                case MULTIPOLYGON_SYMBOL:
                case POLYGON_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.AddRange(VisitExprListWithParentheses(context.exprListWithParentheses()));

                        MySqlTree.PutContextSpan(n, context);
                    });
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static IEnumerable<QsiExpressionNode> VisitWeightStringLevels(WeightStringLevelsContext context)
        {
            if (context.HasToken(MINUS_OPERATOR))
            {
                foreach (var realUlongNumber in context.real_ulong_number())
                    yield return VisitRealUlongNumber(realUlongNumber);
            }
            else
            {
                foreach (var weightStringLevelListItem in context.weightStringLevelListItem())
                    yield return VisitWeightStringLevelListItem(weightStringLevelListItem);
            }
        }

        public static QsiOrderExpressionNode VisitWeightStringLevelListItem(WeightStringLevelListItemContext context)
        {
            // reverse ignore
            return TreeHelper.Create<QsiOrderExpressionNode>(n =>
            {
                n.Expression.SetValue(VisitRealUlongNumber(context.real_ulong_number()));
                n.Order = context.DESC_SYMBOL() != null ? QsiSortOrder.Descending : QsiSortOrder.Ascending;

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Function Call
        public static QsiInvokeExpressionNode VisitFunctionCall(FunctionCallContext context)
        {
            QsiQualifiedIdentifier identifier;
            IEnumerable<QsiExpressionNode> parameters = Enumerable.Empty<QsiExpressionNode>();

            // <pureIdentifier> (<udfExprList>)
            if (context.pureIdentifier() != null)
            {
                identifier = new QsiQualifiedIdentifier(IdentifierVisitor.VisitPureIdentifier(context.pureIdentifier()));

                if (context.udfExprList() != null)
                    parameters = VisitUdfExprList(context.udfExprList());
            }
            // <qualifiedIdentifier> (<exprList>)
            else
            {
                identifier = IdentifierVisitor.VisitQualifiedIdentifier(context.qualifiedIdentifier());

                if (context.exprList() != null)
                    parameters = VisitExprList(context.exprList());
            }

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(new QsiFunctionExpressionNode
                {
                    Identifier = identifier
                });

                n.Parameters.AddRange(parameters);
            });
        }
        #endregion

        #region User/System Variable
        public static QsiVariableExpressionNode VisitVariable(VariableContext context)
        {
            if (context.userVariable() != null)
                return VisitUserVariable(context.userVariable());

            return VisitSystemVariable(context.systemVariable());
        }

        public static QsiVariableExpressionNode VisitUserVariable(UserVariableContext context)
        {
            QsiVariableExpressionNode node;

            if (context.HasToken(AT_TEXT_SUFFIX))
            {
                // .SIMPLE_IDENTIFIER
                string variableName = context.AT_TEXT_SUFFIX().GetText()[1..];

                node = new QsiVariableExpressionNode
                {
                    Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(variableName, false))
                };
            }
            else
            {
                node = new QsiVariableExpressionNode
                {
                    Identifier = new QsiQualifiedIdentifier(VisitTextOrIdentifier(context.textOrIdentifier()))
                };
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiVariableExpressionNode VisitSystemVariable(SystemVariableContext context)
        {
            // context.varIdentType() ignored

            QsiQualifiedIdentifier identifier;

            if (context.dotIdentifier() != null)
            {
                identifier = new QsiQualifiedIdentifier(
                    VisitTextOrIdentifier(context.textOrIdentifier()),
                    IdentifierVisitor.VisitDotIdentifier(context.dotIdentifier())
                );
            }
            else
            {
                identifier = new QsiQualifiedIdentifier(VisitTextOrIdentifier(context.textOrIdentifier()));
            }

            var node = new QsiVariableExpressionNode
            {
                Identifier = identifier
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        #region Interval
        public static QsiInvokeExpressionNode BuildInterval(IToken intervalSymbol, ExprContext exprContext, IntervalContext intervalContext)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(MySqlKnownFunction.Interval));
                n.Parameters.Add(VisitExpr(exprContext));
                n.Parameters.Add(VisitInterval(intervalContext));

                MySqlTree.PutContextSpan(n, intervalSymbol, intervalContext.Stop);
            });
        }

        public static QsiTypeExpressionNode VisitInterval(IntervalContext context)
        {
            var node = new QsiTypeExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        #region SumExpr (Windowed Aggregate Function)
        public static QsiInvokeExpressionNode VisitSumExpr(SumExprContext context)
        {
            var member = TreeHelper.CreateFunction(context.name?.Text ?? string.Empty);

            switch (context.name?.Type)
            {
                // AVG(<DISTINCT>? <ALL>? <expr>) <windowingClause>
                case AVG_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.inSumExpr().expr()));

                        MySqlTree.PutContextSpan(n, context);
                        // windowingClause ignored
                    });

                // BIT_AND(<ALL>? <expr>) <windowingClause>
                // BIT_OR (<ALL>? <expr>) <windowingClause>
                // BIT_XOR(<ALL>? <expr>) <windowingClause>
                case BIT_AND_SYMBOL:
                case BIT_OR_SYMBOL:
                case BIT_XOR_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.inSumExpr().expr()));

                        MySqlTree.PutContextSpan(n, context);
                        // windowingClause ignored
                    });

                // COUNT
                case COUNT_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        // COUNT(<ALL>? *)
                        if (context.HasToken(MULT_OPERATOR))
                        {
                            n.Parameters.Add(TreeHelper.Create<QsiColumnExpressionNode>(cn =>
                            {
                                cn.Column.SetValue(new QsiAllColumnNode());
                            }));
                        }
                        // COUNT(<ALL>? <expr>)
                        else if (context.inSumExpr() != null)
                        {
                            n.Parameters.Add(VisitExpr(context.inSumExpr().expr()));
                        }
                        // COUNT(<DISTINCT> <expr>,..)
                        else if (context.HasToken(DISTINCT_SYMBOL))
                        {
                            n.Parameters.AddRange(VisitExprList(context.exprList()));
                        }

                        MySqlTree.PutContextSpan(n, context);
                        // windowingClause ignored
                    });

                case MIN_SYMBOL:
                case MAX_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.inSumExpr().expr()));

                        MySqlTree.PutContextSpan(n, context);
                        // windowingClause ignored
                    });

                // STD        (<ALL>? <expr>)
                // VARIANCE   (<ALL>? <expr>)
                // STDDEV_SAMP(<ALL>? <expr>)
                // VAR_SAMP   (<ALL>? <expr>)
                case STD_SYMBOL:
                case VARIANCE_SYMBOL:
                case STDDEV_SAMP_SYMBOL:
                case VAR_SAMP_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.inSumExpr().expr()));

                        MySqlTree.PutContextSpan(n, context);
                        // windowingClause ignored
                    });

                // SUM(<DISTNICT>? <ALL>? <expr>)
                case SUM_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.Add(VisitExpr(context.inSumExpr().expr()));

                        MySqlTree.PutContextSpan(n, context);
                        // windowingClause ignored
                    });

                // GROUP_CONCAT(<DISTINCT>? <expr>,.. <order>? (SEPARATOR <textString>)?
                case GROUP_CONCAT_SYMBOL:
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(member);

                        n.Parameters.AddRange(VisitExprList(context.exprList()));

                        if (context.HasToken(SEPARATOR_SYMBOL))
                            n.Parameters.Add(VisitTextString(context.textString()));

                        MySqlTree.PutContextSpan(n, context);
                        // windowingClause ignored
                    });

                default:
                {
                    if (context.jsonFunction() != null)
                        return VisitJsonFunction(context.jsonFunction());

                    break;
                }
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonFunction(JsonFunctionContext context)
        {
            var node = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.Start.Text));
            });

            if (context.HasToken(JSON_ARRAYAGG_SYMBOL))
            {
                node.Parameters.Add(VisitExpr(context.inSumExpr(0).expr()));
            }
            else if (context.HasToken(JSON_OBJECTAGG_SYMBOL))
            {
                node.Parameters.AddRange(context.inSumExpr().Select(x => x.expr()).Select(VisitExpr));
            }
            else
            {
                throw TreeHelper.NotSupportedTree(context);
            }

            MySqlTree.PutContextSpan(node, context);
            // windowingClause ignored

            return node;
        }
        #endregion

        #region Case
        public static QsiExpressionNode VisitWhenExpression(WhenExpressionContext context)
        {
            return VisitExpr(context.expr());
        }

        public static QsiExpressionNode VisitThenExpression(ThenExpressionContext context)
        {
            return VisitExpr(context.expr());
        }

        public static QsiExpressionNode VisitElseExpression(ElseExpressionContext context)
        {
            return VisitExpr(context.expr());
        }
        #endregion

        #region Window Function Call
        public static QsiInvokeExpressionNode VisitWindowFunctionCall(WindowFunctionCallContext context)
        {
            var member = TreeHelper.CreateFunction(context.Start.Text);

            var node = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(member);
            });

            switch (context.Start.Type)
            {
                case ROW_NUMBER_SYMBOL:
                case RANK_SYMBOL:
                case DENSE_RANK_SYMBOL:
                case CUME_DIST_SYMBOL:
                case PERCENT_RANK_SYMBOL:

                    break;

                case NTILE_SYMBOL:
                    node.Parameters.Add(VisitSimpleExprWithParentheses(context.simpleExprWithParentheses()));
                    break;

                case LEAD_SYMBOL:
                case LAG_SYMBOL:
                    node.Parameters.Add(VisitExpr(context.expr()));
                    break;

                case FIRST_VALUE_SYMBOL:
                case LAST_VALUE_SYMBOL:
                    node.Parameters.Add(VisitExprWithParentheses(context.exprWithParentheses()));
                    break;

                case NTH_VALUE_SYMBOL:
                    node.Parameters.Add(VisitExpr(context.expr()));
                    node.Parameters.Add(VisitSimpleExpr(context.simpleExpr()));
                    break;
            }

            MySqlTree.PutContextSpan(node, context);
            // windowingClause ignored

            return node;
        }
        #endregion

        public static QsiInvokeExpressionNode VisitGroupingOperation(GroupingOperationContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(context.Start.Text));

                n.Parameters.AddRange(VisitExprList(context.exprList()));
            });
        }

        public static QsiTypeExpressionNode VisitDateTimeTtype(DateTimeTtypeContext context)
        {
            var node = new QsiTypeExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static IEnumerable<QsiLiteralExpressionNode> VisitTimeFunctionParameters(TimeFunctionParametersContext context)
        {
            if (context?.fractionalPrecision() != null)
                yield return VisitFractionalPrecision(context.fractionalPrecision());
        }

        public static QsiLiteralExpressionNode VisitFractionalPrecision(FractionalPrecisionContext context)
        {
            return VisitLiteralFromToken(context.INT_NUMBER().Symbol);
        }

        #region Having Clause
        public static QsiExpressionNode VisitHavingClause(HavingClauseContext context)
        {
            return VisitExpr(context.expr());
        }
        #endregion

        #region Order Clause
        public static QsiMultipleOrderExpressionNode VisitOrderClause(OrderClauseContext context)
        {
            return TreeHelper.Create<QsiMultipleOrderExpressionNode>(n =>
            {
                n.Orders.AddRange(VisitOrderList(context.orderList()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static IEnumerable<QsiOrderExpressionNode> VisitOrderList(OrderListContext context)
        {
            return context.orderExpression().Select(VisitOrderExpression);
        }

        public static QsiOrderExpressionNode VisitOrderExpression(OrderExpressionContext context)
        {
            return TreeHelper.Create<QsiOrderExpressionNode>(n =>
            {
                n.Expression.SetValue(VisitExpr(context.expr()));
                n.Order = VisitDirection(context.direction());

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiSortOrder VisitDirection(DirectionContext context)
        {
            if (context == null || context.ASC_SYMBOL() != null)
                return QsiSortOrder.Ascending;

            return QsiSortOrder.Descending;
        }
        #endregion

        #region Limit Clause
        public static QsiLimitExpressionNode VisitLimitClause(LimitClauseContext context)
        {
            return TreeHelper.Create<QsiLimitExpressionNode>(n =>
            {
                var limitOptions = context.limitOptions();

                n.Limit.SetValue(VisitLimitOption(limitOptions.limitOption(0)));

                if (limitOptions.limitOption(1) != null)
                    n.Offset.SetValue(VisitLimitOption(limitOptions.limitOption(1)));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiLimitExpressionNode VisitSimpleLimitClause(SimpleLimitClauseContext context)
        {
            return TreeHelper.Create<QsiLimitExpressionNode>(n =>
            {
                n.Limit.SetValue(VisitLimitOption(context.limitOption()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitLimitOption(LimitOptionContext context)
        {
            switch (context.children[0])
            {
                case IdentifierContext identifier:
                    var node = new QsiFieldExpressionNode
                    {
                        Identifier = new QsiQualifiedIdentifier(IdentifierVisitor.VisitIdentifier(identifier))
                    };

                    MySqlTree.PutContextSpan(node, context);

                    return node;

                case ITerminalNode terminalNode:
                    if (terminalNode is { Symbol: { Type: PARAM_MARKER } })
                    {
                        var contextWrapper = new ParserRuleContextWrapper<ITerminalNode>(
                            terminalNode,
                            terminalNode.Symbol,
                            terminalNode.Symbol
                        );

                        return VisitParamMarker(contextWrapper);
                    }

                    return VisitLiteralFromToken(terminalNode.Symbol);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }
        #endregion

        #region Where Clause
        public static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
        {
            return TreeHelper.Create<QsiWhereExpressionNode>(n =>
            {
                n.Expression.SetValue(VisitExpr(context.expr()));

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Group By Clause
        public static QsiGroupingExpressionNode VisitGroupByClause(GroupByClauseContext context)
        {
            return TreeHelper.Create<QsiGroupingExpressionNode>(n =>
            {
                n.Items.AddRange(VisitOrderList(context.orderList()));

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        public static QsiTableExpressionNode VisitSubquery(SubqueryContext context)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitSubquery(context));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiAliasNode VisitSelectAlias(SelectAliasContext context)
        {
            var node = new QsiAliasNode
            {
                Name = context.identifier() != null
                    ? IdentifierVisitor.VisitIdentifier(context.identifier())
                    : VisitTextStringLiteralAsIdentifier(context.textStringLiteral())
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiTypeExpressionNode VisitCastType(CastTypeContext context)
        {
            string typeName = context.HasToken(INT_SYMBOL)
                ? $"{context.Start.Text} {context.INT_SYMBOL().GetText()}"
                : context.Start.Text;

            var node = new QsiTypeExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(typeName, false))
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiLiteralExpressionNode VisitCharsetName(CharsetNameContext context)
        {
            return TreeHelper.CreateLiteral(context.GetText());
        }

        #region Column
        public static IEnumerable<QsiColumnExpressionNode> VisitIdentListArg(IdentListArgContext context)
        {
            return context.identList().simpleIdentifier().Select(VisitSimpleIdentifier);
        }

        public static QsiColumnExpressionNode VisitSimpleIdentifier(SimpleIdentifierContext context)
        {
            var identifier = IdentifierVisitor.VisitSimpleIdentifier(context);

            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(TreeHelper.Create<QsiDeclaredColumnNode>(cn =>
                {
                    cn.Name = identifier;

                    MySqlTree.PutContextSpan(cn, context);
                }));

                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Literal
        public static QsiExpressionNode VisitLiteral(LiteralContext context)
        {
            switch (context.children[0])
            {
                case TextLiteralContext textLiteral:
                    return VisitTextLiteral(textLiteral);

                case NumLiteralContext numLiteral:
                    return VisitNumLiteral(numLiteral);

                case TemporalLiteralContext temporalLiteral:
                    return VisitTemporalLiteral(temporalLiteral);

                case NullLiteralContext nullLiteral:
                    return VisitNullLiteral(nullLiteral);

                case BoolLiteralContext boolLiteral:
                    return VisitBoolLiteral(boolLiteral);

                default:
                {
                    var charSet = context.UNDERSCORE_CHARSET()?.GetText();
                    object value;
                    QsiDataType type;

                    switch (context.children[^1])
                    {
                        // BIN_NUMBER
                        case ITerminalNode { Symbol: { Type: HEX_NUMBER } } hexNumber:
                        {
                            string text = hexNumber.GetText();

                            if (text[0] == '0')
                            {
                                // 0x00FF00
                                value = new MySqlString(MySqlStringKind.Hexa, text[2..], charSet, null);
                                type = QsiDataType.Custom;
                            }
                            else
                            {
                                // X'00FF00'
                                value = new MySqlString(MySqlStringKind.HexaString, text[2..^1], charSet, null);
                                type = QsiDataType.Custom;
                            }

                            break;
                        }

                        case ITerminalNode { Symbol: { Type: BIN_NUMBER } } binNumber:
                        {
                            string text = binNumber.GetText();

                            if (text[0] == '0')
                            {
                                // 0b0100
                                value = new MySqlString(MySqlStringKind.Bit, text[2..], null, null);
                                type = QsiDataType.Custom;
                            }
                            else
                            {
                                // b'0100'
                                value = new MySqlString(MySqlStringKind.BitString, text[2..^1], null, null);
                                type = QsiDataType.Custom;
                            }

                            break;
                        }

                        default:
                            throw TreeHelper.NotSupportedTree(context.children[^1]);
                    }

                    var node = new QsiLiteralExpressionNode
                    {
                        Value = value,
                        Type = type
                    };

                    MySqlTree.PutContextSpan(node, context);

                    return node;
                }
            }
        }

        public static QsiLiteralExpressionNode VisitUlongNumber(Ulong_numberContext context)
        {
            return VisitLiteralFromToken(context.Start);
        }

        public static QsiLiteralExpressionNode VisitRealUlongNumber(Real_ulong_numberContext context)
        {
            return VisitLiteralFromToken(context.Start);
        }

        public static QsiLiteralExpressionNode VisitTextLiteral(TextLiteralContext context)
        {
            QsiLiteralExpressionNode node;

            IEnumerable<string> textStringLiterals = context.textStringLiteral()
                .Select(l => IdentifierUtility.Unescape(l.value.Text));

            var rawValue = string.Join(string.Empty, textStringLiterals);

            // National String
            if (context.TryGetToken(NCHAR_TEXT, out var ncharText))
            {
                // N'text'
                var strValue = $"{IdentifierUtility.Unescape(ncharText.Text[1..])}{rawValue}";

                node = new QsiLiteralExpressionNode
                {
                    Value = new MySqlString(MySqlStringKind.National, strValue, null, null),
                    Type = QsiDataType.Custom
                };
            }
            // CharSet String
            else if (context.TryGetToken(UNDERSCORE_CHARSET, out var charSet))
            {
                // _utf8'text'
                node = new QsiLiteralExpressionNode
                {
                    Value = new MySqlString(MySqlStringKind.Default, rawValue, charSet.Text, null),
                    Type = QsiDataType.Custom
                };
            }
            // Default String
            else
            {
                // 'text'
                node = new QsiLiteralExpressionNode
                {
                    Value = rawValue,
                    Type = QsiDataType.String
                };
            }

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiLiteralExpressionNode VisitTextStringLiteral(TextStringLiteralContext context)
        {
            var node = new QsiLiteralExpressionNode
            {
                Value = IdentifierUtility.Unescape(context.value.Text),
                Type = QsiDataType.String
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiLiteralExpressionNode VisitTextString(TextStringContext context)
        {
            switch (context.children[0])
            {
                case TextStringLiteralContext textStringLiteral:
                    return VisitTextStringLiteral(textStringLiteral);

                case ITerminalNode terminalNode:
                    return VisitLiteralFromToken(terminalNode.Symbol);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiLiteralExpressionNode VisitNumLiteral(NumLiteralContext context)
        {
            return VisitLiteralFromToken(context.Start);
        }

        public static QsiInvokeExpressionNode VisitTemporalLiteral(TemporalLiteralContext context)
        {
            string functionName = context.children[0].GetText().ToUpper();
            var singleQuotedText = (ITerminalNode)context.children[1];

            switch (functionName)
            {
                case MySqlKnownFunction.Date:
                case MySqlKnownFunction.Time:
                case MySqlKnownFunction.Timestamp:
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            var node = new QsiInvokeExpressionNode();
            node.Member.SetValue(TreeHelper.CreateFunction(functionName));
            node.Parameters.Add(VisitLiteralFromToken(singleQuotedText.Symbol));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiLiteralExpressionNode VisitNullLiteral(NullLiteralContext context)
        {
            return VisitLiteralFromToken(context.Start);
        }

        public static QsiLiteralExpressionNode VisitBoolLiteral(BoolLiteralContext context)
        {
            return VisitLiteralFromToken(context.Start);
        }

        public static QsiLiteralExpressionNode VisitLiteralFromToken(IToken token)
        {
            object value;
            QsiDataType type;

            switch (token.Type)
            {
                case INT_NUMBER:
                {
                    value = int.Parse(token.Text);
                    type = QsiDataType.Numeric;
                    break;
                }

                case DECIMAL_NUMBER:
                {
                    if (decimal.TryParse(token.Text, out var decimalValue))
                        value = decimalValue;
                    else
                        value = BigInteger.Parse(token.Text);

                    type = QsiDataType.Decimal;

                    break;
                }

                case FLOAT_NUMBER:
                {
                    value = float.Parse(token.Text);
                    type = QsiDataType.Decimal;
                    break;
                }

                case LONG_NUMBER:
                {
                    value = long.Parse(token.Text);
                    type = QsiDataType.Decimal;
                    break;
                }

                case ULONGLONG_NUMBER:
                {
                    value = ulong.Parse(token.Text);
                    type = QsiDataType.Decimal;
                    break;
                }

                case HEX_NUMBER:
                {
                    var literalText = token.Text;

                    if (literalText[0] == '0')
                    {
                        // 0x00FF00
                        value = new MySqlString(MySqlStringKind.Hexa, literalText[2..], null, null);
                        type = QsiDataType.Custom;
                    }
                    else
                    {
                        // X'00FF00'
                        value = new MySqlString(MySqlStringKind.HexaString, literalText[2..^1], null, null);
                        type = QsiDataType.Custom;
                    }

                    break;
                }

                case TRUE_SYMBOL:
                {
                    value = true;
                    type = QsiDataType.Boolean;
                    break;
                }

                case FALSE_SYMBOL:
                {
                    value = false;
                    type = QsiDataType.Boolean;
                    break;
                }

                case UNKNOWN_SYMBOL:
                {
                    value = token.Text;
                    type = QsiDataType.Constant;
                    break;
                }

                case NULL_SYMBOL:
                case NULL2_SYMBOL:
                {
                    value = null;
                    type = QsiDataType.Null;
                    break;
                }

                case SINGLE_QUOTED_TEXT:
                {
                    value = IdentifierUtility.Unescape(token.Text);
                    type = QsiDataType.String;
                    break;
                }

                default:
                    throw TreeHelper.NotSupportedTree(token);
            }

            var node = new QsiLiteralExpressionNode
            {
                Value = value,
                Type = type
            };

            MySqlTree.PutContextSpan(node, token);

            return node;
        }
        #endregion

        #region Identifier
        public static QsiIdentifier VisitTextOrIdentifier(TextOrIdentifierContext context)
        {
            if (context.identifier() != null)
                return IdentifierVisitor.VisitIdentifier(context.identifier());

            return VisitTextStringLiteralAsIdentifier(context.textStringLiteral());
        }

        public static QsiIdentifier VisitTextStringLiteralAsIdentifier(TextStringLiteralContext context)
        {
            return new(context.GetText(), true);
        }
        #endregion
    }
}
