using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.MySql.Data;
using Qsi.MySql.Tree.Common;
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
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case DataTypeFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.Add(VisitExpression(fContext.expression()));

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case ValuesFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.Add(VisitFullColumnName(fContext.fullColumnName()));

                        MySqlTree.PutContextSpan(n, fContext);
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

                                MySqlTree.PutContextSpan(n, caseFunc);
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

                                MySqlTree.PutContextSpan(n, fContext.elseArg);
                            })
                        );
                    }

                    MySqlTree.PutContextSpan(switchExp, fContext);

                    return switchExp;
                }

                case CharFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.AddRange(VisitFunctionArgs(fContext.functionArgs()));

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case PositionFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));

                        if (LiteralOrExpression(fContext.positionString, fContext.positionExpression, out var param1))
                            n.Parameters.Add(param1);

                        if (LiteralOrExpression(fContext.inString, fContext.inExpression, out var param2))
                            n.Parameters.Add(param2);

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case SubstrFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));

                        if (LiteralOrExpression(fContext.sourceString, fContext.sourceExpression, out var param1))
                            n.Parameters.Add(param1);

                        if (LiteralOrExpression(fContext.fromDecimal, fContext.fromExpression, out var param2))
                            n.Parameters.Add(param2);

                        if (LiteralOrExpression(fContext.forDecimal, fContext.forExpression, out var param3))
                            n.Parameters.Add(param3);

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case TrimFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));

                        if (LiteralOrExpression(fContext.sourceString, fContext.sourceExpression, out var param1))
                            n.Parameters.Add(param1);

                        if (LiteralOrExpression(fContext.fromString, fContext.fromExpression, out var param2))
                            n.Parameters.Add(param2);

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case WeightFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));

                        if (LiteralOrExpression(fContext.stringLiteral(), fContext.expression(), out var param1))
                            n.Parameters.Add(param1);

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case ExtractFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));

                        if (LiteralOrExpression(fContext.stringLiteral(), fContext.expression(), out var param1))
                            n.Parameters.Add(param1);

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case GetFormatFunctionCallContext fContext:
                {
                    // GET_FORMAT  ({DATE | TIME | DATETIME},  string)
                    // ▔\MEMBER/▔    ▔▔▔▔▔▔▔▔\P1/▔▔▔▔▔▔▔▔▔▔    ▔\P2/▔

                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.Add(TreeHelper.CreateConstantLiteral(fContext.datetimeFormat.Text));
                        n.Parameters.Add(VisitLiteral(fContext.stringLiteral()));

                        MySqlTree.PutContextSpan(n.Parameters[0], fContext.datetimeFormat);
                        MySqlTree.PutContextSpan(n, fContext);
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
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.Add(VisitFunctionArg(fContext.functionArg()));

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case CountFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));

                        if (fContext.starArg != null)
                        {
                            n.Parameters.Add(TreeHelper.Create<QsiColumnExpressionNode>(cn =>
                            {
                                cn.Column.SetValue(new QsiAllColumnNode());

                                MySqlTree.PutContextSpan(cn, fContext.starArg);
                                MySqlTree.PutContextSpan(cn.Column.Value, fContext.starArg);
                            }));
                        }
                        else
                        {
                            n.Parameters.Add(VisitFunctionArg(fContext.functionArg()));
                        }

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case CountDistinctFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.AddRange(VisitFunctionArgs(fContext.functionArgs()));

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case LogicalFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.Add(VisitFunctionArg(fContext.functionArg()));

                        MySqlTree.PutContextSpan(n, fContext);
                    });
                }

                case ConcatFunctionCallContext fContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(fContext.children[0]));
                        n.Parameters.AddRange(VisitFunctionArgs(fContext.functionArgs()));

                        OrderByExpressionContext[] orderByExpressions = fContext.orderByExpression();

                        if (orderByExpressions?.Length > 0)
                        {
                            n.Parameters.AddRange(orderByExpressions.Select(VisitOrderByExpression));
                        }

                        MySqlTree.PutContextSpan(n, fContext);
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

                MySqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitUdfFunctionCall(UdfFunctionCallContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(VisitFunctionName(context.fullId()));
                n.Parameters.AddRange(VisitFunctionArgs(context.functionArgs()));

                MySqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiExpressionNode VisitPasswordFunctionClause(PasswordFunctionClauseContext context)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(VisitFunctionName(context.children[0]));
                n.Parameters.Add(VisitFunctionArg(context.functionArg()));

                MySqlTree.PutContextSpan(n, context);
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
                    var literal = VisitLiteral(literalContext.decimalLiteral());
                    literal.Value = -(decimal)literal.Value;
                    return literal;
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
                    var node = new QsiLiteralExpressionNode
                    {
                        Value = decimal.Parse(literalContext.GetText()),
                        Type = QsiDataType.Decimal
                    };

                    MySqlTree.PutContextSpan(node, literalContext);

                    return node;
                }

                case BitStringConstantContext bitStringContext:
                {
                    // B'0101'
                    var node = new QsiLiteralExpressionNode
                    {
                        Value = new MySqlString(MySqlStringKind.Bit, bitStringContext.GetText()[2..^1], null, null),
                        Type = QsiDataType.Custom
                    };

                    MySqlTree.PutContextSpan(node, bitStringContext);

                    return node;
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

                    MySqlTree.PutContextSpan(n, context);
                });
            }

            return nullLiteral;
        }

        public static QsiLiteralExpressionNode VisitLiteral(ParserRuleContext context)
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
                    literalType = QsiDataType.Unknown;
                    value = context.GetText();
                    break;

                case StringLiteralContext stringLiteral:
                {
                    MySqlStringKind? stringKind = null;
                    var charSet = stringLiteral.STRING_CHARSET_NAME()?.GetText();
                    var nationalLiteral = stringLiteral.START_NATIONAL_STRING_LITERAL()?.GetText();
                    var collateName = stringLiteral.collationName()?.GetText();

                    string[] literals = stringLiteral.STRING_LITERAL()?
                        .Select(l => IdentifierUtility.Unescape(l.GetText()))
                        .ToArray();

                    string literalValue = null;

                    if (literals?.Length > 0)
                        literalValue = literals.Length > 1 ? string.Join(string.Empty, literals) : literals[0];

                    if (!string.IsNullOrEmpty(nationalLiteral))
                    {
                        stringKind = MySqlStringKind.National;
                        literalValue = $"{IdentifierUtility.Unescape(nationalLiteral[1..])}{literalValue}";
                    }
                    else if (!string.IsNullOrEmpty(charSet) || !string.IsNullOrEmpty(collateName))
                    {
                        stringKind = MySqlStringKind.Default;
                    }

                    if (stringKind.HasValue)
                    {
                        literalType = QsiDataType.Custom;
                        value = new MySqlString(stringKind.Value, literalValue, charSet, collateName);
                    }
                    else
                    {
                        literalType = QsiDataType.String;
                        value = literalValue;
                    }

                    break;
                }

                case DecimalLiteralContext _:
                    literalType = QsiDataType.Decimal;
                    value = decimal.Parse(context.GetText());
                    break;

                case HexadecimalLiteralContext hexadecimalLiteral:
                {
                    var charSet = hexadecimalLiteral.STRING_CHARSET_NAME()?.GetText();
                    var literalText = hexadecimalLiteral.HEXADECIMAL_LITERAL().GetText();

                    if (literalText[0] == '0')
                    {
                        // 0x00FF00
                        literalType = QsiDataType.Custom;
                        value = new MySqlString(MySqlStringKind.Hexa, literalText[2..], charSet, null);
                    }
                    else
                    {
                        // X'00FF00'
                        literalType = QsiDataType.Custom;
                        value = new MySqlString(MySqlStringKind.HexaString, literalText[2..^1], charSet, null);
                    }

                    break;
                }

                case BooleanLiteralContext _:
                    literalType = QsiDataType.Boolean;
                    value = bool.Parse(context.GetText());
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            var node = new QsiLiteralExpressionNode
            {
                Value = value,
                Type = literalType
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
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
            return VisitExpressions(context, context.expression());
        }

        private static QsiMultipleExpressionNode VisitExpressions(ParserRuleContext context, IEnumerable<ExpressionContext> contexts)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(contexts.Select(VisitExpression));

                MySqlTree.PutContextSpan(n, context);
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

                MySqlTree.PutContextSpan(n, context);
            });
        }

        private static QsiBinaryExpressionNode VisitLogicalExpression(LogicalExpressionContext context)
        {
            var node = CreateBinaryExpression(
                context.logicalOperator().GetText(),
                context.left, context.right,
                VisitExpression);

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitIsExpression(IsExpressionContext context)
        {
            // predicate  IS NOT?  {TRUE | FALSE | UNKNOWN}
            // ▔\LEFT/▔▔  ▔\OP/▔▔   ▔▔▔▔▔▔▔▔\RIGHT/▔▔▔▔▔▔▔▔

            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Operator = context.NOT() == null ? "IS" : "IS NOT";

                n.Left.SetValue(VisitPredicate(context.predicate()));

                n.Right.SetValue(context.testValue.Text.ToUpper() switch
                {
                    "TRUE" => TreeHelper.CreateLiteral(true),
                    "FALSE" => TreeHelper.CreateLiteral(false),
                    "UNKNOWN" => TreeHelper.CreateConstantLiteral("UNKNOWN"),
                    _ => throw new InvalidOperationException()
                });

                MySqlTree.PutContextSpan(n, context);
            });
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

                    return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
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

                        UnwrapBinaryExpressionNode(n);

                        MySqlTree.PutContextSpan(n, pContext);
                    });
                }

                case IsNullPredicateContext pContext:
                {
                    // predicate   IS   NOT?  { NULL | \\N }
                    // ▔\LEFT/▔▔  \OP/  ▔▔▔▔▔▔\RIGHT/▔▔▔▔▔▔▔

                    return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                    {
                        n.Operator = pContext.IS().GetText();
                        n.Left.SetValue(VisitPredicate(pContext.predicate()));
                        n.Right.SetValue(VisitNullNotNull(pContext.nullNotnull()));

                        UnwrapBinaryExpressionNode(n);

                        MySqlTree.PutContextSpan(n, pContext);
                    });
                }

                case BinaryComparasionPredicateContext pContext:
                {
                    // predicate  { = | > | < | <= | >= | <> | != | <=> }  predicate
                    // ▔\LEFT/▔▔   ▔▔▔▔▔▔▔▔▔▔▔▔▔\OPERATOR/▔▔▔▔▔▔▔▔▔▔▔▔▔▔   ▔\RIGHT/▔

                    var node = CreateBinaryExpression(
                        pContext.comparisonOperator().GetText(),
                        pContext.left, pContext.right,
                        VisitPredicate);

                    MySqlTree.PutContextSpan(node, pContext);

                    return node;
                }

                case SubqueryComparasionPredicateContext pContext:
                {
                    // predicate  { = | > | < | <= | >= | <> | != | <=> } {ALL | ANY | SOME}  (selectStatement)
                    // ▔\LEFT/▔▔   ▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔\OPERATOR/▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔    ▔▔▔▔\RIGHT/▔▔▔▔

                    return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
                    {
                        n.Operator = $"{pContext.comparisonOperator().GetText()} {pContext.quantifier.Text}";
                        n.Left.SetValue(VisitPredicate(pContext.predicate()));
                        n.Right.SetValue(VisitSelectStatement(pContext.selectStatement()));

                        UnwrapBinaryExpressionNode(n);

                        MySqlTree.PutContextSpan(n, pContext);
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

                        MySqlTree.PutContextSpan(n, pContext);
                    });
                }

                case SoundsLikePredicateContext pContext:
                {
                    // predicate  SOUNDS LIKE  predicate
                    // ▔\LEFT/▔▔  ▔▔▔\OP/▔▔▔▔  ▔\RIGHT/▔

                    var node = CreateBinaryExpression(
                        JoinTokens(pContext.SOUNDS(), pContext.LIKE()),
                        pContext.left, pContext.right,
                        VisitPredicate);

                    MySqlTree.PutContextSpan(node, pContext);

                    return node;
                }

                case LikePredicateContext pContext:
                {
                    // * Speical Case *
                    // predicate  NOT? LIKE  predicate  (ESCAPE STRING_LITERAL)?
                    // ▔\LEFT/▔▔  ▔▔\OP/▔▔▔  ▔\RIGHT/▔   ▔▔▔▔▔▔▔\SKIP/▔▔▔▔▔▔▔▔

                    var node = CreateBinaryExpression(
                        JoinTokens(pContext.NOT(), pContext.LIKE()),
                        pContext.left, pContext.right,
                        VisitPredicate);

                    MySqlTree.PutContextSpan(node, pContext);

                    return node;
                }

                case RegexpPredicateContext pContext:
                {
                    // predicate  NOT? {REGEXP | RLIKE}  predicate
                    // ▔\LEFT/▔▔  ▔▔▔▔▔▔▔▔\OP/▔▔▔▔▔▔▔▔   ▔\RIGHT/▔

                    var node = CreateBinaryExpression(
                        JoinTokens(pContext.NOT()?.GetText(), pContext.regex.Text),
                        pContext.left, pContext.right,
                        VisitPredicate);

                    MySqlTree.PutContextSpan(node, pContext);

                    return node;
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
                    return VisitCollateExpressionAtom(pContext);
                }

                case MysqlVariableExpressionAtomContext pContext:
                {
                    return VisitVariable(pContext.mysqlVariable());
                }

                case UnaryExpressionAtomContext pContext:
                {
                    var unaryOperator = pContext.unaryOperator().GetText();
                    var expression = VisitExpressionAtom(pContext.expressionAtom());

                    if (expression is QsiLiteralExpressionNode literal)
                    {
                        switch (unaryOperator)
                        {
                            case "-" when literal.Type == QsiDataType.Decimal:
                                literal.Value = -(decimal)literal.Value;
                                return literal;

                            case "!" when literal.Type == QsiDataType.Boolean:
                                literal.Value = !(bool)literal.Value;
                                return literal;
                        }
                    }

                    return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                    {
                        n.Operator = unaryOperator;
                        n.Expression.SetValue(expression);

                        MySqlTree.PutContextSpan(n, pContext);
                    });
                }

                case BinaryExpressionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                    {
                        n.Operator = pContext.BINARY().GetText();
                        n.Expression.SetValue(VisitExpressionAtom(pContext.expressionAtom()));

                        MySqlTree.PutContextSpan(n, pContext);
                    });
                }

                case NestedExpressionAtomContext pContext:
                {
                    return VisitExpressions(pContext, pContext.expression());
                }

                case NestedRowExpressionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(pContext.children[0]));
                        n.Parameters.AddRange(pContext.expression().Select(VisitExpression));

                        MySqlTree.PutContextSpan(n, pContext);
                    });
                }

                case ExistsExpessionAtomContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(VisitFunctionName(pContext.children[0]));
                        n.Parameters.Add(VisitSelectStatement(pContext.selectStatement()));

                        MySqlTree.PutContextSpan(n, pContext);
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

                        MySqlTree.PutContextSpan(n, pContext);
                    });
                }

                case BitExpressionAtomContext pContext:
                {
                    var node = CreateBinaryExpression(
                        pContext.bitOperator().GetText(),
                        pContext.left,
                        pContext.right,
                        VisitExpressionAtom);

                    MySqlTree.PutContextSpan(node, pContext);

                    return node;
                }

                case MathExpressionAtomContext pContext:
                {
                    var node = CreateBinaryExpression(
                        pContext.mathOperator().GetText(),
                        pContext.left,
                        pContext.right,
                        VisitExpressionAtom);

                    MySqlTree.PutContextSpan(node, pContext);

                    return node;
                }

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        private static QsiExpressionNode VisitCollateExpressionAtom(CollateExpressionAtomContext pContext)
        {
            var left = VisitExpressionAtom(pContext.expressionAtom());
            var collate = pContext.collationName().GetText();

            if (left is QsiLiteralExpressionNode literal &&
                literal.Value is MySqlString mySqlString &&
                (mySqlString.Kind == MySqlStringKind.Default || mySqlString.Kind == MySqlStringKind.National) &&
                string.IsNullOrEmpty(mySqlString.CollateName))
            {
                literal.Value = mySqlString.WithCollate(collate);

                MySqlTree.PutContextSpan(literal, pContext);

                return literal;
            }

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction("COLLATE"));
                n.Parameters.Add(left);
                n.Parameters.Add(TreeHelper.CreateConstantLiteral(collate));

                MySqlTree.PutContextSpan(n, pContext);
            });
        }

        internal static QsiExpressionNode VisitLocalIdAssign(LocalIdAssignContext context, QsiExpressionNode expression)
        {
            return TreeHelper.Create<QsiSetVariableExpressionNode>(n =>
            {
                n.Target = IdentifierVisitor.VisitLocalId(context.LOCAL_ID());
                n.AssignmentKind = QsiAssignmentKind.ColonEquals;
                n.Value.SetValue(expression);

                var span = MySqlTree.GetSpan(expression);

                MySqlTree.PutSpan(n, new Range(context.Start.StartIndex, span.End));
            });
        }

        private static QsiBinaryExpressionNode CreateBinaryExpression<TContext>(
            string @operator,
            TContext left,
            TContext right,
            Func<TContext, QsiExpressionNode> visitor)
        {
            var node = new QsiBinaryExpressionNode
            {
                Operator = @operator
            };

            node.Left.SetValue(visitor(left));
            node.Right.SetValue(visitor(right));

            return UnwrapBinaryExpressionNode(node);
        }

        private static QsiBinaryExpressionNode UnwrapBinaryExpressionNode(QsiBinaryExpressionNode node)
        {
            if (node.Left.Value is QsiMultipleExpressionNode leftArrayExpr)
                node.Left.SetValue(Unwrap(leftArrayExpr));

            if (node.Right.Value is QsiMultipleExpressionNode rightArrayExpr)
                node.Right.SetValue(Unwrap(rightArrayExpr));

            return node;

            static QsiExpressionNode Unwrap(QsiMultipleExpressionNode arrayExpression)
            {
                if (arrayExpression.Elements.Count == 1 &&
                    arrayExpression.Elements[0] is QsiBinaryExpressionNode innerLogicalExpr)
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

                MySqlTree.PutContextSpan(n, context);
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

                MySqlTree.PutContextSpan(n.Column.Value, context);
                MySqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Variable
        public static QsiMultipleExpressionNode VisitUserVariables(UserVariablesContext context)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                n.Elements.AddRange(context.LOCAL_ID().Select(VisitLocalId));
                MySqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiVariableExpressionNode VisitVariable(MysqlVariableContext context)
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

        public static QsiVariableExpressionNode VisitLocalId(ITerminalNode terminalNode)
        {
            var node = new QsiVariableExpressionNode
            {
                Identifier = IdentifierVisitor.VisitLocalId(terminalNode)
            };

            MySqlTree.PutContextSpan(node, terminalNode.Symbol);

            return node;
        }

        public static QsiVariableExpressionNode VisitGlobalId(ITerminalNode terminalNode)
        {
            var node = new QsiVariableExpressionNode
            {
                Identifier = IdentifierVisitor.VisitGlobalId(terminalNode)
            };

            MySqlTree.PutContextSpan(node, terminalNode.Symbol);

            return node;
        }
        #endregion

        #region FunctionAccess
        private static QsiFunctionExpressionNode VisitScalarFunctionName(ScalarFunctionNameContext context)
        {
            if (context.children[0] is FunctionNameBaseContext functionNameBaseContext)
                return VisitFunctionNameBase(functionNameBaseContext);

            var node = new QsiFunctionExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiFunctionExpressionNode VisitFunctionNameBase(FunctionNameBaseContext context)
        {
            var node = new QsiFunctionExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.GetText(), false))
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiFunctionExpressionNode VisitFunctionName(IParseTree tree)
        {
            var node = new QsiFunctionExpressionNode
            {
                Identifier = IdentifierVisitor.Visit(tree)
            };

            if (tree.Payload is CommonToken token)
                MySqlTree.PutContextSpan(node, token);

            return node;
        }
        #endregion

        #region TypeAccess
        private static QsiTypeExpressionNode VisitTypeAccess(UidContext context)
        {
            var node = new QsiTypeExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(IdentifierVisitor.VisitUid(context))
            };

            MySqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        public static QsiRowValueExpressionNode VisitExpressionsWithDefaults(ExpressionsWithDefaultsContext context)
        {
            IEnumerable<QsiExpressionNode> expressions = context.expressionOrDefault()
                .Select(c => new CommonExpressionOrDefaultContext(c))
                .Select(VisitExpressionOrDefault);

            var node = new QsiRowValueExpressionNode();
            node.ColumnValues.AddRange(expressions);

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitExpressionOrDefault(CommonExpressionOrDefaultContext context)
        {
            if (context.IsDefault)
            {
                var defaultLiteral = TreeHelper.CreateDefaultLiteral();

                MySqlTree.PutContextSpan(defaultLiteral, context.Context);

                return defaultLiteral;
            }

            return Visit(context.Expression);
        }

        public static QsiSetColumnExpressionNode VisitUpdatedElement(UpdatedElementContext context)
        {
            var assignNode = new QsiSetColumnExpressionNode
            {
                Target = IdentifierVisitor.VisitFullColumnName(context.fullColumnName())
            };

            var valueContext = new CommonExpressionOrDefaultContext(context);
            assignNode.Value.SetValue(VisitExpressionOrDefault(valueContext));

            MySqlTree.PutContextSpan(assignNode, context);

            return assignNode;
        }

        public static QsiWhereExpressionNode VisitCommonWhere(in CommonWhereContext context)
        {
            var node = new QsiWhereExpressionNode();

            node.Expression.SetValue(VisitExpression(context.Expression));
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiMultipleOrderExpressionNode VisitOrderByClause(OrderByClauseContext context)
        {
            var node = new QsiMultipleOrderExpressionNode();

            node.Orders.AddRange(context.orderByExpression().Select(VisitOrderByExpression));
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiOrderExpressionNode VisitOrderByExpression(OrderByExpressionContext context)
        {
            var node = new QsiOrderExpressionNode
            {
                Order = QsiSortOrder.Ascending
            };

            if (context.order?.Text.Equals("DESC", StringComparison.OrdinalIgnoreCase) ?? false)
                node.Order = QsiSortOrder.Descending;

            node.Expression.SetValue(VisitExpression(context.expression()));
            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiLimitExpressionNode VisitLimitClause(LimitClauseContext context)
        {
            return VisitCommonLimitClause(new CommonLimitClauseContext(context));
        }

        public static QsiLimitExpressionNode VisitCommonLimitClause(in CommonLimitClauseContext context)
        {
            var node = new QsiLimitExpressionNode();

            if (context.Offset != null)
                node.Offset.SetValue(VisitLimitClauseAtom(context.Offset));

            if (context.Limit != null)
                node.Limit.SetValue(VisitLimitClauseAtom(context.Limit));

            MySqlTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitLimitClauseAtom(LimitClauseAtomContext context)
        {
            if (context.decimalLiteral() != null)
                return VisitLiteral(context.decimalLiteral());

            return VisitVariable(context.mysqlVariable());
        }

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
