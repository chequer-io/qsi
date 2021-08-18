using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Oracle.Internal;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class ExpressionVisitor
    {
        public static IEnumerable<QsiColumnNode> VisitSelectList(SelectListContext context)
        {
            if (context.HasToken(MULT_SYMBOL))
                yield return OracleTree.CreateWithSpan<QsiAllColumnNode>(context);

            foreach (var selectListItem in context.selectListItem())
                yield return VisitSelectListItem(selectListItem);
        }

        public static QsiColumnNode VisitSelectListItem(SelectListItemContext context)
        {
            switch (context)
            {
                case ObjectSelectListItemContext objectSelectListItem:
                    var allNode = OracleTree.CreateWithSpan<QsiAllColumnNode>(objectSelectListItem);
                    allNode.Path = IdentifierVisitor.CreateQualifiedIdentifier(objectSelectListItem.identifier());

                    return allNode;

                case ExprSelectListItemContext exprSelectListItem:
                    QsiColumnNode columnNode;

                    var exprNode = VisitExpr(exprSelectListItem.expr());

                    if (exprNode is QsiColumnExpressionNode columnExpressionNode)
                    {
                        columnNode = columnExpressionNode.Column.Value;
                    }
                    else
                    {
                        var node = OracleTree.CreateWithSpan<QsiDerivedColumnNode>(exprSelectListItem);
                        node.Expression.Value = exprNode;

                        columnNode = node;
                    }

                    if (exprSelectListItem.alias() != null)
                    {
                        if (columnNode is not QsiDerivedColumnNode qsiDerivedColumn)
                        {
                            qsiDerivedColumn = OracleTree.CreateWithSpan<QsiDerivedColumnNode>(exprSelectListItem);
                            qsiDerivedColumn.Column.Value = columnNode;

                            columnNode = qsiDerivedColumn;
                        }

                        qsiDerivedColumn.Alias.Value = new QsiAliasNode
                        {
                            Name = IdentifierVisitor.VisitAlias(exprSelectListItem.alias())
                        };
                    }

                    return columnNode;
            }

            throw new NotSupportedException();
        }

        #region Expr
        public static QsiExpressionNode VisitExpr(ExprContext context)
        {
            while (context is ParenthesisExprContext parens)
                context = parens.expr();

            return context switch
            {
                SignExprContext signExpr => VisitSignExpr(signExpr),
                TimestampExprContext timestampExpr => VisitTimestampExpr(timestampExpr),
                BinaryExprContext binaryExpr => VisitBinaryExpr(binaryExpr),
                CollateExprContext collateExpr => VisitCollateExpr(collateExpr),
                FunctionExprContext functionExpr => VisitFunctionExpr(functionExpr.functionExpression()),
                CalcMeasExprContext calcMeasExpr => VisitCalcMeasExpr(calcMeasExpr.avMeasExpression()),
                CaseExprContext caseExpr => VisitCaseExpr(caseExpr.caseExpression()),
                CursorExprContext cursorExpr => VisitCursorExpr(cursorExpr),
                IntervalExprContext intervalExpr => VisitIntervalExpr(intervalExpr.intervalExpression()),
                ModelExprContext modelExpr => VisitModelExpr(modelExpr.modelExpression()),
                ObjectAccessExprContext objectAccessExpr => VisitObjectAccessExpr(objectAccessExpr.objectAccessExpression()),
                PlaceholderExprContext placeholderExpr => VisitPlaceholderExpr(placeholderExpr.placeholderExpression()),
                ScalarSubqueryExprContext scalarSubqueryExpr => VisitScalarSubqueryExpr(scalarSubqueryExpr),
                TypeConstructorExprContext typeConstructorExpr => VisitTypeConstructorExpr(typeConstructorExpr.typeConstructorExpression()),
                DatetimeExprContext datetimeExpr => VisitDatetimeExpr(datetimeExpr),
                SimpleExprContext simpleExpr => VisitSimpleExpr(simpleExpr.simpleExpression()),
                BindVariableExprContext bindVariableExpr => VisitBindVariable(bindVariableExpr.bindVariable()),
                MultisetExceptExprContext multisetExceptExpr => VisitMultisetExceptExpr(multisetExceptExpr),
                ColumnOuterJoinExprContext columnOuterJoinExpr => VisitColumnOuterJoinExpr(columnOuterJoinExpr),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiExpressionNode VisitSignExpr(SignExprContext context)
        {
            var node = TreeHelper.CreateUnary(context.op.Text, VisitExpr(context.expr()));

            OracleTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitTimestampExpr(TimestampExprContext context)
        {
            var node = TreeHelper.CreateUnary(context.TIMESTAMP().GetText(), VisitExpr(context.expr()));

            OracleTree.PutContextSpan(node, context);

            return node;
        }

        public static QsiExpressionNode VisitBinaryExpr(BinaryExprContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l);
            node.Operator = context.op.Text;
            node.Right.Value = VisitExpr(context.r);

            return node;
        }

        public static QsiExpressionNode VisitCollateExpr(CollateExprContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            var rightNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
            var collationName = IdentifierVisitor.VisitIdentifier(context.r.identifier());
            rightNode.Identifier = new QsiQualifiedIdentifier(collationName);

            node.Left.Value = VisitExpr(context.l);
            node.Operator = context.COLLATE().GetText();
            node.Right.Value = rightNode;

            return node;
        }

        public static QsiExpressionNode VisitFunctionExpr(FunctionExpressionContext context)
        {
            switch (context.children[0])
            {
                case FunctionExpressionContext functionExpressionContext:
                {
                    var accessNode = OracleTree.CreateWithSpan<QsiMemberAccessExpressionNode>(functionExpressionContext);

                    accessNode.Target.Value = VisitFunctionExpr(functionExpressionContext);

                    IEnumerable<IParseTree> childs = context.children
                        .Skip(2)
                        .Where(c => c is FunctionExpressionContext or IdentifierContext);

                    childs.Aggregate((accessNode, 0), (acc, cur) =>
                    {
                        var (accumulator, index) = acc;

                        switch (cur)
                        {
                            case FunctionExpressionContext childFunctionExpressionContext:
                                if (childs.Count() - 1 > index)
                                {
                                    var childAccessNode = OracleTree.CreateWithSpan<QsiMemberAccessExpressionNode>(childFunctionExpressionContext);

                                    childAccessNode.Member.Value = VisitFunctionExpr(childFunctionExpressionContext);
                                    accumulator.Member.Value = childAccessNode;

                                    return (childAccessNode, index + 1);
                                }

                                accumulator.Member.Value = VisitFunctionExpr(childFunctionExpressionContext);
                                return (accumulator, index + 1);

                            case IdentifierContext childIdentifierContext:
                                if (childs.Count() - 1 > index)
                                {
                                    var childAccessNode = OracleTree.CreateWithSpan<QsiMemberAccessExpressionNode>(childIdentifierContext);

                                    childAccessNode.Member.Value = VisitFunctionMemberExpression(childIdentifierContext);
                                    accumulator.Member.Value = childAccessNode;

                                    return (childAccessNode, index + 1);
                                }

                                accumulator.Member.Value = VisitFunctionMemberExpression(childIdentifierContext);
                                return (accumulator, index + 1);
                        }

                        return acc;
                    });

                    static QsiFieldExpressionNode VisitFunctionMemberExpression(IdentifierContext context)
                    {
                        var node = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

                        node.Identifier = new QsiQualifiedIdentifier(
                            IdentifierVisitor.VisitIdentifier(context)
                        );

                        return node;
                    }

                    return accessNode;
                }

                case FunctionNameContext functionNameContext:
                {
                    var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
                    var functionName = IdentifierVisitor.CreateQualifiedIdentifier(functionNameContext.identifier());

                    var functionExpressionNode = OracleTree.CreateWithSpan<QsiFunctionExpressionNode>(context);
                    functionExpressionNode.Identifier = functionName;

                    node.Member.Value = functionExpressionNode;

                    IEnumerable<QsiExpressionNode> argumentList = VisitArgumentList(context.argumentList());

                    if (argumentList is not null)
                        node.Parameters.AddRange(argumentList);

                    return node;
                }

                case AnalyticFunctionContext analyticFunctionContext:
                {

                    return null;
                }

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static IEnumerable<QsiExpressionNode> VisitArgumentList(ArgumentListContext context)
        {
            if (context is null)
                return null;

            while (context.argumentList() is not null)
                context = context.argumentList();

            return context.argument().Select(VisitArgument);
        }

        public static QsiExpressionNode VisitArgument(ArgumentContext context)
        {
            var name = context.identifier();

            if (name is not null)
            {
                var node = OracleTree.CreateWithSpan<OracleNamedParameterExpressionNode>(context);

                node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(name);
                node.Expression.Value = VisitExpr(context.expr());

                return node;
            }

            return VisitExpr(context.expr());
        }

        public static QsiExpressionNode VisitCalcMeasExpr(AvMeasExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCaseExpr(CaseExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCursorExpr(CursorExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitIntervalExpr(IntervalExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitModelExpr(ModelExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitObjectAccessExpr(ObjectAccessExpressionContext context)
        {
            switch (context)
            {
                case ColumnWithExprAccessExpressionContext columnWithExprAccessExpression:
                    throw new NotSupportedException();

                case ColumnAccessExpressionContext columnAccessExpression:
                {
                    var columnExpressionNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(columnAccessExpression);
                    var columnNode = OracleTree.CreateWithSpan<QsiColumnReferenceNode>(columnAccessExpression);
                    columnNode.Name = new QsiQualifiedIdentifier(columnAccessExpression.identifier().Select(IdentifierVisitor.VisitIdentifier));
                    columnExpressionNode.Column.Value = columnNode;

                    return columnExpressionNode;
                }

                case PseudoColumnAccessExpressionContext pseudoColumnAccessExpression:
                {
                    var pseudoColumn = pseudoColumnAccessExpression.pseudoColumn();

                    var columnExpressionNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(pseudoColumnAccessExpression);
                    var columnNode = OracleTree.CreateWithSpan<QsiColumnReferenceNode>(pseudoColumn);
                    var text = pseudoColumn.GetText();
                    var identifier = new QsiIdentifier(text, false);
                    columnNode.Name = new QsiQualifiedIdentifier(identifier);

                    columnExpressionNode.Column.Value = columnNode;

                    return columnExpressionNode;
                }
            }

            throw new NotSupportedException();
        }

        public static QsiExpressionNode VisitPlaceholderExpr(PlaceholderExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitScalarSubqueryExpr(ScalarSubqueryExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitTypeConstructorExpr(TypeConstructorExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitDatetimeExpr(DatetimeExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiLiteralExpressionNode VisitSimpleExpr(SimpleExpressionContext context)
        {
            if (context.HasToken(NULL))
            {
                var node = OracleTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
                node.Type = QsiDataType.Null;
                node.Value = null;

                return node;
            }

            return VisitLiteral(context.literal());
        }

        public static QsiLiteralExpressionNode VisitLiteral(LiteralContext context)
        {
            switch (context.children[0])
            {
                case StringLiteralContext stringLiteral:
                    return VisitStringLiteral(stringLiteral);

                case NumberLiteralContext numberLiteral:
                    return VisitNumberLiteral(numberLiteral);

                case IntervalLiteralContext intervalLiteral:
                    throw new NotImplementedException();

                case DateTimeLiteralContext dateTimeLiteral:
                    throw new NotImplementedException();

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        public static QsiLiteralExpressionNode VisitStringLiteral(StringLiteralContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            node.Type = QsiDataType.String;

            var text = context.GetText();

            switch (context.children[0])
            {
                case ITerminalNode { Symbol: { Type: TK_SINGLE_QUOTED_STRING } }:
                {
                    node.Value = text[1..^1];
                    break;
                }

                case ITerminalNode { Symbol: { Type: TK_QUOTED_STRING } }:
                {
                    node.Value = text[3..^2];
                    break;
                }

                case ITerminalNode { Symbol: { Type: TK_NATIONAL_STRING } }:
                {
                    node.Value = text[1] is 'q' or 'Q' ? text[4..^2] : text[2..^1];
                    break;
                }

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }

            return node;
        }

        public static QsiLiteralExpressionNode VisitNumberLiteral(NumberLiteralContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

            var text = context.GetText();

            switch (context)
            {
                case IntegerLiteralContext:
                {
                    node.Type = QsiDataType.Numeric;
                    node.Value = int.Parse(text);
                    break;
                }

                case NumbericLiteralContext numbericLiteral:
                {
                    if (numbericLiteral.numberType is not null)
                        text = text[..^1];

                    node.Type = QsiDataType.Decimal;
                    node.Value = decimal.Parse(text, System.Globalization.NumberStyles.Float);
                    break;
                }
            }

            return node;
        }

        public static QsiLiteralExpressionNode VisitInteger(IntegerContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            node.Type = QsiDataType.Numeric;
            node.Value = int.Parse(context.GetText());

            return node;
        }

        public static QsiExpressionNode VisitBindVariable(BindVariableContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitMultisetExceptExpr(MultisetExceptExprContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitColumnOuterJoinExpr(ColumnOuterJoinExprContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Condition
        public static QsiExpressionNode VisitCondition(ConditionContext context)
        {
            return context switch
            {
                SimpleComparisonCondition1Context simpleComparisonCondition1 => VisitSimpleComparisonCondition1(simpleComparisonCondition1),
                SimpleComparisonCondition2Context simpleComparisonCondition2 => VisitSimpleComparisonCondition2(simpleComparisonCondition2),
                ComparisonConditionContext comparisonCondition => VisitComparisonCondition(comparisonCondition),
                FloatingPointConditionContext floatingPointCondition => VisitFloatingPointCondition(floatingPointCondition),
                DanglingConditionContext danglingCondition => VisitDanglingCondition(danglingCondition),
                LogicalNotConditionContext logicalNotCondition => VisitLogicalNotCondition(logicalNotCondition),
                LogicalAndConditionContext logicalAndCondition => VisitLogicalAndCondition(logicalAndCondition),
                LogicalOrConditionContext logicalOrCondition => VisitLogicalOrCondition(logicalOrCondition),
                ModelIsAnyConditionContext modelIsAnyCondition => VisitModelIsAnyCondition(modelIsAnyCondition),
                ModelIsPresentConditionContext modelIsPresentCondition => VisitModelIsPresentCondition(modelIsPresentCondition),
                MultisetIsASetConditionContext multisetIsASetCondition => VisitMultisetIsASetCondition(multisetIsASetCondition),
                MultisetIsEmptyConditionContext multisetIsEmptyCondition => VisitMultisetIsEmptyCondition(multisetIsEmptyCondition),
                MultisetMemberConditionContext multisetMemberCondition => VisitMultisetMemberCondition(multisetMemberCondition),
                MultisetSubmultisetConditionContext multisetSubmultisetCondition => VisitMultisetSubmultisetCondition(multisetSubmultisetCondition),
                PatternMatchingLikeConditionContext patternMatchingLikeCondition => VisitPatternMatchingLikeCondition(patternMatchingLikeCondition),
                PatternMatchingRegexpLikeConditionContext patternMatchingRegexpLikeCondition => VisitPatternMatchingRegexpLikeCondition(patternMatchingRegexpLikeCondition),
                IsNullConditionContext isNullCondition => VisitIsNullCondition(isNullCondition),
                XmlEqualsPathConditionContext xmlEqualsPathCondition => VisitXmlEqualsPathCondition(xmlEqualsPathCondition),
                XmlUnderPathConditionContext xmlUnderPathCondition => VisitXmlUnderPathCondition(xmlUnderPathCondition),
                JsonIsJsonConditionContext jsonIsJsonCondition => VisitJsonIsJsonCondition(jsonIsJsonCondition),
                JsonEqualConditionContext jsonEqualCondition => VisitJsonEqualCondition(jsonEqualCondition),
                JsonExistsConditionContext jsonExistsCondition => VisitJsonExistsCondition(jsonExistsCondition),
                JsonTextContainsConditionContext jsonTextContainsCondition => VisitJsonTextContainsCondition(jsonTextContainsCondition),
                CompoundParenthesisConditionContext compoundParenthesisCondition => VisitCompoundParenthesisCondition(compoundParenthesisCondition),
                BetweenConditionContext betweenCondition => VisitBetweenCondition(betweenCondition),
                ExistsConditionContext existsCondition => VisitExistsCondition(existsCondition),
                InCondition1Context inCondition1 => VisitInCondition1(inCondition1),
                IsOfTypeConditionContext isOfTypeCondition => VisitIsOfTypeCondition(isOfTypeCondition),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiExpressionNode VisitSimpleComparisonCondition1(SimpleComparisonCondition1Context context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitSimpleComparisonCondition2(SimpleComparisonCondition2Context context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitComparisonCondition(ComparisonConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitFloatingPointCondition(FloatingPointConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitDanglingCondition(DanglingConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitLogicalNotCondition(LogicalNotConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitLogicalAndCondition(LogicalAndConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitLogicalOrCondition(LogicalOrConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitModelIsAnyCondition(ModelIsAnyConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitModelIsPresentCondition(ModelIsPresentConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitMultisetIsASetCondition(MultisetIsASetConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitMultisetIsEmptyCondition(MultisetIsEmptyConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitMultisetMemberCondition(MultisetMemberConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitMultisetSubmultisetCondition(MultisetSubmultisetConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitPatternMatchingLikeCondition(PatternMatchingLikeConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitPatternMatchingRegexpLikeCondition(PatternMatchingRegexpLikeConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitIsNullCondition(IsNullConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitXmlEqualsPathCondition(XmlEqualsPathConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitXmlUnderPathCondition(XmlUnderPathConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitJsonIsJsonCondition(JsonIsJsonConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitJsonEqualCondition(JsonEqualConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitJsonExistsCondition(JsonExistsConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitJsonTextContainsCondition(JsonTextContainsConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitCompoundParenthesisCondition(CompoundParenthesisConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitBetweenCondition(BetweenConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitExistsCondition(ExistsConditionContext context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitInCondition1(InCondition1Context context)
        {
            throw new NotImplementedException();
        }

        public static QsiExpressionNode VisitIsOfTypeCondition(IsOfTypeConditionContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
