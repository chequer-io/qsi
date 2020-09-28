using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode Visit(IParseTree tree)
        {
            switch (tree)
            {
                case FunctionCallContext functionCallContext:
                    return VisitFunctionCall(functionCallContext);

                case ExpressionContext expressionContext:
                    return VisitExpression(expressionContext);

                default:
                    throw TreeHelper.NotSupportedTree(tree);
            }
        }

        #region Function
        internal static QsiExpressionNode VisitFunctionCall(FunctionCallContext context)
        {
            switch (context)
            {
                case SpecificFunctionCallContext specificFunctionCallContext:
                    return VisitSpecificFunctionCall(specificFunctionCallContext);

                case AggregateFunctionCallContext aggregateFunctionCallContext:
                    return VisitAggregateFunctionCall(aggregateFunctionCallContext);

                case ScalarFunctionCallContext scalarFunctionCallContext:
                    return VisitScalarFunctionCall(scalarFunctionCallContext);

                case UdfFunctionCallContext udfFunctionCallContext:
                    return VisitUdfFunctionCall(udfFunctionCallContext);

                case PasswordFunctionCallContext passwordFunctionCallContext:
                    return VisitPasswordFunctionClause(passwordFunctionCallContext.passwordFunctionClause());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiExpressionNode VisitSpecificFunctionCall(SpecificFunctionCallContext context)
        {
            var specificFunction = context.specificFunction();

            switch (specificFunction)
            {
                case SimpleFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                    });
                }

                case DataTypeFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        n.Parameters.Add(VisitExpression(fContext.expression()));
                    });
                }

                case ValuesFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        n.Parameters.Add(VisitFullColumnName(fContext.fullColumnName()));
                    });
                }

                case CaseFunctionCallContext fContext:
                {
                    var switchExp = new QsiSwitchExpressionNode();

                    // CASE <value>
                    if (fContext.expression() != null)
                    {
                        switchExp.Value.SetValue(VisitExpression(fContext.expression()));
                    }

                    // WHEN <condition> THEN <consequent> 
                    foreach (var caseFunc in fContext.caseFuncAlternative())
                    {
                        switchExp.Cases.Add(
                            TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
                            {
                                n.Condition.SetValue(VisitFunctionArg(caseFunc.condition));
                                n.Consequent.SetValue(VisitFunctionArg(caseFunc.consequent));
                            })
                        );
                    }

                    // ELSE <else>
                    if (fContext.elseArg != null)
                    {
                        switchExp.Cases.Add(
                            TreeHelper.Create<QsiSwitchCaseExpressionNode>(n =>
                            {
                                n.Consequent.SetValue(VisitFunctionArg(fContext.elseArg));
                            })
                        );
                    }

                    return switchExp;
                }

                case CharFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        n.Parameters.AddRange(VisitFunctionArgs(fContext.functionArgs()));
                    });
                }

                case PositionFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));

                        if (LiteralOrExpression(fContext.positionString, fContext.positionExpression, out var param1))
                            n.Parameters.Add(param1);

                        if (LiteralOrExpression(fContext.inString, fContext.inExpression, out var param2))
                            n.Parameters.Add(param2);
                    });
                }

                case SubstrFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));

                        if (LiteralOrExpression(fContext.sourceString, fContext.sourceExpression, out var param1))
                            n.Parameters.Add(param1);

                        if (LiteralOrExpression(fContext.fromDecimal, fContext.fromExpression, out var param2))
                            n.Parameters.Add(param2);

                        if (LiteralOrExpression(fContext.forDecimal, fContext.forExpression, out var param3))
                            n.Parameters.Add(param3);
                    });
                }

                case TrimFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));

                        if (LiteralOrExpression(fContext.sourceString, fContext.sourceExpression, out var param1))
                            n.Parameters.Add(param1);

                        if (LiteralOrExpression(fContext.fromString, fContext.fromExpression, out var param2))
                            n.Parameters.Add(param2);
                    });
                }

                case WeightFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));

                        if (LiteralOrExpression(fContext.stringLiteral(), fContext.expression(), out var param1))
                            n.Parameters.Add(param1);
                    });
                }

                case ExtractFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));

                        if (LiteralOrExpression(fContext.stringLiteral(), fContext.expression(), out var param1))
                            n.Parameters.Add(param1);
                    });
                }

                case GetFormatFunctionCallContext fContext:
                {
                    // GET_FORMAT  ({DATE | TIME | DATETIME},  string)
                    // ▔\MEMBER/▔   ▔▔▔▔▔▔▔▔\SKIP/▔▔▔▔▔▔▔▔▔    ▔\P1/▔

                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        // n.Parameters.Add(VisitLiteral(fContext.datetimeFormat));
                        n.Parameters.Add(VisitLiteral(fContext.stringLiteral()));
                    });
                }

                default:
                    throw TreeHelper.NotSupportedTree(specificFunction);
            }

            static bool LiteralOrExpression(ParserRuleContext literal, ExpressionContext expression, out QsiExpressionNode result)
            {
                if (literal != null && expression != null)
                    throw new InvalidOperationException();

                result = null;

                if (literal != null)
                    result = VisitLiteral(literal);

                if (expression != null)
                    result = VisitExpression(expression);

                return result != null;
            }
        }

        private static QsiExpressionNode VisitAggregateFunctionCall(AggregateFunctionCallContext context)
        {
            switch (context.aggregateWindowedFunction())
            {
                case TotalFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        n.Parameters.Add(VisitFunctionArg(fContext.functionArg()));
                    });
                }

                case CountFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));

                        if (fContext.starArg != null)
                        {
                            n.Parameters.Add(TreeHelper.Create<QsiColumnExpressionNode>(cn =>
                            {
                                cn.Column.SetValue(new QsiAllColumnNode());
                            }));
                        }
                        else
                        {
                            n.Parameters.Add(VisitFunctionArg(fContext.functionArg()));
                        }
                    });
                }

                case CountDistinctFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        n.Parameters.AddRange(VisitFunctionArgs(fContext.functionArgs()));
                    });
                }

                case LogicalFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        n.Parameters.Add(VisitFunctionArg(fContext.functionArg()));
                    });
                }

                case ConcatFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(fContext.children[0]));
                        n.Parameters.AddRange(VisitFunctionArgs(fContext.functionArgs()));

                        OrderByExpressionContext[] orderByExpressions = fContext.orderByExpression();

                        if (orderByExpressions?.Length > 0)
                        {
                            n.Parameters.AddRange(orderByExpressions.Select(VisitOrderByExpression));
                        }
                    });
                }

                default:
                    throw TreeHelper.NotSupportedTree(context.aggregateWindowedFunction());
            }
        }

        private static QsiExpressionNode VisitScalarFunctionCall(ScalarFunctionCallContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(VisitScalarFunctionName(context.scalarFunctionName()));
                n.Parameters.AddRange(VisitFunctionArgs(context.functionArgs()));
            });
        }

        private static QsiExpressionNode VisitUdfFunctionCall(UdfFunctionCallContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(VisitFunctionAccess(context.fullId()));
                n.Parameters.AddRange(VisitFunctionArgs(context.functionArgs()));
            });
        }

        private static QsiExpressionNode VisitPasswordFunctionClause(PasswordFunctionClauseContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(VisitFunctionAccess(context.children[0]));
                n.Parameters.Add(VisitFunctionArg(context.functionArg()));
            });
        }

        private static IEnumerable<QsiExpressionNode> VisitFunctionArgs(FunctionArgsContext context)
        {
            return
                context?.functionArg()?.Select(VisitFunctionArg) ??
                Enumerable.Empty<QsiExpressionNode>();
        }

        private static QsiExpressionNode VisitFunctionArg(FunctionArgContext context)
        {
            switch (context)
            {
                case ConstantArgContext argContext:
                    return VisitConstant(argContext.constant());

                case FullColumnNameArgContext argContext:
                    return VisitFullColumnName(argContext.fullColumnName());

                case FunctionCallArgContext argContext:
                    return VisitFunctionCall(argContext.functionCall());

                case ExpressionArgContext argContext:
                    return VisitExpression(argContext.expression());

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }
        #endregion

        #region Literal
        private static QsiExpressionNode VisitConstant(ConstantContext constant)
        {
            switch (constant)
            {
                case StringLiteralConstantContext literalContext:
                {
                    return VisitLiteral(literalContext.stringLiteral());
                }

                case PositiveDecimalLiteralConstantContext literalContext:
                {
                    return VisitLiteral(literalContext.decimalLiteral());
                }

                case NegativeDecimalLiteralConstantContext literalContext:
                {
                    return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                    {
                        n.Operator = literalContext.MINUS().GetText();
                        n.Expression.SetValue(VisitLiteral(literalContext.decimalLiteral()));
                    });
                }

                case HexadecimalLiteralConstantContext literalContext:
                {
                    return VisitLiteral(literalContext.hexadecimalLiteral());
                }

                case BooleanLiteralConstantContext literalContext:
                {
                    return VisitLiteral(literalContext.booleanLiteral());
                }

                case RealLiteralConstantContext literalContext:
                {
                    return new QsiLiteralExpressionNode
                    {
                        Value = literalContext.GetText(),
                        Type = QsiDataType.Decimal
                    };
                }

                case BitStringConstantContext literalContext:
                {
                    return new QsiLiteralExpressionNode
                    {
                        Value = literalContext.GetText(),
                        Type = QsiDataType.Binary
                    };
                }

                case NullLiteralConstantContext literalContext:
                {
                    return VisitNullNotNull(literalContext.nullNotnull());
                }

                default:
                    throw TreeHelper.NotSupportedTree(constant);
            }
        }

        private static QsiExpressionNode VisitNullNotNull(NullNotnullContext context)
        {
            var nullLiteral = VisitLiteral(context.nullLiteral());

            if (context.NOT() != null)
            {
                return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                {
                    n.Operator = context.NOT().GetText();
                    n.Expression.SetValue(nullLiteral);
                });
            }

            return nullLiteral;
        }

        private static QsiExpressionNode VisitCollationName(CollationNameContext context)
        {
            if (context.uid() != null)
            {
                return VisitTypeAccess(context.uid());
            }

            return new QsiLiteralExpressionNode
            {
                Value = IdentifierUtility.Unescape(context.STRING_LITERAL().GetText()),
                Type = QsiDataType.String
            };
        }

        public static QsiExpressionNode VisitLiteral(ParserRuleContext context)
        {
            QsiDataType literalType;
            object value;

            switch (context)
            {
                case NullLiteralContext _:
                    literalType = QsiDataType.Null;
                    value = null;
                    break;

                case FileSizeLiteralContext _:
                case StringLiteralContext _:
                    literalType = QsiDataType.String;
                    value = IdentifierUtility.Unescape(context.GetText());
                    break;

                case DecimalLiteralContext _:
                    literalType = QsiDataType.Decimal;
                    value = context.GetText();
                    break;

                case HexadecimalLiteralContext _:
                    literalType = QsiDataType.Hexadecimal;
                    value = context.GetText();
                    break;

                case BooleanLiteralContext _:
                    literalType = QsiDataType.Boolean;
                    value = context.GetText();
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            return new QsiLiteralExpressionNode
            {
                Value = value,
                Type = literalType
            };
        }
        #endregion

        #region Expression
        internal static QsiExpressionNode VisitExpression(ExpressionContext context)
        {
            switch (context)
            {
                case NotExpressionContext notExpressionContext:
                    return VisitNotExpression(notExpressionContext);

                case LogicalExpressionContext logicalExpressionContext:
                    return VisitLogicalExpression(logicalExpressionContext);

                case IsExpressionContext isExpressionContext:
                    return VisitIsExpression(isExpressionContext);

                case PredicateExpressionContext predicateExpressionContext:
                    return VisitPredicateExpression(predicateExpressionContext);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiMultipleExpressionNode VisitExpressions(ExpressionsContext context)
        {
            return VisitExpressions(context.expression());
        }

        private static QsiMultipleExpressionNode VisitExpressions(IEnumerable<ExpressionContext> contexts)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(contexts.Select(VisitExpression));
            });
        }

        private static QsiExpressionNode VisitNotExpression(NotExpressionContext context)
        {
            // (NOT | '!')  expression
            //  ▔▔\OP/▔▔▔   ▔▔\EXP/▔▔▔

            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = context.notOperator.Text;
                n.Expression.SetValue(VisitExpression(context.expression()));
            });
        }

        private static QsiLogicalExpressionNode VisitLogicalExpression(LogicalExpressionContext context)
        {
            return CreateLogicalExpression(
                context.logicalOperator().GetText(),
                context.left, context.right,
                VisitExpression);
        }

        private static QsiExpressionNode VisitIsExpression(IsExpressionContext context)
        {
            // predicate  IS NOT?  {TRUE | FALSE | UNKNOWN}
            // ▔\LEFT/▔▔  ▔\OP/▔▔   ▔▔▔▔▔▔▔▔\RIGHT/▔▔▔▔▔▔▔▔

            return VisitPredicate(context.predicate());
        }

        private static QsiExpressionNode VisitPredicateExpression(PredicateExpressionContext context)
        {
            return VisitPredicate(context.predicate());
        }

        private static QsiExpressionNode VisitPredicate(PredicateContext context)
        {
            switch (context)
            {
                case InPredicateContext pContext:
                {
                    // predicate  NOT? IN  ({selectStatement | expressions})
                    // ▔\LEFT/▔▔  ▔\OP/▔▔   ▔▔▔▔▔▔▔▔▔▔▔▔\RIGHT/▔▔▔▔▔▔▔▔▔▔▔▔

                    return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
                    {
                        n.Operator = JoinTokens(pContext.NOT(), pContext.IN());
                        n.Left.SetValue(VisitPredicate(pContext.predicate()));

                        if (pContext.selectStatement() != null)
                        {
                            n.Right.SetValue(VisitSelectStatement(pContext.selectStatement()));
                        }
                        else
                        {
                            n.Right.SetValue(VisitExpressions(pContext.expressions()));
                        }

                        UnwrapLogicalExpressionNode(n);
                    });
                }

                case IsNullPredicateContext pContext:
                {
                    // predicate   IS   NOT?  { NULL | \\N }
                    // ▔\LEFT/▔▔  \OP/  ▔▔▔▔▔▔\RIGHT/▔▔▔▔▔▔▔

                    return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
                    {
                        n.Operator = pContext.IS().GetText();
                        n.Left.SetValue(VisitPredicate(pContext.predicate()));
                        n.Right.SetValue(VisitNullNotNull(pContext.nullNotnull()));

                        UnwrapLogicalExpressionNode(n);
                    });
                }

                case BinaryComparasionPredicateContext pContext:
                {
                    // predicate  { = | > | < | <= | >= | <> | != | <=> }  predicate
                    // ▔\LEFT/▔▔   ▔▔▔▔▔▔▔▔▔▔▔▔▔\OPERATOR/▔▔▔▔▔▔▔▔▔▔▔▔▔▔   ▔\RIGHT/▔

                    return CreateLogicalExpression(
                        pContext.comparisonOperator().GetText(),
                        pContext.left, pContext.right,
                        VisitPredicate);
                }

                case SubqueryComparasionPredicateContext pContext:
                {
                    // predicate  { = | > | < | <= | >= | <> | != | <=> } {ALL | ANY | SOME}  (selectStatement)
                    // ▔\LEFT/▔▔   ▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔\OPERATOR/▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔    ▔▔▔▔\RIGHT/▔▔▔▔

                    return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
                    {
                        n.Operator = $"{pContext.comparisonOperator().GetText()} {pContext.quantifier.Text}";
                        n.Left.SetValue(VisitPredicate(pContext.predicate()));
                        n.Right.SetValue(VisitSelectStatement(pContext.selectStatement()));

                        UnwrapLogicalExpressionNode(n);
                    });
                }

                case BetweenPredicateContext pContext:
                {
                    // * Speical Case *
                    // predicate NOT? BETWEEN predicate AND predicate
                    //           ▔▔▔▔▔\▔▔▔▔▔▔           ▔/▔
                    //                 ▔▔▔▔▔▔▔\SKIP/▔▔▔▔▔

                    return TreeHelper.Create<QsiParametersExpressionNode>(n =>
                    {
                        n.Expressions.AddRange(pContext.predicate().Select(VisitPredicate));
                    });
                }

                case SoundsLikePredicateContext pContext:
                {
                    // predicate  SOUNDS LIKE  predicate
                    // ▔\LEFT/▔▔  ▔▔▔\OP/▔▔▔▔  ▔\RIGHT/▔

                    return CreateLogicalExpression(
                        JoinTokens(pContext.SOUNDS(), pContext.LIKE()),
                        pContext.left, pContext.right,
                        VisitPredicate);
                }

                case LikePredicateContext pContext:
                {
                    // * Speical Case *
                    // predicate  NOT? LIKE  predicate  (ESCAPE STRING_LITERAL)?
                    // ▔\LEFT/▔▔  ▔▔\OP/▔▔▔  ▔\RIGHT/▔   ▔▔▔▔▔▔▔\SKIP/▔▔▔▔▔▔▔▔

                    return CreateLogicalExpression(
                        JoinTokens(pContext.NOT(), pContext.LIKE()),
                        pContext.left, pContext.right,
                        VisitPredicate);
                }

                case RegexpPredicateContext pContext:
                {
                    // predicate  NOT? {REGEXP | RLIKE}  predicate
                    // ▔\LEFT/▔▔  ▔▔▔▔▔▔▔▔\OP/▔▔▔▔▔▔▔▔   ▔\RIGHT/▔

                    return CreateLogicalExpression(
                        JoinTokens(pContext.NOT()?.GetText(), pContext.regex.Text),
                        pContext.left, pContext.right,
                        VisitPredicate);
                }

                case ExpressionAtomPredicateContext pContext:
                    return VisitExpressionAtomPredicate(pContext);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiExpressionNode VisitExpressionAtomPredicate(ExpressionAtomPredicateContext context)
        {
            var expression = VisitExpressionAtom(context.expressionAtom());
            var localIdAssign = context.localIdAssign();

            if (localIdAssign != null)
            {
                return VisitLocalIdAssign(context.localIdAssign(), expression);
            }

            return expression;
        }

        private static QsiExpressionNode VisitExpressionAtom(ExpressionAtomContext context)
        {
            switch (context)
            {
                case ConstantExpressionAtomContext pContext:
                {
                    return VisitConstant(pContext.constant());
                }

                case FullColumnNameExpressionAtomContext pContext:
                {
                    return VisitFullColumnName(pContext.fullColumnName());
                }

                case FunctionCallExpressionAtomContext pContext:
                {
                    return VisitFunctionCall(pContext.functionCall());
                }

                case CollateExpressionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiLogicalExpressionNode>(n =>
                    {
                        n.Operator = pContext.COLLATE().GetText();
                        n.Left.SetValue(VisitExpressionAtom(pContext.expressionAtom()));
                        n.Right.SetValue(VisitCollationName(pContext.collationName()));

                        UnwrapLogicalExpressionNode(n);
                    });
                }

                case MysqlVariableExpressionAtomContext pContext:
                {
                    return VisitVariable(pContext.mysqlVariable());
                }

                case UnaryExpressionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                    {
                        n.Operator = pContext.unaryOperator().GetText();
                        n.Expression.SetValue(VisitExpressionAtom(pContext.expressionAtom()));
                    });
                }

                case BinaryExpressionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                    {
                        n.Operator = pContext.BINARY().GetText();
                        n.Expression.SetValue(VisitExpressionAtom(pContext.expressionAtom()));
                    });
                }

                case NestedExpressionAtomContext pContext:
                {
                    return VisitExpressions(pContext.expression());
                }

                case NestedRowExpressionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(pContext.children[0]));
                        n.Parameters.AddRange(pContext.expression().Select(VisitExpression));
                    });
                }

                case ExistsExpessionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionAccess(pContext.children[0]));
                        n.Parameters.Add(VisitSelectStatement(pContext.selectStatement()));
                    });
                }

                case SubqueryExpessionAtomContext pContext:
                {
                    return VisitSelectStatement(pContext.selectStatement());
                }

                case IntervalExpressionAtomContext pContext:
                {
                    // * Speical Case *
                    // INTERVAL  expression  intervalType
                    // ▔▔\OP/▔▔  ▔▔\EXP/▔▔▔  ▔▔▔\SKIP/▔▔▔

                    return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                    {
                        n.Operator = pContext.INTERVAL().GetText();
                        n.Expression.SetValue(VisitExpression(pContext.expression()));
                    });
                }

                case BitExpressionAtomContext pContext:
                {
                    return CreateLogicalExpression(
                        pContext.bitOperator().GetText(),
                        pContext.left,
                        pContext.right,
                        VisitExpressionAtom);
                }

                case MathExpressionAtomContext pContext:
                {
                    return CreateLogicalExpression(
                        pContext.mathOperator().GetText(),
                        pContext.left,
                        pContext.right,
                        VisitExpressionAtom);
                }

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        internal static QsiExpressionNode VisitLocalIdAssign(LocalIdAssignContext context, QsiExpressionNode expression)
        {
            return TreeHelper.Create<QsiAssignExpressionNode>(n =>
            {
                n.Operator = context.VAR_ASSIGN().GetText();
                n.Variable.SetValue(VisitLocalId(context.LOCAL_ID()));
                n.Value.SetValue(expression);
            });
        }

        private static QsiExpressionNode VisitOrderByExpression(OrderByExpressionContext context)
        {
            return VisitExpression(context.expression());
        }

        private static QsiLogicalExpressionNode CreateLogicalExpression<TContext>(
            string @operator,
            TContext left,
            TContext right,
            Func<TContext, QsiExpressionNode> visitor)
        {
            var node = new QsiLogicalExpressionNode
            {
                Operator = @operator
            };

            node.Left.SetValue(visitor(left));
            node.Right.SetValue(visitor(right));

            return UnwrapLogicalExpressionNode(node);
        }

        private static QsiLogicalExpressionNode UnwrapLogicalExpressionNode(QsiLogicalExpressionNode node)
        {
            if (node.Left.Value is QsiMultipleExpressionNode leftArrayExpr)
                node.Left.SetValue(Unwrap(leftArrayExpr));

            if (node.Right.Value is QsiMultipleExpressionNode rightArrayExpr)
                node.Right.SetValue(Unwrap(rightArrayExpr));

            return node;

            static QsiExpressionNode Unwrap(QsiMultipleExpressionNode arrayExpression)
            {
                if (arrayExpression.Elements.Count == 1 &&
                    arrayExpression.Elements[0] is QsiLogicalExpressionNode innerLogicalExpr)
                {
                    return innerLogicalExpr;
                }

                return arrayExpression;
            }
        }
        #endregion

        #region StructureExpression
        private static QsiTableExpressionNode VisitSelectStatement(SelectStatementContext context)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitSelectStatement(context));
            });
        }
        #endregion

        #region ColumnExpression
        private static QsiColumnExpressionNode VisitFullColumnName(FullColumnNameContext context)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                var identifier = IdentifierVisitor.Visit(context);

                if (identifier[^1].Value == "*")
                {
                    n.Column.SetValue(new QsiAllColumnNode
                    {
                        Path = identifier.Level == 1 ? null : new QsiQualifiedIdentifier(identifier[..^1])
                    });
                }
                else
                {
                    n.Column.SetValue(new QsiDeclaredColumnNode
                    {
                        Name = identifier
                    });
                }
            });
        }
        #endregion

        #region Variable
        public static QsiMultipleExpressionNode VisitUserVariables(UserVariablesContext context)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(context.LOCAL_ID().Select(VisitLocalId));
            });
        }

        public static QsiVariableAccessExpressionNode VisitVariable(MysqlVariableContext context)
        {
            if (!(context.children[0] is ITerminalNode terminalNode))
            {
                throw TreeHelper.NotSupportedTree(context);
            }

            switch (terminalNode.Symbol.Type)
            {
                case LOCAL_ID:
                    return VisitLocalId(terminalNode);

                case GLOBAL_ID:
                    return VisitGlobalId(terminalNode);
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiVariableAccessExpressionNode VisitLocalId(ITerminalNode node)
        {
            var text = node.GetText()[1..];
            var identifier = new QsiIdentifier(text, IdentifierUtility.IsEscaped(text));

            return new QsiVariableAccessExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(identifier)
            };
        }

        public static QsiVariableAccessExpressionNode VisitGlobalId(ITerminalNode node)
        {
            var text = node.GetText()[2..];
            var identifier = new QsiIdentifier(text, IdentifierUtility.IsEscaped(text));

            return new QsiVariableAccessExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(identifier)
            };
        }
        #endregion

        #region FunctionAccess
        private static QsiFunctionAccessExpressionNode VisitScalarFunctionName(ScalarFunctionNameContext context)
        {
            if (context.children[0] is FunctionNameBaseContext functionNameBaseContext)
                return VisitFunctionNameBase(functionNameBaseContext);

            return new QsiFunctionAccessExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };
        }

        private static QsiFunctionAccessExpressionNode VisitFunctionNameBase(FunctionNameBaseContext context)
        {
            return new QsiFunctionAccessExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };
        }

        private static QsiFunctionAccessExpressionNode VisitFunctionAccess(IParseTree tree)
        {
            return new QsiFunctionAccessExpressionNode
            {
                Identifier = IdentifierVisitor.Visit(tree)
            };
        }
        #endregion

        #region TypeAccess
        private static QsiTypeAccessExpressionNode VisitTypeAccess(UidContext context)
        {
            return new QsiTypeAccessExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(IdentifierVisitor.VisitUid(context))
            };
        }
        #endregion

        private static string JoinTokens(params string[] tokens)
        {
            return string.Join(" ", tokens.Where(t => t != null));
        }

        private static string JoinTokens(params IParseTree[] trees)
        {
            return string.Join(" ", trees.Where(t => t != null));
        }
    }
}
