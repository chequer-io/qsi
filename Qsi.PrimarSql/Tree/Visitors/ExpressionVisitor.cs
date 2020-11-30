using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;
using static PrimarSql.Internal.PrimarSqlParser;

namespace Qsi.PrimarSql.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode VisitExpression(ExpressionContext context)
        {
            switch (context)
            {
                case NotExpressionContext notExpressionContext:
                    return VisitNotExpression(notExpressionContext);

                case LogicalExpressionContext logicalExpressionContext:
                    return VisitLogicalExpression(logicalExpressionContext);

                case PredicateExpressionContext predicateExpressionContext:
                    return VisitPredicateExpression(predicateExpressionContext);
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitNotExpression(NotExpressionContext context)
        {
            return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
            {
                n.Operator = context.notOperator.Text;
                n.Expression.SetValue(VisitExpression(context.expression()));
            });
        }

        public static QsiBinaryExpressionNode VisitLogicalExpression(LogicalExpressionContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitExpression(context.left));
                n.Operator = context.logicalOperator().GetText();
                n.Right.SetValue(VisitExpression(context.right));

                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitPredicateExpression(PredicateExpressionContext context)
        {
            return VisitPredicate(context.predicate());
        }

        public static QsiExpressionNode VisitPredicate(PredicateContext context)
        {
            switch (context)
            {
                case InPredicateContext inPredicateContext:
                    return VisitInPredicate(inPredicateContext);

                case IsNullPredicateContext isNullPredicateContext:
                    return VisitIsNullPredicate(isNullPredicateContext);

                case BinaryComparasionPredicateContext binaryComparisonPredicateContext:
                    return VisitBinaryComparisonPredicate(binaryComparisonPredicateContext);

                case BetweenPredicateContext betweenPredicateContext:
                    return VisitBetweenPredicate(betweenPredicateContext);

                case LikePredicateContext _:
                    throw TreeHelper.NotSupportedFeature("LIKE");

                case RegexpPredicateContext _:
                    throw TreeHelper.NotSupportedFeature("RLIKE, REGEXP");

                case ExpressionAtomPredicateContext expressionAtomPredicateContext:
                    return VisitExpressionAtomPredicate(expressionAtomPredicateContext);
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        #region Predicate
        public static QsiBinaryExpressionNode VisitInPredicate(InPredicateContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Operator = JoinTokens(context.NOT(), context.IN());
                n.Left.SetValue(VisitPredicate(context.predicate()));

                if (context.selectStatement() != null)
                {
                    n.Right.SetValue(VisitSelectStatement(context.selectStatement()));
                }
                else
                {
                    n.Right.SetValue(VisitExpressions(context.expressions()));
                }

                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitIsNullPredicate(IsNullPredicateContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitPredicate(context.predicate()));
                n.Operator = "IS";
                n.Right.SetValue(VisitNullNotNull(context.nullNotnull()));

                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiBinaryExpressionNode VisitBinaryComparisonPredicate(BinaryComparasionPredicateContext context)
        {
            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Left.SetValue(VisitPredicate(context.left));
                n.Operator = context.comparisonOperator().GetText();
                n.Right.SetValue(VisitPredicate(context.right));

                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitBetweenPredicate(BetweenPredicateContext context)
        {
            return TreeHelper.Create<QsiParametersExpressionNode>(n =>
            {
                n.Expressions.AddRange(context.predicate().Select(VisitPredicate));

                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        public static QsiExpressionNode VisitExpressionAtomPredicate(ExpressionAtomPredicateContext context)
        {
            return VisitExpressionAtom(context.expressionAtom());
        }
        #endregion

        public static QsiExpressionNode VisitExpressionAtom(ExpressionAtomContext context)
        {
            switch (context)
            {
                case ConstantExpressionAtomContext constantExpressionAtomContext:
                    return VisitConstant(constantExpressionAtomContext.constant());

                case FullColumnNameExpressionAtomContext fullColumnNameContext:
                    return VisitFullColumnName(fullColumnNameContext.fullColumnName());

                case FunctionCallExpressionAtomContext functionCallExpressionAtomContext:
                    return VisitFunctionCall(functionCallExpressionAtomContext.functionCall());
            }

            // TODO: Impl

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitNullNotNull(NullNotnullContext context)
        {
            var nullLiteral = VisitLiteral(context.nullLiteral());

            if (context.NOT() != null)
            {
                return TreeHelper.Create<QsiUnaryExpressionNode>(n =>
                {
                    n.Operator = context.NOT().GetText();
                    n.Expression.SetValue(nullLiteral);

                    PrimarSqlTree.PutContextSpan(n, context);
                });
            }

            return nullLiteral;
        }

        #region Function
        public static QsiExpressionNode VisitFunctionCall(FunctionCallContext context)
        {
            if (context.builtInFunctionCall() != null)
                return VisitBuiltInFunctionCall(context.builtInFunctionCall());

            if (context.nativeFunctionCall() != null)
                return VisitNativeFunctionCall(context.nativeFunctionCall());

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitBuiltInFunctionCall(BuiltInFunctionCallContext context)
        {
            throw TreeHelper.NotSupportedFeature("Built-In Function");
        }

        public static QsiExpressionNode VisitNativeFunctionCall(NativeFunctionCallContext context)
        {
            switch (context)
            {
                case UpdateItemFunctionCallContext updateItemFunctionCallContext:
                    return VisitUpdateItemFunction(updateItemFunctionCallContext.updateItemFunction());

                case ConditionExpressionFunctionCallContext conditionExpressionFunctionCallContext:
                    return VisitConditionExpressionFunction(conditionExpressionFunctionCallContext.conditionExpressionFunction());
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitUpdateItemFunction(UpdateItemFunctionContext context)
        {
            switch (context)
            {
                case IfNotExistsFunctionCallContext ifNotExistsFunctionCallContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(TreeHelper.CreateFunction("IF_NOT_EXISTS"));

                        n.Parameters.Add(VisitFullColumnName(ifNotExistsFunctionCallContext.fullColumnName()));
                        n.Parameters.Add(VisitConstant(ifNotExistsFunctionCallContext.constant()));
                    });
                }
            }

            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiExpressionNode VisitConditionExpressionFunction(ConditionExpressionFunctionContext context)
        {
            switch (context)
            {
                case AttributeExistsFunctionCallContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(TreeHelper.CreateFunction("ATTRIBUTE_EXISTS"));

                        n.Parameters.Add(VisitFullColumnName(pContext.fullColumnName()));
                    });
                }

                case AttributeNotExistsFunctionCallContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(TreeHelper.CreateFunction("ATTRIBUTE_NOT_EXISTS"));

                        n.Parameters.Add(VisitFullColumnName(pContext.fullColumnName()));
                    });
                }

                case AttributeTypeFunctionCallContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(TreeHelper.CreateFunction("ATTRIBUTE_TYPE"));

                        n.Parameters.Add(VisitFullColumnName(pContext.fullColumnName()));

                        n.Parameters.Add(TreeHelper.Create<QsiTypeExpressionNode>(pN =>
                        {
                            pN.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(pContext.dataType().GetText(), false));
                        }));
                    });
                }

                case BeginsWithFunctionCallContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(TreeHelper.CreateFunction("BEGINS_WITH"));

                        n.Parameters.Add(VisitFullColumnName(pContext.fullColumnName()));
                        n.Parameters.Add(VisitLiteral(pContext.stringLiteral()));
                    });
                }
                
                case ContainsFunctionCallContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(TreeHelper.CreateFunction("CONTAINS"));

                        n.Parameters.Add(VisitFullColumnName(pContext.fullColumnName()));
                        n.Parameters.Add(VisitLiteral(pContext.stringLiteral()));
                    });
                }
                
                case SizeFunctionCallContext pContext:
                {
                    return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
                    {
                        n.Member.SetValue(TreeHelper.CreateFunction("SIZE"));

                        n.Parameters.Add(VisitFullColumnName(pContext.fullColumnName()));
                    });
                }
            }

            throw TreeHelper.NotSupportedTree(context);
        }
        #endregion

        #region ColumnExpression
        private static QsiColumnExpressionNode VisitFullColumnName(FullColumnNameContext context)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                var identifier = IdentifierVisitor.Visit(context);

                n.Column.SetValue(new QsiDeclaredColumnNode
                {
                    Name = identifier
                });

                PrimarSqlTree.PutContextSpan(n.Column.Value, context);
                PrimarSqlTree.PutContextSpan(n, context);
            });
        }
        #endregion

        #region Literal / Constant
        public static QsiLiteralExpressionNode VisitConstant(ConstantContext context)
        {
            switch (context)
            {
                case StringLiteralConstantContext pContext:
                {
                    return VisitLiteral(pContext.stringLiteral());
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

                    PrimarSqlTree.PutContextSpan(node, literalContext);

                    return node;
                }

                case BitStringConstantContext _:
                {
                    throw TreeHelper.NotSupportedFeature("Bit string");
                }

                case NullConstantContext _:
                {
                    return new QsiLiteralExpressionNode
                    {
                        Value = null,
                        Type = QsiDataType.Null
                    };
                }

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
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

                case StringLiteralContext stringLiteral:
                {
                    value = string.Join("",
                        stringLiteral.STRING_LITERAL()
                            .Select(l => IdentifierUtility.Unescape(l.GetText())));

                    literalType = QsiDataType.String;
                    break;
                }

                case DecimalLiteralContext _:
                    literalType = QsiDataType.Decimal;
                    value = decimal.Parse(context.GetText());
                    break;

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

            PrimarSqlTree.PutContextSpan(node, context);

            return node;
        }
        #endregion

        public static QsiMultipleExpressionNode VisitExpressions(ExpressionsContext context)
        {
            return TreeHelper.Create<QsiMultipleExpressionNode>(n =>
            {
                foreach (var expression in context.expression())
                    n.Elements.Add(VisitExpression(expression));

                PrimarSqlTree.PutContextSpan(n, context);
            });
        }

        #region TableVisitor
        public static QsiTableExpressionNode VisitSelectStatement(SelectStatementContext context)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitSelectStatement(context));
            });
        }
        #endregion

        private static string JoinTokens(params IParseTree[] trees)
        {
            return string.Join(" ", trees.Where(t => t != null));
        }
    }
}
