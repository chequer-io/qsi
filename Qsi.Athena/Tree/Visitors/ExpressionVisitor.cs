using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Antlr4.Runtime.Tree;
using Qsi.Athena.Common;
using Qsi.Athena.Internal;
using Qsi.Athena.Tree.Nodes;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Tree.Visitors;

using static SqlBaseParser;

internal static class ExpressionVisitor
{
    #region Expression
    public static QsiExpressionNode VisitExpression(ExpressionContext context)
    {
        return context.children[0] switch
        {
            BooleanExpressionContext booleanExpressionContext => VisitBooleanExpression(booleanExpressionContext),
            _ => throw TreeHelper.NotSupportedTree(context.children[0])
        };
    }
    #endregion

    private static QsiSwitchCaseExpressionNode VisitWhenClause(WhenClauseContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiSwitchCaseExpressionNode>(context);
        node.Condition.Value = VisitExpression(context.condition);
        node.Consequent.Value = VisitExpression(context.result);

        return node;
    }

    internal static QsiExpressionNode VisitParameterExpression(ParameterExpressionContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiBindParameterExpressionNode>(context);

        node.Index = context.index;
        node.Prefix = "?";
        node.NoSuffix = true;
        node.Type = QsiParameterType.Index;

        return node;
    }

    private static QsiExpressionNode VisitQuery(QueryContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiTableExpressionNode>(context);
        node.Table.Value = TableVisitor.VisitQuery(context);

        return node;
    }

    internal static QsiMultipleOrderExpressionNode VisitOrderBy(OrderByContext context)
    {
        SortItemContext[] sortItems = context.sortItem();
        IEnumerable<QsiOrderExpressionNode> sortItemNodes = sortItems.Select(VisitSortItem);

        var node = AthenaTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(context);
        node.Orders.AddRange(sortItemNodes);

        return node;
    }

    internal static QsiOrderExpressionNode VisitSortItem(SortItemContext context)
    {
        var expression = context.expression();
        var ordering = context.ordering;
        var nullOrdering = context.nullOrdering;

        var expressionNode = VisitExpression(expression);

        var node = AthenaTree.CreateWithSpan<AthenaOrderExpressionNode>(context);
        node.Expression.Value = expressionNode;

        if (ordering is not null)
            node.Order = ordering.Type == ASC
                ? QsiSortOrder.Ascending
                : QsiSortOrder.Descending;

        if (context.HasToken(NULLS))
            node.NullBehavior = nullOrdering.Type == FIRST
                ? AthenaOrderByNullBehavior.NullsFirst
                : AthenaOrderByNullBehavior.NullsLast;

        return node;
    }

    internal static AthenaSetQuantifier VisitSetQuantifier(SetQuantifierContext context)
    {
        return context.HasToken(ALL)
            ? AthenaSetQuantifier.All
            : AthenaSetQuantifier.Distinct;
    }

    internal static QsiWhereExpressionNode VisitWhereTerm(WhereTermContext context)
    {
        var where = context.where;
        var whereNode = VisitBooleanExpression(where);

        var node = AthenaTree.CreateWithSpan<QsiWhereExpressionNode>(context);
        node.Expression.Value = whereNode;

        return node;
    }

    internal static AthenaGroupingExpressionNode VisitGroupByHavingTerm(GroupByHavingTermContext context)
    {
        var setQuantifier = context.setQuantifier();
        GroupingElementContext[] groupingElements = context.groupingElement();
        var having = context.having;

        var node = AthenaTree.CreateWithSpan<AthenaGroupingExpressionNode>(context);

        if (setQuantifier is not null)
        {
            var setQuantifierEnum = VisitSetQuantifier(setQuantifier);
            node.SetQuantifier = setQuantifierEnum;
        }

        IEnumerable<QsiExpressionNode> groupingElementNodes = groupingElements.Select(VisitGroupingElement);
        node.Items.AddRange(groupingElementNodes);

        if (having is not null)
            node.Having.Value = VisitBooleanExpression(having);

        return node;
    }

    internal static QsiExpressionNode VisitGroupingElement(GroupingElementContext context)
    {
        return context switch
        {
            SingleGroupingSetContext singleGroupingSet => VisitSingleGroupingSet(singleGroupingSet),
            RollupContext rollup => VisitRollup(rollup),
            CubeContext cube => VisitCube(cube),
            MultipleGroupingSetsContext multipleGroupingSets => VisitMultipleGroupingSets(multipleGroupingSets),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    internal static QsiExpressionNode VisitSingleGroupingSet(SingleGroupingSetContext context)
    {
        return VisitGroupingSet(context.groupingSet());
    }

    internal static QsiExpressionNode VisitRollup(RollupContext context)
    {
        ExpressionContext[] expressions = context.expression();

        IEnumerable<QsiExpressionNode> expressionNodes = expressions.Select(VisitExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction("ROLLUP");
        node.Parameters.AddRange(expressionNodes);

        return node;
    }

    internal static QsiExpressionNode VisitCube(CubeContext context)
    {
        ExpressionContext[] expressions = context.expression();

        IEnumerable<QsiExpressionNode> expressionNodes = expressions.Select(VisitExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction("CUBE");
        node.Parameters.AddRange(expressionNodes);

        return node;
    }

    internal static QsiExpressionNode VisitMultipleGroupingSets(MultipleGroupingSetsContext context)
    {
        GroupingSetContext[] groupSets = context.groupingSet();
        IEnumerable<QsiExpressionNode> groupingSetNodes = groupSets.Select(VisitGroupingSet);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction(AthenaKnownFunction.GroupingSets);
        node.Parameters.AddRange(groupingSetNodes);

        return node;
    }

    internal static QsiExpressionNode VisitGroupingSet(GroupingSetContext context)
    {
        return context switch
        {
            MultipleExpressionGroupingSetContext multipleExpressionGroupingSet => VisitMultipleExpressionGroupingSet(multipleExpressionGroupingSet),
            SingleExpressionGroupingSetContext signSingleExpressionGroupingSet => VisitSingleExpressionGroupingSet(signSingleExpressionGroupingSet),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    internal static QsiExpressionNode VisitMultipleExpressionGroupingSet(MultipleExpressionGroupingSetContext context)
    {
        ExpressionContext[] expressions = context.expression();
        IEnumerable<QsiExpressionNode> expressionNodes = expressions.Select(VisitExpression);

        var node = AthenaTree.CreateWithSpan<QsiMultipleExpressionNode>(context);
        node.Elements.AddRange(expressionNodes);

        return node;
    }

    internal static QsiExpressionNode VisitSingleExpressionGroupingSet(SingleExpressionGroupingSetContext context)
    {
        return VisitExpression(context.expression());
    }

    internal static QsiLimitExpressionNode VisitLimitOffsetTerm(LimitOffsetTermContext context)
    {
        var offsetTerm = context.offsetTerm();
        var limitTerm = context.limitTerm();

        var node = AthenaTree.CreateWithSpan<QsiLimitExpressionNode>(context);

        if (offsetTerm is not null)
        {
            var offsetNode = VisitOffsetTerm(offsetTerm);

            node.Offset.Value = offsetNode;
        }

        if (limitTerm is not null)
        {
            var limitNode = VisitLimitTerm(limitTerm);

            if (limitNode is not null)
                node.Limit.Value = limitNode;
        }

        return node;
    }

    internal static QsiExpressionNode VisitOffsetTerm(OffsetTermContext context)
    {
        return VisitRowCountTerm(context.rowCountTerm());
    }

    internal static QsiExpressionNode VisitLimitTerm(LimitTermContext context)
    {
        return context.HasToken(ALL)
            ? null
            : VisitRowCountTerm(context.rowCountTerm());
    }

    internal static QsiExpressionNode VisitRowCountTerm(RowCountTermContext context)
    {
        if (context.HasToken(INTEGER_VALUE))
        {
            var limitText = context.INTEGER_VALUE().GetText();

            return new QsiLiteralExpressionNode
            {
                Type = QsiDataType.Numeric,
                Value = long.Parse(limitText)
            };
        }

        return VisitParameterExpression(context.parameterExpression());
    }

    public static QsiWhereExpressionNode VisitWhere(BooleanExpressionContext context, ITerminalNode whereNode)
    {
        var node = AthenaTree.CreateWithSpan<QsiWhereExpressionNode>(whereNode.Symbol, context.Stop);
        node.Expression.Value = VisitBooleanExpression(context);

        return node;
    }

    public static QsiMultipleOrderExpressionNode CreateMultipleOrderExpression(SortItemContext[] items, ITerminalNode orderNode)
    {
        var node = AthenaTree.CreateWithSpan<QsiMultipleOrderExpressionNode>(orderNode.Symbol, items[^1].Stop);
        node.Orders.AddRange(items.Select(VisitSortItem));

        return node;
    }

    #region BooleanExpression
    public static QsiExpressionNode VisitBooleanExpression(BooleanExpressionContext context)
    {
        return context switch
        {
            PredicatedContext predicated => VisitPredicated(predicated),
            LogicalNotContext logicalNot => VisitLogicalNot(logicalNot),
            LogicalBinaryContext logicalBinary => VisitLogicalBinary(logicalBinary),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiExpressionNode VisitPredicated(PredicatedContext context)
    {
        var leftNode = VisitValueExpression(context.valueExpression());

        if (context.predicate() is null)
            return leftNode;

        var node = VisitPredicate(context.predicate(), leftNode);
        AthenaTree.PutContextSpan(node, context);

        return node;
    }

    private static QsiExpressionNode VisitLogicalNot(LogicalNotContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiUnaryExpressionNode>(context);

        node.Operator = "NOT";
        node.Expression.Value = VisitBooleanExpression(context.booleanExpression());

        return node;
    }

    private static QsiExpressionNode VisitLogicalBinary(LogicalBinaryContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

        node.Left.Value = VisitBooleanExpression(context.left);
        node.Operator = context.@operator.Text;
        node.Right.Value = VisitBooleanExpression(context.right);

        return node;
    }
    #endregion

    #region Predicate
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
        var comparisonOperator = context.comparisonOperator();
        var valueExpression = context.valueExpression();

        var comparisonOperatorString = VisitComparisonOperator(comparisonOperator);
        var rightNode = VisitValueExpression(valueExpression);

        var node = AthenaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
        node.Left.Value = leftNode;
        node.Operator = comparisonOperatorString;
        node.Right.Value = rightNode;

        return node;
    }

    private static QsiExpressionNode VisitQuantifiedComparison(QuantifiedComparisonContext context, QsiExpressionNode leftNode)
    {
        var comparisonOperator = context.comparisonOperator();
        var comparisonQuantifier = context.comparisonQuantifier();

        var comparisonOperatorString = VisitComparisonOperator(comparisonOperator);
        var comparisonQuantifierString = VisitComparisonQuantifier(comparisonQuantifier);
        var rightNode = VisitQuery(context.query());

        var node = AthenaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
        node.Left.Value = leftNode;
        node.Operator = $"{comparisonOperatorString} ${comparisonQuantifierString}";
        node.Right.Value = rightNode;

        return node;
    }

    private static QsiExpressionNode VisitBetween(BetweenContext context, QsiExpressionNode leftNode)
    {
        var lowerNode = VisitValueExpression(context.lower);
        var upperNode = VisitValueExpression(context.upper);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

        node.Member.Value = AthenaTree.CreateFunction(
            context.HasToken(NOT)
                ? AthenaKnownFunction.NotBetween
                : AthenaKnownFunction.Between
        );

        node.Parameters.Add(leftNode);
        node.Parameters.Add(lowerNode);
        node.Parameters.Add(upperNode);

        return node;
    }

    private static QsiExpressionNode VisitInList(InListContext context, QsiExpressionNode leftNode)
    {
        IEnumerable<QsiExpressionNode> expressions = context.expression().Select(VisitExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

        node.Member.Value = AthenaTree.CreateFunction(
            context.HasToken(NOT)
                ? AthenaKnownFunction.NotIn
                : AthenaKnownFunction.In
        );

        node.Parameters.Add(leftNode);
        node.Parameters.AddRange(expressions);

        return node;
    }

    private static QsiExpressionNode VisitInSubquery(InSubqueryContext context, QsiExpressionNode leftNode)
    {
        var query = context.query();
        var queryNode = VisitQuery(query);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

        node.Member.Value = AthenaTree.CreateFunction(
            context.HasToken(NOT)
                ? AthenaKnownFunction.NotIn
                : AthenaKnownFunction.In
        );

        node.Parameters.Add(leftNode);
        node.Parameters.Add(queryNode);

        return node;
    }

    private static QsiExpressionNode VisitLike(LikeContext context, QsiExpressionNode leftNode)
    {
        var patternNode = VisitValueExpression(context.pattern);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

        node.Member.Value = AthenaTree.CreateFunction(
            context.HasToken(NOT)
                ? AthenaKnownFunction.NotLike
                : AthenaKnownFunction.Like
        );

        node.Parameters.Add(leftNode);
        node.Parameters.Add(patternNode);

        if (context.HasToken(ESCAPE))
        {
            var escapeNode = VisitValueExpression(context.escape);
            node.Parameters.Add(escapeNode);
        }

        return node;
    }

    private static QsiExpressionNode VisitNullPredicate(NullPredicateContext context, QsiExpressionNode leftNode)
    {
        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

        node.Member.Value = AthenaTree.CreateFunction(
            context.HasToken(NOT)
                ? AthenaKnownFunction.IsNotNull
                : AthenaKnownFunction.IsNull
        );

        node.Parameters.Add(leftNode);

        return node;
    }

    private static QsiExpressionNode VisitDistinctFrom(DistinctFromContext context, QsiExpressionNode leftNode)
    {
        var rightNode = VisitValueExpression(context.right);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

        node.Member.Value = AthenaTree.CreateFunction(
            context.HasToken(NOT)
                ? AthenaKnownFunction.IsNotDistinctFrom
                : AthenaKnownFunction.IsDistinctFrom
        );

        node.Parameters.Add(leftNode);
        node.Parameters.Add(rightNode);

        return node;
    }
    #endregion

    #region ValueExpression
    public static QsiExpressionNode VisitValueExpression(ValueExpressionContext context)
    {
        return context switch
        {
            ValueExpressionDefaultContext valueExpressionDefault => VisitValueExpressionDefault(valueExpressionDefault),
            AtTimeZoneContext atTimeZoneContext => VisitAtTimeZone(atTimeZoneContext),
            ArithmeticUnaryContext arithmeticUnary => VisitArithmeticUnary(arithmeticUnary),
            ArithmeticBinaryContext arithmeticBinary => VisitArithmeticBinary(arithmeticBinary),
            ConcatenationContext concatenation => VisitConcatenation(concatenation),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiExpressionNode VisitValueExpressionDefault(ValueExpressionDefaultContext context)
    {
        return VisitPrimaryExpression(context.primaryExpression());
    }

    private static QsiExpressionNode VisitAtTimeZone(AtTimeZoneContext context)
    {
        var value = context.valueExpression();
        var timeZoneSpecifier = context.timeZoneSpecifier();

        var valueNode = VisitValueExpression(value);
        var timeZoneSpecifierNode = VisitTimeZoneSpecifier(timeZoneSpecifier);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction(AthenaKnownFunction.AtTimeZone);
        node.Parameters.Add(valueNode);
        node.Parameters.Add(timeZoneSpecifierNode);

        return node;
    }

    private static QsiExpressionNode VisitArithmeticUnary(ArithmeticUnaryContext context)
    {
        var operatorLiteral = context.@operator.Text;
        var valueNode = VisitValueExpression(context.valueExpression());

        var node = AthenaTree.CreateWithSpan<QsiUnaryExpressionNode>(context);
        node.Operator = operatorLiteral;
        node.Expression.Value = valueNode;

        return node;
    }

    private static QsiExpressionNode VisitArithmeticBinary(ArithmeticBinaryContext context)
    {
        var leftNode = VisitValueExpression(context.left);
        var operatorLiteral = context.@operator.Text;
        var rightNode = VisitValueExpression(context.right);

        var node = AthenaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
        node.Left.Value = leftNode;
        node.Operator = operatorLiteral;
        node.Right.Value = rightNode;

        return node;
    }

    private static QsiExpressionNode VisitConcatenation(ConcatenationContext context)
    {
        var leftNode = VisitValueExpression(context.left);
        var operatorLiteral = context.CONCAT().GetText();
        var rightNode = VisitValueExpression(context.right);

        var node = AthenaTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
        node.Left.Value = leftNode;
        node.Operator = operatorLiteral;
        node.Right.Value = rightNode;

        return node;
    }
    #endregion

    #region PrimaryExpression
    private static QsiExpressionNode VisitPrimaryExpression(PrimaryExpressionContext context)
    {
        return context switch
        {
            NullLiteralContext nullLiteral => VisitNullLiteral(nullLiteral),
            IntervalLiteralContext intervalLiteral => VisitIntervalLiteral(intervalLiteral),
            TypeConstructorContext typeConstructor => VisitTypeConstructor(typeConstructor),
            NumericLiteralContext numericLiteral => VisitNumericLiteral(numericLiteral),
            BooleanLiteralContext booleanLiteral => VisitBooleanLiteral(booleanLiteral),
            StringLiteralContext stringLiteral => VisitStringLiteral(stringLiteral),
            BinaryLiteralContext binaryLiteral => VisitBinaryLiteral(binaryLiteral),
            ParameterContext parameter => VisitParameter(parameter),
            PositionContext position => VisitPosition(position),
            RowConstructorContext rowConstructor => VisitRowConstructor(rowConstructor),
            FunctionCallContext functionCall => VisitFunctionCall(functionCall),
            LambdaContext lambda => VisitLambda(lambda),
            SubqueryExpressionContext subqueryExpression => VisitSubqueryExpression(subqueryExpression),
            ExistsContext exists => VisitExists(exists),
            SimpleCaseContext simpleCase => VisitSimpleCase(simpleCase),
            SearchedCaseContext searchedCase => VisitSearchedCase(searchedCase),
            CastContext cast => VisitCast(cast),
            ArrayConstructorContext arrayConstructor => VisitArrayConstructor(arrayConstructor),
            SubscriptContext subscript => VisitSubscript(subscript),
            ColumnReferenceContext columnReference => VisitColumnReference(columnReference),
            DereferenceContext dereference => VisitDereference(dereference),
            SpecialDateTimeFunctionContext specialDateTimeFunction => VisitSpecialDateTimeFunction(specialDateTimeFunction),
            CurrentUserContext currentUser => VisitCurrentUser(currentUser),
            SubstringContext substring => VisitSubstring(substring),
            NormalizeContext normalize => VisitNormalize(normalize),
            ExtractContext extract => VisitExtract(extract),
            ParenthesizedExpressionContext parenthesizedExpression => VisitParenthesizedExpression(parenthesizedExpression),
            GroupingOperationContext groupingOperation => VisitGroupingOperation(groupingOperation),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiExpressionNode VisitNullLiteral(NullLiteralContext context)
    {
        var nullLiteral = context.NULL().GetText();

        var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
        node.Value = nullLiteral;

        return node;
    }

    private static QsiExpressionNode VisitIntervalLiteral(IntervalLiteralContext context)
    {
        return VisitInterval(context.interval());
    }

    private static AthenaTypeConstructorExpressionNode VisitTypeConstructor(TypeConstructorContext context)
    {
        var identifier = context.HasToken(DOUBLE_PRECISION)
            ? context.DOUBLE_PRECISION().GetText()
            : context.identifier().qi.Value;

        var value = VisitString(context.@string());

        var node = AthenaTree.CreateWithSpan<AthenaTypeConstructorExpressionNode>(context);

        node.Name = new QsiIdentifier(identifier, false);
        node.Expression.Value = value;

        return node;
    }

    private static QsiExpressionNode VisitNumericLiteral(NumericLiteralContext context)
    {
        return VisitNumber(context.number());
    }

    private static QsiExpressionNode VisitBooleanLiteral(BooleanLiteralContext context)
    {
        return VisitBooleanValue(context.booleanValue());
    }

    private static QsiExpressionNode VisitStringLiteral(StringLiteralContext context)
    {
        return VisitString(context.@string());
    }

    private static QsiLiteralExpressionNode VisitBinaryLiteral(BinaryLiteralContext context)
    {
        {
            var value = context.BINARY_LITERAL().GetText();

            var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            node.Value = value;

            return node;
        }
    }

    private static QsiExpressionNode VisitParameter(ParameterContext context)
    {
        return VisitParameterExpression(context.parameterExpression());
    }

    private static QsiInvokeExpressionNode VisitPosition(PositionContext context)
    {
        IEnumerable<QsiExpressionNode> parameterNodes = context.valueExpression().Select(VisitValueExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = AthenaTree.CreateFunction(AthenaKnownFunction.Position);
        node.Parameters.AddRange(parameterNodes);

        return node;
    }

    private static QsiMultipleExpressionNode VisitRowConstructor(RowConstructorContext context)
    {
        IEnumerable<QsiExpressionNode> expressionNodes = context.expression().Select(VisitExpression);

        var node = AthenaTree.CreateWithSpan<QsiMultipleExpressionNode>(context);
        node.Elements.AddRange(expressionNodes);

        return node;
    }

    private static AthenaInvokeExpressionNode VisitFunctionCall(FunctionCallContext context)
    {
        var functionIdentifier = context.qualifiedName().qqi;

        var node = AthenaTree.CreateWithSpan<AthenaInvokeExpressionNode>(context);
        node.Member.Value = AthenaTree.CreateFunction(functionIdentifier);

        var filter = context.filter();
        var over = context.over();

        if (filter is not null)
            node.Filter.Value = VisitFilter(filter);

        if (over is not null)
            node.Over.Value = VisitOver(over);

        if (context.HasToken(ASTERISK))
        {
            var allColumnNode = AthenaTree.CreateAllColumnExpressionNode();
            node.Parameters.Add(allColumnNode);

            return node;
        }

        IEnumerable<QsiExpressionNode> expressionNodes = context.expression().Select(VisitExpression);
        var orderBy = context.orderBy();
        var setQuantifier = context.setQuantifier();
        var nullTreatment = context.nullTreatment();

        node.Parameters.AddRange(expressionNodes);

        if (orderBy is not null)
            node.OrderBy.Value = VisitOrderBy(orderBy);

        if (setQuantifier is not null)
            node.SetQuantifier = VisitSetQuantifier(setQuantifier);

        if (nullTreatment is not null)
            node.NullTreatment = VisitNullTreatment(nullTreatment);

        return node;
    }

    private static AthenaLambdaExpressionNode VisitLambda(LambdaContext context)
    {
        IEnumerable<QsiIdentifier> identifiers = context.identifier().Select(identifierContext => identifierContext.qi);
        var expression = context.expression();
        var expressionNode = VisitExpression(expression);

        var node = AthenaTree.CreateWithSpan<AthenaLambdaExpressionNode>(context);
        node.Identifiers.AddRange(identifiers);
        node.Expression.Value = expressionNode;

        return node;
    }

    private static QsiTableExpressionNode VisitSubqueryExpression(SubqueryExpressionContext context)
    {
        var query = context.query();
        var queryNode = TableVisitor.VisitQuery(query);

        var node = AthenaTree.CreateWithSpan<QsiTableExpressionNode>(context);
        node.Table.Value = queryNode;

        return node;
    }

    private static AthenaExistsExpressionNode VisitExists(ExistsContext context)
    {
        var query = context.query();

        var queryNode = TableVisitor.VisitQuery(query);

        var tableNode = AthenaTree.CreateWithSpan<QsiTableExpressionNode>(query);
        tableNode.Table.Value = queryNode;

        var node = AthenaTree.CreateWithSpan<AthenaExistsExpressionNode>(context);
        node.Query.Value = tableNode;

        return node;
    }

    private static QsiSwitchExpressionNode VisitSimpleCase(SimpleCaseContext context)
    {
        var value = context.valueExpression();
        WhenClauseContext[] whenClauses = context.whenClause();
        var elseExpression = context.elseExpression;

        var valueNode = VisitValueExpression(value);
        IEnumerable<QsiSwitchCaseExpressionNode> whenClauseNodes = whenClauses.Select(VisitWhenClause);

        var node = AthenaTree.CreateWithSpan<QsiSwitchExpressionNode>(context);
        node.Cases.AddRange(whenClauseNodes);
        node.Value.Value = valueNode;

        if (elseExpression is not null)
        {
            var elseExpressionNode = VisitExpression(elseExpression);

            var elseCaseNode = new QsiSwitchCaseExpressionNode
            {
                Consequent =
                {
                    Value = elseExpressionNode
                }
            };

            node.Cases.Add(elseCaseNode);
        }

        return node;
    }

    private static QsiSwitchExpressionNode VisitSearchedCase(SearchedCaseContext context)
    {
        WhenClauseContext[] whenClauses = context.whenClause();
        var elseExpression = context.elseExpression;

        IEnumerable<QsiSwitchCaseExpressionNode> whenCluaseNodes = whenClauses.Select(VisitWhenClause);

        var node = AthenaTree.CreateWithSpan<QsiSwitchExpressionNode>(context);
        node.Cases.AddRange(whenCluaseNodes);

        if (elseExpression is not null)
        {
            var elseExpressionNode = VisitExpression(elseExpression);

            var elseCaseNode = new QsiSwitchCaseExpressionNode
            {
                Consequent =
                {
                    Value = elseExpressionNode
                }
            };

            node.Cases.Add(elseCaseNode);
        }

        return node;
    }

    private static QsiInvokeExpressionNode VisitCast(CastContext context)
    {
        var expression = context.expression();
        var dataType = context.dataType();

        var expressionNode = VisitExpression(expression);
        var dataTypeNode = TreeHelper.CreateConstantLiteral(dataType.GetInputText());

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction(AthenaKnownFunction.Cast);

        node.Parameters.Add(expressionNode);
        node.Parameters.Add(dataTypeNode);

        return node;
    }

    private static QsiInvokeExpressionNode VisitArrayConstructor(ArrayConstructorContext context)
    {
        ExpressionContext[] expressions = context.expression();
        var function = context.ARRAY().GetText();

        IEnumerable<QsiExpressionNode> expressionNodes = expressions.Select(VisitExpression);
        var functionNode = TreeHelper.CreateFunction(function);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = functionNode;
        node.Parameters.AddRange(expressionNodes);

        return node;
    }

    private static AthenaSubscriptExpressionNode VisitSubscript(SubscriptContext context)
    {
        var value = context.value;
        var index = context.index;

        var valueNode = VisitPrimaryExpression(value);
        var indexNode = VisitValueExpression(index);

        var node = AthenaTree.CreateWithSpan<AthenaSubscriptExpressionNode>(context);

        node.Value.Value = valueNode;
        node.Index.Value = indexNode;

        return node;
    }

    private static QsiColumnExpressionNode VisitColumnReference(ColumnReferenceContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiColumnExpressionNode>(context);

        node.Column.Value = new QsiColumnReferenceNode
        {
            Name = new QsiQualifiedIdentifier(context.identifier().qi)
        };

        return node;
    }

    private static QsiExpressionNode VisitDereference(DereferenceContext context)
    {
        var target = VisitPrimaryExpression(context.expr);
        var memberIdentifier = context.fieldName.qi;

        if (
            target is QsiColumnExpressionNode columnExpression &&
            columnExpression.Column.Value is QsiColumnReferenceNode refNode
        )
        {
            refNode.Name = new QsiQualifiedIdentifier(refNode.Name.Append(memberIdentifier));
            AthenaTree.PutContextSpan(refNode, context);
            AthenaTree.PutContextSpan(columnExpression, context);

            return columnExpression;
        }

        var node = AthenaTree.CreateWithSpan<QsiMemberAccessExpressionNode>(context);
        node.Target.Value = target;

        var memberRefNode = new QsiColumnReferenceNode
        {
            Name = new QsiQualifiedIdentifier(memberIdentifier)
        };

        node.Member.Value = new QsiColumnExpressionNode
        {
            Column = { Value = memberRefNode }
        };

        return node;
    }

    private static QsiInvokeExpressionNode VisitSpecialDateTimeFunction(SpecialDateTimeFunctionContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction(context.name.Text);

        if (context.precision is not null)
            node.Parameters.Add(TreeHelper.CreateLiteral(long.Parse(context.precision.Text)));

        return node;
    }

    private static QsiInvokeExpressionNode VisitCurrentUser(CurrentUserContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction("CURRENT_USER");

        return node;
    }

    private static QsiInvokeExpressionNode VisitSubstring(SubstringContext context)
    {
        ValueExpressionContext[] valueExpressions = context.valueExpression();
        IEnumerable<QsiExpressionNode> valueExpressionNodes = valueExpressions.Select(VisitValueExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction("SUBSTRING");

        node.Parameters.AddRange(valueExpressionNodes);

        return node;
    }

    private static QsiInvokeExpressionNode VisitNormalize(NormalizeContext context)
    {
        var valueExpression = context.valueExpression();
        var normalForm = context.normalForm();

        var valueExpressionNode = VisitValueExpression(valueExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction("NORMALIZE");
        node.Parameters.Add(valueExpressionNode);

        if (normalForm is not null)
        {
            var normalFormNode = VisitNormalForm(normalForm);
            node.Parameters.Add(normalFormNode);
        }

        return node;
    }

    private static QsiInvokeExpressionNode VisitExtract(ExtractContext context)
    {
        var identifier = context.identifier().qi;
        var valueExpression = context.valueExpression();

        var identifierNode = AthenaTree.CreateColumnExpressionNode(identifier);
        var valueExpressionNode = VisitValueExpression(valueExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction("EXTRACT");
        node.Parameters.Add(identifierNode);
        node.Parameters.Add(valueExpressionNode);

        return node;
    }

    private static QsiExpressionNode VisitParenthesizedExpression(ParenthesizedExpressionContext context)
    {
        return VisitExpression(context.expression());
    }

    private static QsiInvokeExpressionNode VisitGroupingOperation(GroupingOperationContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction("GROUPING");

        QualifiedNameContext[] qualifiedNames = context.qualifiedName();

        IEnumerable<QsiColumnExpressionNode> qualifiedNameNodes = qualifiedNames
            .Select(name => AthenaTree.CreateColumnExpressionNode(name.qqi));

        node.Parameters.AddRange(qualifiedNameNodes);

        return node;
    }
    #endregion

    #region DataType
    #endregion

    #region Literals
    #region String
    private static QsiExpressionNode VisitString(StringContext context)
    {
        return context switch
        {
            BasicStringLiteralContext basicStringLiteral => VisitBasicStringLiteral(basicStringLiteral),
            UnicodeStringLiteralContext unicodeStringLiteral => VisitUnicodeStringLiteral(unicodeStringLiteral),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiExpressionNode VisitBasicStringLiteral(BasicStringLiteralContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

        node.Type = QsiDataType.String;
        node.Value = context.GetText()[1..^1];

        return node;
    }

    private static QsiExpressionNode VisitUnicodeStringLiteral(UnicodeStringLiteralContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

        var text = context.UNICODE_STRING().GetText()[3..^1];

        var escapeString = context.HasToken(UESCAPE)
            ? context.STRING().GetText()[1]
            : '\\';

        node.Type = QsiDataType.Custom;
        node.Value = new AthenaString(text, escapeString);

        return node;
    }
    #endregion

    #region Number
    private static QsiLiteralExpressionNode VisitNumber(NumberContext context)
    {
        return context switch
        {
            DecimalLiteralContext decimalLiteral => VisitDecimalLiteral(decimalLiteral),
            DoubleLiteralContext doubleLiteral => VisitDoubleLiteral(doubleLiteral),
            IntegerLiteralContext integerLiteral => VisitIntegerLiteral(integerLiteral),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiLiteralExpressionNode VisitDecimalLiteral(DecimalLiteralContext context)
    {
        var text = context.GetText();

        var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

        if (decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            node.Value = value;
            node.Type = QsiDataType.Numeric;
        }
        else
        {
            node.Value = text;
            node.Type = QsiDataType.Raw;
        }

        return node;
    }

    private static QsiLiteralExpressionNode VisitDoubleLiteral(DoubleLiteralContext context)
    {
        var text = context.GetText();

        var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

        if (decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            node.Value = value;
            node.Type = QsiDataType.Decimal;
        }
        else
        {
            node.Value = text;
            node.Type = QsiDataType.Raw;
        }

        return node;
    }

    private static QsiLiteralExpressionNode VisitIntegerLiteral(IntegerLiteralContext context)
    {
        var text = context.GetText();

        var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);

        if (long.TryParse(text, out var value))
        {
            node.Value = value;
            node.Type = QsiDataType.Numeric;
        }
        else
        {
            node.Value = text;
            node.Type = QsiDataType.Raw;
        }

        return node;
    }
    #endregion

    #region Boolean
    private static QsiLiteralExpressionNode VisitBooleanValue(BooleanValueContext context)
    {
        var node = AthenaTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
        node.Type = QsiDataType.Boolean;
        node.Value = context.HasToken(TRUE);

        return node;
    }
    #endregion

    #region NullTreatment
    private static AthenaNullTreatment VisitNullTreatment(NullTreatmentContext context)
    {
        return context.HasToken(IGNORE)
            ? AthenaNullTreatment.IgnoreNulls
            : AthenaNullTreatment.RespectNulls;
    }
    #endregion

    #region TimeZoneSpecifier
    private static QsiExpressionNode VisitTimeZoneSpecifier(TimeZoneSpecifierContext context)
    {
        return context switch
        {
            TimeZoneIntervalContext timeZoneInterval => VisitTimeZoneInterval(timeZoneInterval),
            TimeZoneStringContext timeZoneString => VisitTimeZoneString(timeZoneString),
            _ => throw TreeHelper.NotSupportedTree(context)
        };
    }

    private static QsiExpressionNode VisitTimeZoneInterval(TimeZoneIntervalContext context)
    {
        return VisitInterval(context.interval());
    }

    private static QsiExpressionNode VisitTimeZoneString(TimeZoneStringContext context)
    {
        return VisitString(context.@string());
    }
    #endregion

    #region Comparison Literals
    private static string VisitComparisonOperator(ComparisonOperatorContext context)
    {
        return context.GetText();
    }

    private static string VisitComparisonQuantifier(ComparisonQuantifierContext context)
    {
        return context.GetText();
    }
    #endregion

    #region Interval
    private static QsiExpressionNode VisitInterval(IntervalContext context)
    {
        var sign = context.sign;
        var time = context.@string();
        var from = context.from;
        var to = context.to;

        var timeNode = VisitString(time);

        var node = AthenaTree.CreateWithSpan<AthenaIntervalExpressionNode>(context);

        if (sign is not null)
            timeNode = TreeHelper.CreateUnary(sign.Text, timeNode);

        node.Time.Value = timeNode;

        node.From = VisitIntervalField(from);

        if (context.HasToken(TO))
            node.To = VisitIntervalField(to);

        return node;
    }

    private static AthenaIntervalField VisitIntervalField(IntervalFieldContext context)
    {
        if (context.HasToken(YEAR)) return AthenaIntervalField.Year;
        if (context.HasToken(MONTH)) return AthenaIntervalField.Month;
        if (context.HasToken(DAY)) return AthenaIntervalField.Day;
        if (context.HasToken(HOUR)) return AthenaIntervalField.Hour;
        if (context.HasToken(MINUTE)) return AthenaIntervalField.Minute;
        if (context.HasToken(SECOND)) return AthenaIntervalField.Second;

        throw TreeHelper.NotSupportedFeature($"Interval Field '{context.GetText()}'");
    }
    #endregion

    #region NormalForm
    private static QsiExpressionNode VisitNormalForm(NormalFormContext context)
    {
        return TreeHelper.CreateConstantLiteral(context.GetText());
    }
    #endregion
    #endregion

    #region Terms
    private static QsiExpressionNode VisitFilter(FilterContext context)
    {
        var booleanExpression = context.booleanExpression();

        var booleanExpressionNode = VisitBooleanExpression(booleanExpression);

        var node = AthenaTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
        node.Member.Value = TreeHelper.CreateFunction(AthenaKnownFunction.Filter);
        node.Parameters.Add(booleanExpressionNode);

        return node;
    }

    private static AthenaWindowExpressionNode VisitOver(OverContext context)
    {
        var windowSpecification = context.windowSpecification();
        var windowSpecificationNode = VisitWindowSpecification(windowSpecification);

        var node = AthenaTree.CreateWithSpan<AthenaWindowExpressionNode>(context);
        node.Items.Add(windowSpecificationNode);

        return node;
    }

    private static AthenaWindowItemNode VisitWindowSpecification(WindowSpecificationContext context)
    {
        var node = AthenaTree.CreateWithSpan<AthenaWindowItemNode>(context);

        var partitionBy = context.partitionBy();
        var orderBy = context.orderBy();

        if (context.HasToken(PARTITION))
        {
            var partitionByNode = VisitPartitionBy(partitionBy);
            node.Partition.Value = partitionByNode;
        }

        if (context.HasToken(ORDER))
        {
            var orderByNode = VisitOrderBy(orderBy);
            node.Order.Value = orderByNode;
        }

        if (context.windowFrame() is not null)
            node.Windowing.Value = new QsiExpressionFragmentNode
            {
                // TODO: Question: Why use GetInputText instead of GetText ?
                Text = context.windowFrame().GetInputText()
            };

        return node;
    }

    private static QsiMultipleExpressionNode VisitPartitionBy(PartitionByContext context)
    {
        IList<ExpressionContext> partitions = context._partition;

        IEnumerable<QsiExpressionNode> partitionNodes = partitions.Select(VisitExpression);

        var node = AthenaTree.CreateWithSpan<QsiMultipleExpressionNode>(context);
        node.Elements.AddRange(partitionNodes);

        return node;
    }
    #endregion
}
