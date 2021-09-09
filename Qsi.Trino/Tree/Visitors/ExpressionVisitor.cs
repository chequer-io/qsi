using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Trino.Common;
using Qsi.Trino.Internal;
using Qsi.Utilities;

namespace Qsi.Trino.Tree.Visitors
{
    using static SqlBaseParser;

    internal static class ExpressionVisitor
    {
        #region Expressions
        public static QsiExpressionNode VisitExpression(ExpressionContext context)
        {
            return context.children[0] switch
            {
                BooleanExpressionContext booleanExpressionContext => VisitBooleanExpression(booleanExpressionContext),
                _ => throw TreeHelper.NotSupportedTree(context.children[0])
            };
        }

        public static QsiExpressionNode VisitBooleanExpression(BooleanExpressionContext context)
        {
            return context switch
            {
                PredicatedContext predicatedContext => VisitPredicated(predicatedContext),
                LogicalNotContext logicalNotContext => VisitLogicalNot(logicalNotContext),
                LogicalBinaryContext logicalBinaryContext => VisitLogicalBinary(logicalBinaryContext),
                _ => throw TreeHelper.NotSupportedTree(context)
            };
        }

        private static QsiExpressionNode VisitLogicalBinary(LogicalBinaryContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitBooleanExpression(context.left);
            node.Operator = context.@operator.Text;
            node.Right.Value = VisitBooleanExpression(context.right);

            return node;
        }

        private static QsiExpressionNode VisitLogicalNot(LogicalNotContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiUnaryExpressionNode>(context);

            node.Operator = "NOT";
            node.Expression.Value = VisitBooleanExpression(context.booleanExpression());

            return node;
        }

        private static QsiExpressionNode VisitPredicated(PredicatedContext context)
        {
            var leftNode = VisitValueExpression(context.valueExpression());

            if (context.predicate() is null)
                return leftNode;

            var node = VisitPredicate(context.predicate(), leftNode);
            TrinoTree.PutContextSpan(node, context);

            return node;
        }

        private static QsiExpressionNode VisitPredicate(PredicateContext context, QsiExpressionNode leftNode)
        {
            return context switch
            {
                ComparisonContext comparison => VisitComparison(comparison, leftNode),
                QuantifiedComparisonContext quantifiedComparison => VisitQuantifiedComparison(quantifiedComparison, leftNode),
                BetweenContext between => VisitBetween(between, leftNode),
                InListContext inList => VisitInList(inList, leftNode),
                InSubqueryContext inSubquery => VisitInSubquery(inSubquery, leftNode),
                LikeContext like => VisitLike(like, leftNode),
                NullPredicateContext nullPredicate => VisitNullPredicate(nullPredicate, leftNode),
                DistinctFromContext distinctFrom => VisitDistinctFrom(distinctFrom, leftNode),
                _ => throw TreeHelper.NotSupportedTree(context)
            };
        }

        private static QsiExpressionNode VisitComparison(ComparisonContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiBinaryExpressionNode();
            node.Left.Value = leftNode;
            node.Operator = context.comparisonOperator().GetText();
            node.Right.Value = VisitValueExpression(context.right);

            return node;
        }

        private static QsiExpressionNode VisitQuantifiedComparison(QuantifiedComparisonContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiBinaryExpressionNode();
            node.Left.Value = leftNode;
            node.Operator = $"{context.comparisonOperator().GetText()} {context.comparisonQuantifier().GetText()}";
            node.Right.Value = VisitQuery(context.query());

            return node;
        }

        private static QsiExpressionNode VisitBetween(BetweenContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.Value = TreeHelper.CreateFunction(context.HasToken(NOT)
                ? TrinoKnownFunction.NotBetween
                : TrinoKnownFunction.Between
            );

            node.Parameters.Add(leftNode);
            node.Parameters.Add(VisitValueExpression(context.lower));
            node.Parameters.Add(VisitValueExpression(context.upper));

            return node;
        }

        private static QsiExpressionNode VisitInList(InListContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.Value = TreeHelper.CreateFunction(context.HasToken(NOT)
                ? TrinoKnownFunction.NotIn
                : TrinoKnownFunction.In
            );

            node.Parameters.Add(leftNode);
            node.Parameters.AddRange(context.expression().Select(VisitExpression));

            return node;
        }

        private static QsiExpressionNode VisitInSubquery(InSubqueryContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.Value = TreeHelper.CreateFunction(context.HasToken(NOT)
                ? TrinoKnownFunction.NotIn
                : TrinoKnownFunction.In
            );

            node.Parameters.Add(leftNode);
            node.Parameters.Add(VisitQuery(context.query()));

            return node;
        }

        private static QsiExpressionNode VisitLike(LikeContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.Value = TreeHelper.CreateFunction(context.HasToken(NOT)
                ? TrinoKnownFunction.NotLike
                : TrinoKnownFunction.Like
            );

            node.Parameters.Add(leftNode);
            node.Parameters.Add(VisitValueExpression(context.pattern));

            if (context.HasToken(ESCAPE))
                node.Parameters.Add(VisitValueExpression(context.escape));

            return node;
        }

        private static QsiExpressionNode VisitNullPredicate(NullPredicateContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.Value = TreeHelper.CreateFunction(context.HasToken(NOT)
                ? TrinoKnownFunction.IsNotNull
                : TrinoKnownFunction.IsNull
            );

            node.Parameters.Add(leftNode);

            return node;
        }

        private static QsiExpressionNode VisitDistinctFrom(DistinctFromContext context, QsiExpressionNode leftNode)
        {
            var node = new QsiInvokeExpressionNode();

            node.Member.Value = TreeHelper.CreateFunction(context.HasToken(NOT)
                ? TrinoKnownFunction.IsNotDistinctFrom
                : TrinoKnownFunction.IsDistinctFrom
            );

            node.Parameters.Add(leftNode);
            node.Parameters.Add(VisitValueExpression(context.right));

            return node;
        }

        public static QsiExpressionNode VisitValueExpression(ValueExpressionContext context)
        {
            switch (context)
            {
                case ValueExpressionDefaultContext valueExpressionDefault:
                    return VisitPrimaryExpression(valueExpressionDefault.primaryExpression());

                case AtTimeZoneContext atTimeZone:
                    return VisitAtTimeZone(atTimeZone);

                case ArithmeticUnaryContext arithmeticUnary:
                    return VisitArithmeticUnary(arithmeticUnary);

                case ArithmeticBinaryContext arithmeticBinary:
                    return VisitArithmeticBinary(arithmeticBinary);

                case ConcatenationContext concatenation:
                    return VisitConcatenation(concatenation);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiExpressionNode VisitAtTimeZone(AtTimeZoneContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(TrinoKnownFunction.AtTimeZone);
            node.Parameters.Add(VisitValueExpression(context.valueExpression()));

            switch (context.timeZoneSpecifier())
            {
                case TimeZoneIntervalContext timeZoneInterval:
                    node.Parameters.Add(VisitInterval(timeZoneInterval.interval()));
                    break;

                case TimeZoneStringContext timeZoneString:
                    node.Parameters.Add(VisitString(timeZoneString.@string()));
                    break;
            }

            return node;
        }

        private static QsiExpressionNode VisitArithmeticUnary(ArithmeticUnaryContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiUnaryExpressionNode>(context);
            node.Operator = context.@operator.Text;
            node.Expression.Value = VisitValueExpression(context.valueExpression());

            return node;
        }

        private static QsiExpressionNode VisitArithmeticBinary(ArithmeticBinaryContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitValueExpression(context.left);
            node.Operator = context.@operator.Text;
            node.Right.Value = VisitValueExpression(context.right);

            return node;
        }

        private static QsiExpressionNode VisitConcatenation(ConcatenationContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitValueExpression(context.left);
            node.Operator = "CONCAT";
            node.Right.Value = VisitValueExpression(context.right);

            return node;
        }

        public static QsiExpressionNode VisitPrimaryExpression(PrimaryExpressionContext context)
        {
            switch (context)
            {
                case NullLiteralContext:
                    return TreeHelper.CreateNullLiteral();

                case IntervalLiteralContext intervalLiteral:
                    return VisitInterval(intervalLiteral.interval());

                case TypeConstructorContext:
                    throw TreeHelper.NotSupportedFeature("Type constructor");

                case NumericLiteralContext numericLiteral:
                    return VisitNumber(numericLiteral.number());

                case BooleanLiteralContext booleanLiteral:
                    return VisitBooleanValue(booleanLiteral.booleanValue());

                case StringLiteralContext stringLiteral:
                    return VisitString(stringLiteral.@string());

                case BinaryLiteralContext binaryLiteral:
                    return VisitBinaryLiteral(binaryLiteral);

                case ParameterContext parameter:
                    return VisitParameterExpression(parameter.parameterExpression());

                case PositionContext position:
                    return VisitPosition(position);

                case RowConstructorContext rowConstructor:
                    return VisitRowConstructor(rowConstructor);

                case FunctionCallContext functionCall:
                    return VisitFunctionCall(functionCall);

                case MeasureContext measure:
                    return VisitMeasure(measure);

                case LambdaContext lambda:
                    return VisitLambda(lambda);

                case SubqueryExpressionContext subqueryExpression:
                    return VisitSubqueryExpression(subqueryExpression);

                case ExistsContext exists:
                    return VisitExists(exists);

                case SimpleCaseContext simpleCase:
                    return VisitSimpleCase(simpleCase);

                case SearchedCaseContext searchedCase:
                    return VisitSearchedCase(searchedCase);

                case CastContext cast:
                    return VisitCast(cast);

                case ArrayConstructorContext arrayConstructor:
                    return VisitArrayConstructor(arrayConstructor);

                case SubscriptContext subscript:
                    return VisitSubscript(subscript);

                case ColumnReferenceContext columnReference:
                    return VisitColumnReference(columnReference);

                case DereferenceContext dereference:
                    return VisitDereference(dereference);

                case SpecialDateTimeFunctionContext specialDateTimeFunction:
                    return VisitSpecialDateTimeFunction(specialDateTimeFunction);

                case CurrentUserContext currentUser:
                    return VisitCurrentUser(currentUser);

                case CurrentCatalogContext currentCatalog:
                    return VisitCurrentCatalog(currentCatalog);

                case CurrentSchemaContext currentSchema:
                    return VisitCurrentSchema(currentSchema);

                case CurrentPathContext currentPath:
                    return VisitCurrentPath(currentPath);

                case SubstringContext substring:
                    return VisitSubstring(substring);

                case NormalizeContext normalize:
                    return VisitNormalize(normalize);

                case ExtractContext extract:
                    return VisitExtract(extract);

                case ParenthesizedExpressionContext parenthesizedExpression:
                    return VisitParenthesizedExpression(parenthesizedExpression);

                case GroupingOperationContext groupingOperation:
                    return VisitGroupingOperation(groupingOperation);

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiExpressionNode VisitColumnReference(ColumnReferenceContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiColumnExpressionNode>(context);

            node.Column.Value = new QsiColumnReferenceNode
            {
                Name = new QsiQualifiedIdentifier(context.identifier().qi)
            };

            return node;
        }

        public static QsiExpressionNode VisitRowCount(RowCountContext context)
        {
            return context.HasToken(INTEGER_VALUE)
                ? TreeHelper.CreateLiteral(long.Parse(context.INTEGER_VALUE().GetText()))
                : VisitParameterExpression(context.parameterExpression());
        }

        private static QsiBindParameterExpressionNode VisitParameterExpression(ParameterExpressionContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiBindParameterExpressionNode>(context);

            node.Index = context.index;
            node.Prefix = "?";
            node.NoSuffix = true;
            node.Type = QsiParameterType.Index;

            return node;
        }

        public static QsiLiteralExpressionNode VisitNumber(NumberContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            var text = context.GetInputText();

            switch (context)
            {
                case DecimalLiteralContext:
                    node.Type = QsiDataType.Numeric;
                    node.Value = long.Parse(text);
                    break;

                case DoubleLiteralContext:
                    node.Type = QsiDataType.Decimal;
                    node.Value = decimal.Parse(text, NumberStyles.Float);
                    break;

                case IntegerLiteralContext:
                    node.Type = QsiDataType.Numeric;
                    node.Value = long.Parse(text);
                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            return node;
        }

        public static QsiLiteralExpressionNode VisitBooleanValue(BooleanValueContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

            node.Type = QsiDataType.Boolean;
            node.Value = context.HasToken(TRUE);

            return node;
        }

        public static QsiLiteralExpressionNode VisitString(StringContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            node.Type = QsiDataType.String;

            switch (context)
            {
                case BasicStringLiteralContext:
                    node.Value = context.GetText()[1..^1];
                    break;

                case UnicodeStringLiteralContext unicodeString:
                {
                    var text = unicodeString.UNICODE_STRING().GetText()[3..^1];

                    char escapeString = unicodeString.HasToken(UESCAPE)
                        ? unicodeString.STRING().GetText()[1]
                        : '\\';

                    var pattern = new Regex($@"{Regex.Escape(escapeString.ToString())}(\d+)");
                    node.Value = pattern.Replace(text, match => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value)));
                    break;
                }
            }

            return node;
        }

        public static QsiLiteralExpressionNode VisitBinaryLiteral(BinaryLiteralContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

            node.Type = QsiDataType.Binary;
            node.Value = context.BINARY_LITERAL().GetText()[2..^1];

            return node;
        }

        public static TrinoIntervalExpressionNode VisitInterval(IntervalContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoIntervalExpressionNode>(context);

            QsiExpressionNode timeNode = VisitString(context.@string());

            if (context.sign is not null)
                timeNode = TreeHelper.CreateUnary(context.sign.Text, timeNode);

            node.Time.Value = timeNode;

            node.From = VisitIntervalField(context.from);
            node.To = VisitIntervalField(context.to);

            return node;
        }

        public static TrinoIntervalField VisitIntervalField(IntervalFieldContext context)
        {
            return context.GetText().ToUpperInvariant() switch
            {
                "YEAR" => TrinoIntervalField.Year,
                "MONTH" => TrinoIntervalField.Month,
                "DAY" => TrinoIntervalField.Day,
                "HOUR" => TrinoIntervalField.Hour,
                "MINUTE" => TrinoIntervalField.Minute,
                "SECOND" => TrinoIntervalField.Second,
                _ => throw TreeHelper.NotSupportedFeature($"Interval Field '{context.GetText()}'")
            };
        }

        public static QsiExpressionNode VisitLambda(LambdaContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoLambdaExpressionNode>(context);

            node.Identifiers.AddRange(context.identifier().Select(i => i.qi));
            node.Expression.Value = VisitExpression(context.expression());

            return node;
        }

        public static QsiExpressionNode VisitSubqueryExpression(SubqueryExpressionContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiTableExpressionNode>(context);
            node.Table.Value = TableVisitor.VisitQuery(context.query());

            return node;
        }

        public static QsiExpressionNode VisitExists(ExistsContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoExistsExpressionNode>(context);
            node.Query.Value = TableVisitor.VisitQuery(context.query());

            return node;
        }

        public static QsiSwitchCaseExpressionNode VisitWhenClause(WhenClauseContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiSwitchCaseExpressionNode>(context);
            node.Condition.Value = VisitExpression(context.condition);
            node.Consequent.Value = VisitExpression(context.result);

            return node;
        }

        public static QsiExpressionNode VisitSimpleCase(SimpleCaseContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiSwitchExpressionNode>(context);

            QsiExpressionNode elseNode = null;

            var valueNode = VisitExpression(context.operand);
            QsiSwitchCaseExpressionNode[] caseExpressionNodes = context.whenClause().Select(VisitWhenClause).ToArray();

            if (context.elseExpression != null)
                elseNode = VisitExpression(context.elseExpression);

            node.Value.Value = valueNode;

            node.Cases.AddRange(caseExpressionNodes);

            if (elseNode != null)
            {
                var caseNode = new QsiSwitchCaseExpressionNode();
                caseNode.Consequent.SetValue(elseNode);
                node.Cases.Add(caseNode);
            }

            return node;
        }

        public static QsiExpressionNode VisitSearchedCase(SearchedCaseContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiSwitchExpressionNode>(context);

            QsiExpressionNode elseNode = null;
            QsiSwitchCaseExpressionNode[] caseExpressionNodes = context.whenClause().Select(VisitWhenClause).ToArray();

            if (context.elseExpression != null)
                elseNode = VisitExpression(context.elseExpression);

            node.Cases.AddRange(caseExpressionNodes);

            if (elseNode != null)
            {
                var caseNode = new QsiSwitchCaseExpressionNode();
                caseNode.Consequent.SetValue(elseNode);
                node.Cases.Add(caseNode);
            }

            return node;
        }

        public static QsiExpressionNode VisitCast(CastContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(TrinoKnownFunction.Cast);

            node.Parameters.Add(VisitExpression(context.expression()));
            node.Parameters.Add(TreeHelper.CreateConstantLiteral(context.type().GetText()));

            return node;
        }

        public static QsiExpressionNode VisitArrayConstructor(ArrayConstructorContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.ARRAY().GetText());

            node.Parameters.AddRange(context.expression().Select(VisitExpression));

            return node;
        }

        public static QsiExpressionNode VisitSubscript(SubscriptContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoSubscriptExpressionNode>(context);

            node.Value.Value = VisitPrimaryExpression(context.value);
            node.Index.Value = VisitValueExpression(context.index);

            return node;
        }

        public static QsiExpressionNode VisitDereference(DereferenceContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiMemberAccessExpressionNode>(context);
            node.Target.Value = VisitPrimaryExpression(context.expr);

            node.Member.Value = new QsiColumnExpressionNode
            {
                Column =
                {
                    Value = new QsiColumnReferenceNode
                    {
                        Name = new QsiQualifiedIdentifier(context.fieldName.qi)
                    }
                }
            };

            return node;
        }

        public static QsiExpressionNode VisitSpecialDateTimeFunction(SpecialDateTimeFunctionContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoDateTimeFunctionExpressionNode>(context);
            node.Name = context.name.Text;

            if (context.precision is not null)
                node.Precision = long.Parse(context.precision.Text);

            return node;
        }

        public static QsiExpressionNode VisitCurrentUser(CurrentUserContext context)
        {
            return TreeHelper.CreateConstantLiteral("CURRENT_USER");
        }

        public static QsiExpressionNode VisitCurrentCatalog(CurrentCatalogContext context)
        {
            return TreeHelper.CreateConstantLiteral("CURRENT_CATALOG");
        }

        public static QsiExpressionNode VisitCurrentSchema(CurrentSchemaContext context)
        {
            return TreeHelper.CreateConstantLiteral("CURRENT_SCHEMA");
        }

        public static QsiExpressionNode VisitCurrentPath(CurrentPathContext context)
        {
            return TreeHelper.CreateConstantLiteral("CURRENT_PATH");
        }

        public static QsiExpressionNode VisitSubstring(SubstringContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SUBSTRING().GetText());

            node.Parameters.AddRange(context.valueExpression().Select(VisitValueExpression));

            return node;
        }

        public static QsiExpressionNode VisitNormalize(NormalizeContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.NORMALIZE().GetText());
            node.Parameters.Add(VisitValueExpression(context.valueExpression()));

            var normalForm = context.normalForm();

            if (normalForm is not null)
                node.Parameters.Add(TreeHelper.CreateConstantLiteral(normalForm.GetText()));

            return node;
        }

        public static QsiExpressionNode VisitExtract(ExtractContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.EXTRACT().GetText());

            node.Parameters.Add(new QsiColumnExpressionNode
            {
                Column =
                {
                    Value = new QsiColumnReferenceNode
                    {
                        Name = new QsiQualifiedIdentifier(context.identifier().qi)
                    }
                }
            });

            node.Parameters.Add(VisitValueExpression(context.valueExpression()));

            return node;
        }

        public static QsiExpressionNode VisitParenthesizedExpression(ParenthesizedExpressionContext context)
        {
            return VisitExpression(context.expression());
        }

        public static QsiExpressionNode VisitGroupingOperation(GroupingOperationContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.GROUPING().GetText());

            QualifiedNameContext[] qualifiedNames = context.qualifiedName();

            node.Parameters.AddRange(qualifiedNames.Select(name => new QsiColumnExpressionNode
                {
                    Column =
                    {
                        Value = new QsiColumnReferenceNode
                        {
                            Name = name.qqi
                        }
                    }
                })
            );

            return node;
        }
        #endregion

        public static QsiExpressionNode VisitQuery(QueryContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiTableExpressionNode>(context);
            node.Table.Value = TableVisitor.VisitQuery(context);

            return node;
        }

        public static QsiExpressionNode VisitPosition(PositionContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(TrinoKnownFunction.Position);
            node.Parameters.AddRange(context.valueExpression().Select(VisitValueExpression));

            return node;
        }

        public static QsiExpressionNode VisitRowConstructor(RowConstructorContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiMultipleExpressionNode>(context);
            node.Elements.AddRange(context.expression().Select(VisitExpression));

            // ROW token ignored

            return node;
        }

        public static QsiExpressionNode VisitFunctionCall(FunctionCallContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoInvokeExpressionNode>(context);
            node.Member.Value = new QsiFunctionExpressionNode { Identifier = context.qualifiedName().qqi };

            if (context.HasToken(ASTERISK))
            {
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("*"));
                return node;
            }

            if (context.processingMode() is not null)
                node.ProcessingMode = context.processingMode().HasToken(RUNNING) ? TrinoProcessingMode.Running : TrinoProcessingMode.Final;

            node.Parameters.AddRange(context.expression().Select(VisitExpression));

            if (context.HasToken(ORDER))
            {
                var orderByNode = TrinoTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(context.ORDER().Symbol, context.sortItem()[^1].Stop);
                orderByNode.Orders.AddRange(context.sortItem().Select(VisitSortItem));

                node.OrderBy.Value = orderByNode;
            }

            if (context.nullTreatment() is not null)
                node.NullTreatment = context.nullTreatment().HasToken(IGNORE) ? TrinoNullTreatment.IgnoreNulls : TrinoNullTreatment.RespectNulls;

            if (context.filter() is not null)
                node.Filter.Value = VisitFilter(context.filter());

            if (context.over() is not null)
                node.Over.Value = VisitOver(context.over());

            return node;
        }

        public static TrinoMeasureExpressionNode VisitMeasure(MeasureContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoMeasureExpressionNode>(context);

            node.Identifier = context.identifier().qi;
            node.Over.Value = VisitOver(context.over());

            return node;
        }

        public static TrinoWindowExpressionNode VisitOver(OverContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoWindowExpressionNode>(context);

            node.Items.Add(context.windowName is not null
                ? new TrinoWindowItemNode { Identifier = context.windowName.qi }
                : VisitWindowSpecification(context.windowSpecification())
            );

            return node;
        }

        public static TrinoWindowItemNode VisitWindowDefinition(WindowDefinitionContext context)
        {
            var node = VisitWindowSpecification(context.windowSpecification());
            node.Identifier = context.name.qi;

            TrinoTree.PutContextSpan(node, context);

            return node;
        }

        public static TrinoWindowItemNode VisitWindowSpecification(WindowSpecificationContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoWindowItemNode>(context);

            if (context.existingWindowName is not null)
                node.ExistingWindow = context.existingWindowName.qi;

            if (context.HasToken(PARTITION))
            {
                var partitionNode = TrinoTree.CreateWithSpan<QsiMultipleExpressionNode>(context.PARTITION().Symbol, context._partition[^1].Stop);
                partitionNode.Elements.AddRange(context._partition.Select(VisitExpression));

                node.Partition.Value = partitionNode;
            }

            if (context.HasToken(ORDER))
            {
                var orderByNode = TrinoTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(context.ORDER().Symbol, context.sortItem()[^1].Stop);
                orderByNode.Orders.AddRange(context.sortItem().Select(VisitSortItem));

                node.Order.Value = orderByNode;
            }

            if (context.windowFrame() is not null)
                node.Windowing.Value = new QsiExpressionFragmentNode { Text = context.windowFrame().GetInputText() };

            return node;
        }

        public static QsiExpressionNode VisitFilter(FilterContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(TrinoKnownFunction.Filter);
            node.Parameters.Add(VisitBooleanExpression(context.booleanExpression()));

            return node;
        }

        public static QsiExpressionNode VisitGroupingElement(GroupingElementContext context)
        {
            switch (context)
            {
                case SingleGroupingSetContext singleGroupingSet:
                {
                    return VisitGroupingSet(singleGroupingSet.groupingSet());
                }

                case RollupContext rollup:
                {
                    var invokeNode = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
                    invokeNode.Member.Value = TreeHelper.CreateFunction("ROLLUP");
                    invokeNode.Parameters.AddRange(rollup.expression().Select(VisitExpression));

                    return invokeNode;
                }

                case CubeContext cube:
                {
                    var invokeNode = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
                    invokeNode.Member.Value = TreeHelper.CreateFunction("CUBE");
                    invokeNode.Parameters.AddRange(cube.expression().Select(VisitExpression));

                    return invokeNode;
                }

                case MultipleGroupingSetsContext multipleGroupingSets:
                {
                    var invokeNode = TrinoTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
                    invokeNode.Member.Value = TreeHelper.CreateFunction(TrinoKnownFunction.GroupingSets);
                    invokeNode.Parameters.AddRange(multipleGroupingSets.groupingSet().Select(VisitGroupingSet));

                    return invokeNode;
                }

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiExpressionNode VisitGroupingSet(GroupingSetContext context)
        {
            if (context.GetText()[0] == '(')
            {
                var multipleExpressionNode = TrinoTree.CreateWithSpan<QsiMultipleExpressionNode>(context);
                multipleExpressionNode.Elements.AddRange(context.expression().Select(VisitExpression));

                return multipleExpressionNode;
            }

            return VisitExpression(context.expression(0));
        }

        public static QsiOrderExpressionNode VisitSortItem(SortItemContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoOrderExpressionNode>(context);

            node.Expression.Value = VisitExpression(context.expression());

            if (context.ordering is not null)
                node.Order = context.ordering.Type == ASC ? QsiSortOrder.Ascending : QsiSortOrder.Descending;

            if (context.HasToken(NULLS))
                node.NullBehavior = context.nullOrdering.Type == FIRST ? TrinoOrderByNullBehavior.NullsFirst : TrinoOrderByNullBehavior.NullsLast;

            return node;
        }

        public static QsiWhereExpressionNode VisitWhere(BooleanExpressionContext context, IToken whereToken)
        {
            var node = TrinoTree.CreateWithSpan<QsiWhereExpressionNode>(whereToken, context.Stop);
            node.Expression.Value = VisitBooleanExpression(context);

            return node;
        }

        public static QsiSetColumnExpressionNode VisitUpdateAssignment(UpdateAssignmentContext context)
        {
            var node = TrinoTree.CreateWithSpan<QsiSetColumnExpressionNode>(context);
            node.Target = new QsiQualifiedIdentifier(context.identifier().qi);
            node.Value.Value = VisitExpression(context.expression());

            return node;
        }
    }
}
