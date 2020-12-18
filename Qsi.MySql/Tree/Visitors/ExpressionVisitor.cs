using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
        {
            return TreeHelper.Create<QsiWhereExpressionNode>(n =>
            {
                n.Expression.SetValue(VisitExpr(context.expr()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiMultipleOrderExpressionNode VisitOrderClause(OrderClauseContext context)
        {
            return TreeHelper.Create<QsiMultipleOrderExpressionNode>(n =>
            {
                // TODO: Impl
            });
        }

        public static QsiLimitExpressionNode VisitLimitClause(LimitClauseContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiGroupingExpressionNode VisitGroupByClause(GroupByClauseContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitHavingClause(HavingClauseContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitExpr(ExprContext context)
        {
            switch (context)
            {
                case ExprIsContext exprIs:
                    return VisitExprIs(exprIs);

                case ExprNotContext exprNot:
                    break;

                case ExprAndContext exprAnd:
                    break;

                case ExprXorContext exprXor:
                    break;

                case ExprOrContext exprOr:
                    break;
            }

            throw TreeHelper.NotSupportedTree(context);
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

        public static QsiExpressionNode VisitExprIs(ExprIsContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitBoolPri(BoolPriContext context)
        {
            switch (context)
            {
                case PrimaryExprPredicateContext primaryExprPredicate:
                    return VisitPredicate(primaryExprPredicate.predicate());

                case PrimaryExprIsNullContext primaryExprIsNull:
                    break;

                case PrimaryExprCompareContext primaryExprCompare:
                    break;

                case PrimaryExprAllAnyContext primaryExprAllAny:
                    break;
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitPrimaryExprIsNull(PrimaryExprIsNullContext context)
        {
            return null;
        }

        public static QsiExpressionNode VisitPredicate(PredicateContext context)
        {
            return null;
        }

        public static QsiExpressionNode VisitBitExpr(BitExprContext context)
        {
            return null;
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
                    break;

                case SimpleExprCollateContext simpleExprCollate:
                    break;

                case SimpleExprLiteralContext simpleExprLiteral:
                    break;

                case SimpleExprParamMarkerContext simpleExprParamMarker:
                    break;

                case SimpleExprSumContext simpleExprSum:
                    break;

                case SimpleExprGroupingOperationContext simpleExprGroupingOperation:
                    break;

                case SimpleExprWindowingFunctionContext simpleExprWindowingFunction:
                    break;

                case SimpleExprConcatContext simpleExprConcat:
                    break;

                case SimpleExprUnaryContext simpleExprUnary:
                    break;

                case SimpleExprNotContext simpleExprNot:
                    break;

                case SimpleExprListContext simpleExprList:
                    break;

                case SimpleExprSubQueryContext simpleExprSubQuery:
                    break;

                case SimpleExprOdbcContext simpleExprOdbc:
                    break;

                case SimpleExprMatchContext simpleExprMatch:
                    break;

                case SimpleExprBinaryContext simpleExprBinary:
                    break;

                case SimpleExprCastContext simpleExprCast:
                    break;

                case SimpleExprCaseContext simpleExprCase:
                    break;

                case SimpleExprConvertContext simpleExprConvert:
                    break;

                case SimpleExprConvertUsingContext simpleExprConvertUsing:
                    break;

                case SimpleExprDefaultContext simpleExprDefault:
                    break;

                case SimpleExprValuesContext simpleExprValues:
                    break;

                case SimpleExprIntervalContext simpleExprInterval:
                    break;
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
            // TODO: Implement
            return null;
        }

        public static QsiInvokeExpressionNode VisitSimpleExprRuntimeFunction(SimpleExprRuntimeFunctionContext context)
        {
            return VisitRuntimeFunctionCall(context.runtimeFunctionCall());
        }
        #endregion

        #region Runtime Function
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

                        // TODO: charsetName ignored

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
                    });

                // trimFunction
                // substringFunction
            }

            return null;
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
            // TODO: context.varIdentType()

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

            return new QsiVariableExpressionNode
            {
                Identifier = identifier
            };
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
            return VisitLiteral(context.INT_NUMBER());
        }

        #region Literal
        public static QsiLiteralExpressionNode VisitLiteral(ParserRuleContext context)
        {
            switch (context)
            {
                case NumLiteralContext numLiteral:
                    return VisitLiteral(numLiteral.Start);

                case BoolLiteralContext boolLiteral:
                    return VisitLiteral(boolLiteral.Start);

                case NullLiteralContext nullLiteral:
                    return VisitLiteral(nullLiteral.Start);

                case TextStringLiteralContext textStringLiteral:
                    return new QsiLiteralExpressionNode
                    {
                        Value = textStringLiteral.value.Text[1..^2],
                        Type = QsiDataType.String
                    };

                case TextLiteralContext textLiteral:
                    return new QsiLiteralExpressionNode
                    {
                    };
            }

            return null;
        }

        public static QsiLiteralExpressionNode VisitLiteral(ITerminalNode terminalNode)
        {
            return VisitLiteral(terminalNode.Symbol);
        }

        public static QsiLiteralExpressionNode VisitLiteral(IToken token)
        {
            object value;
            QsiDataType type;

            switch (token.Type)
            {
                // LONG_NUMBER ULONGLONG_NUMBER has no declared body
                case INT_NUMBER:
                {
                    value = int.Parse(token.Text);
                    type = QsiDataType.Numeric;

                    break;
                }

                case DECIMAL_NUMBER:
                {
                    value = double.Parse(token.Text);
                    type = QsiDataType.Decimal;

                    break;
                }

                case FLOAT_NUMBER:
                {
                    value = float.Parse(token.Text);
                    type = QsiDataType.Decimal;

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

                case NULL_SYMBOL:
                case NULL2_SYMBOL:
                {
                    value = null;
                    type = QsiDataType.Null;

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

            return null;
        }
        #endregion

        #region Identifier
        public static QsiIdentifier VisitTextOrIdentifier(TextOrIdentifierContext context)
        {
            if (context.identifier() != null)
                return IdentifierVisitor.VisitIdentifier(context.identifier());

            return new QsiIdentifier(context.textStringLiteral().GetText(), true);
        }
        #endregion

        public static QsiLiteralExpressionNode VisitRealUlongNumber(Real_ulong_numberContext context)
        {
            throw new NotImplementedException();
        }
    }
}
