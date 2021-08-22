using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Oracle.Common;
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

                        qsiDerivedColumn.Alias.Value = IdentifierVisitor.VisitAlias(exprSelectListItem.alias());
                    }

                    return columnNode;
            }

            throw new NotSupportedException();
        }

        #region Expr
        public static QsiMultipleExpressionNode VisitExpressionList(ExpressionListContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiMultipleExpressionNode>(context);

            while (context.OPEN_PAR_SYMBOL() != null)
                context = context.expressionList();

            node.Elements.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

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
                                if (context.ChildCount - 3 > index)
                                {
                                    var childAccessNode = OracleTree.CreateWithSpan<QsiMemberAccessExpressionNode>(childFunctionExpressionContext);

                                    childAccessNode.Member.Value = VisitFunctionExpr(childFunctionExpressionContext);
                                    accumulator.Member.Value = childAccessNode;

                                    return (childAccessNode, index + 1);
                                }

                                accumulator.Member.Value = VisitFunctionExpr(childFunctionExpressionContext);
                                return (accumulator, index + 1);

                            case IdentifierContext childIdentifierContext:
                                if (context.ChildCount - 3 > index)
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
                    return VisitCommonFunction(context, functionNameContext, context.argumentList());

                case AnalyticFunctionContext analyticFunctionContext:
                {
                    var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(analyticFunctionContext);

                    switch (analyticFunctionContext.children[0])
                    {
                        case AnyValueFunctionContext anyValueFunctionContext:
                            node.Function.Value = VisitAnyValueFunction(anyValueFunctionContext);
                            break;

                        case AvgFunctionContext avgFunctionContext:
                            node.Function.Value = VisitAvgFunction(avgFunctionContext);
                            break;

                        case BitAndAggFunctionContext bitAndAggFunctionContext:
                            node.Function.Value = VisitBitAndAggFunction(bitAndAggFunctionContext);
                            break;

                        case BitOrAggFunctionContext bitOrAggFunctionContext:
                            node.Function.Value = VisitBitOrAggFunction(bitOrAggFunctionContext);
                            break;

                        case BitXorAggFunctionContext bitXorAggFunctionContext:
                            node.Function.Value = VisitBitXorAggFunction(bitXorAggFunctionContext);
                            break;

                        case ChecksumFunctionContext checksumFunctionContext:
                            node.Function.Value = VisitChecksumFunction(checksumFunctionContext);
                            break;

                        case CorrFunctionContext corrFunctionContext:
                            node.Function.Value = VisitCorrFunction(corrFunctionContext);
                            break;

                        case CountFunctionContext countFunctionContext:
                            node.Function.Value = VisitCountFunction(countFunctionContext);
                            break;

                        case CovarPopFunctionContext covarPopFunctionContext:
                            node.Function.Value = VisitCovarPopFunction(covarPopFunctionContext);
                            break;

                        case CovarSampFunctionContext covarSampFunctionContext:
                            node.Function.Value = VisitCovarSampFunction(covarSampFunctionContext);
                            break;

                        case FirstValueFunctionContext firstValueFunctionContext:
                            node.Function.Value = VisitFirstValueFunction(firstValueFunctionContext);
                            break;

                        case KurtosisPopFunctionContext kurtosisPopFunctionContext:
                            node.Function.Value = VisitKurtosisPopFunction(kurtosisPopFunctionContext);
                            break;

                        case KurtosisSampFunctionContext kurtosisSampFunctionContext:
                            node.Function.Value = VisitKurtosisSampFunction(kurtosisSampFunctionContext);
                            break;

                        case LastValueFunctionContext lastValueFunctionContext:
                            node.Function.Value = VisitLastValueFunction(lastValueFunctionContext);
                            break;

                        case MaxFunctionContext maxFunctionContext:
                            node.Function.Value = VisitMaxFunction(maxFunctionContext);
                            break;

                        case MedianFunctionContext medianFunctionContext:
                            node.Function.Value = VisitMedianFunction(medianFunctionContext);
                            break;

                        case MinFunctionContext minFunctionContext:
                            node.Function.Value = VisitMinFunction(minFunctionContext);
                            break;

                        case NthValueFunctionContext nthValueFunctionContext:
                            node.Function.Value = VisitNthValueFunction(nthValueFunctionContext);
                            break;

                        case LinearRegrFunctionContext linearRegrFunctionContext:
                            node.Function.Value = VisitLinearRegrFunction(linearRegrFunctionContext);
                            break;

                        case StddevFunctionContext stddevFunctionContext:
                            node.Function.Value = VisitStddevFunction(stddevFunctionContext);
                            break;

                        case StddevPopFunctionContext stddevPopFunctionContext:
                            node.Function.Value = VisitStddevPopFunction(stddevPopFunctionContext);
                            break;

                        case StddevSampFunctionContext stddevSampFunctionContext:
                            node.Function.Value = VisitStddevSampFunction(stddevSampFunctionContext);
                            break;

                        case SumFunctionContext sumFunctionContext:
                            node.Function.Value = VisitSumFunction(sumFunctionContext);
                            break;

                        case VarPopFunctionContext varPopFunctionContext:
                            node.Function.Value = VisitVarPopFunction(varPopFunctionContext);
                            break;

                        case VarSampFunctionContext varSampFunctionContext:
                            node.Function.Value = VisitVarSampFunction(varSampFunctionContext);
                            break;

                        case VarianceFunctionContext varianceFunctionContext:
                            node.Function.Value = VisitVarianceFunction(varianceFunctionContext);
                            break;
                    }

                    if (analyticFunctionContext.HasToken(OVER))
                    {
                        if (analyticFunctionContext.windowName is not null)
                        {
                            node.WindowName = IdentifierVisitor.CreateQualifiedIdentifier(analyticFunctionContext.windowName);
                        }
                        else if (analyticFunctionContext.analyticClause() is not null)
                        {
                            var analyticClause = analyticFunctionContext.analyticClause();

                            if (analyticClause.windowName is not null)
                                node.WindowName = IdentifierVisitor.CreateQualifiedIdentifier(analyticClause.windowName);

                            if (analyticClause.queryPartitionClause() is not null)
                                node.Partition.Value = VisitQueryPartitionClause(analyticClause.queryPartitionClause());

                            if (analyticClause.orderByClause() is not null)
                                node.Order.Value = VisitOrderByClause(analyticClause.orderByClause());

                            if (analyticClause.windowingClause() is not null)
                                node.Windowing.Value = VisitWindowingClause(analyticClause.windowingClause());
                        }
                    }

                    return node;
                }

                case CastFunctionContext castFunctionContext:
                    return VisitCastFunction(castFunctionContext);

                case ApproxCountFunctionContext approxCountFunctionContext:
                    return VisitApproxCountFunction(approxCountFunctionContext);

                case ApproxMedianFunctionContext approxMedianFunctionContext:
                    return VisitApproxMedianFunction(approxMedianFunctionContext);

                case ApproxPercentileFunctionContext approxPercentileFunctionContext:
                    return VisitApproxPercentileFunction(approxPercentileFunctionContext);

                case ApproxPercentileDetailFunctionContext approxPercentileDetailFunctionContext:
                    return VisitApproxPercentileDetailFunction(approxPercentileDetailFunctionContext);

                case ApproxRankFunctionContext approxRankFunctionContext:
                    return VisitApproxRankFunction(approxRankFunctionContext);

                case ApproxSumFunctionContext approxSumFunctionContext:
                    return VisitApproxSumFunction(approxSumFunctionContext);

                case BinToNumFunctionContext binToNumFunctionContext:
                    return VisitBinToNumFunction(binToNumFunctionContext);

                case FirstFunctionContext firstFunctionContext:
                    return VisitFirstFunction(firstFunctionContext);

                case ChrFunctionContext chrFunctionContext:
                    return VisitChrFunction(chrFunctionContext);

                case ClusterDetailsFunctionContext clusterDetailsFunctionContext:
                    return VisitClusterDetailsFunction(clusterDetailsFunctionContext);

                case ClusterDetailsAnalyticFunctionContext clusterDetailsAnalyticFunctionContext:
                    return VisitClusterDetailsAnalyticFunction(clusterDetailsAnalyticFunctionContext);

                case ClusterDistanceFunctionContext clusterDistanceFunctionContext:
                    return VisitClusterDistanceFunction(clusterDistanceFunctionContext);

                case ClusterIdFunctionContext clusterIdFunctionContext:
                    return VisitClusterIdFunction(clusterIdFunctionContext);

                case ClusterIdAnalyticFunctionContext clusterIdAnalyticFunctionContext:
                    return VisitClusterIdAnalyticFunction(clusterIdAnalyticFunctionContext);

                case ClusterProbabilityFunctionContext clusterProbabilityFunctionContext:
                    return VisitClusterProbabilityFunction(clusterProbabilityFunctionContext);

                case ClusterProbAnalyticFunctionContext clusterProbAnalyticFunctionContext:
                    return VisitClusterProbAnalyticFunction(clusterProbAnalyticFunctionContext);

                case ClusterSetFunctionContext clusterSetFunctionContext:
                    return VisitClusterSetFunction(clusterSetFunctionContext);

                case ClusterSetAnalyticFunctionContext clusterSetAnalyticFunctionContext:
                    return VisitClusterSetAnalyticFunction(clusterSetAnalyticFunctionContext);

                case CollectFunctionContext collectFunctionContext:
                    return VisitCollectFunction(collectFunctionContext);

                case ConnectByRootFunctionContext connectByRootFunctionContext:
                    return VisitConnectByRootFunction(connectByRootFunctionContext);

                case CorrelationFunctionContext correlationFunctionContext:
                    return VisitCorrelationFunction(correlationFunctionContext);

                case CubeTableFunctionContext cubeTableFunctionContext:
                    return VisitCubeTableFunction(cubeTableFunctionContext);

                case CumeDistFunctionContext cumeDistFunctionContext:
                    return VisitCumeDistFunction(cumeDistFunctionContext);

                case CumeDistAnalyticFunctionContext cumeDistAnalyticFunctionContext:
                    return VisitCumeDistAnalyticFunction(cumeDistAnalyticFunctionContext);

                case CurrentDateFunctionContext currentDateFunctionContext:
                    return VisitCurrentDateFunction(currentDateFunctionContext);

                case CurrentTimestampFunctionContext currentTimestampFunctionContext:
                    return VisitCurrentTimestampFunction(currentTimestampFunctionContext);

                case DbTimeZoneFunctionContext dbTimeZoneFunctionContext:
                    return VisitDbTimeZoneFunction(dbTimeZoneFunctionContext);

                case DenseRankAggregateFunctionContext denseRankAggregateFunctionContext:
                    return VisitDenseRankAggregateFunction(denseRankAggregateFunctionContext);

                case DenseRankAnalyticFunctionContext denseRankAnalyticFunctionContext:
                    return VisitDenseRankAnalyticFunction(denseRankAnalyticFunctionContext);

                case ExtractDateTimeFunctionContext extractDateTimeFunctionContext:
                    return VisitExtractDateTimeFunction(extractDateTimeFunctionContext);

                case FeatureCompareFunctionContext featureCompareFunctionContext:
                    return VisitFeatureCompareFunction(featureCompareFunctionContext);

                case FeatureDetailsFunctionContext featureDetailsFunctionContext:
                    return VisitFeatureDetailsFunction(featureDetailsFunctionContext);

                case FeatureIdFunctionContext featureIdFunctionContext:
                    return VisitFeatureIdFunction(featureIdFunctionContext);

                case FeatureIdAnalyticFunctionContext featureIdAnalyticFunctionContext:
                    return VisitFeatureIdAnalyticFunction(featureIdAnalyticFunctionContext);

                case FeatureSetFunctionContext featureSetFunctionContext:
                    return VisitFeatureSetFunction(featureSetFunctionContext);

                case FeatureSetAnalyticFunctionContext featureSetAnalyticFunctionContext:
                    return VisitFeatureSetAnalyticFunction(featureSetAnalyticFunctionContext);

                case FeatureValueFunctionContext featureValueFunctionContext:
                    return VisitFeatureValueFunction(featureValueFunctionContext);

                case FeatureValueAnalyticFunctionContext featureValueAnalyticFunctionContext:
                    return VisitFeatureValueAnalyticFunction(featureValueAnalyticFunctionContext);

                case FirstValueFunctionContext firstValueFunctionContext:
                    return VisitFirstValueFunction(firstValueFunctionContext);

                case IterationNumberFunctionContext iterationNumberFunctionContext:
                    return VisitIterationNumberFunction(iterationNumberFunctionContext);

                case JsonArrayFunctionContext jsonArrayFunctionContext:
                    return VisitJsonArrayFunction(jsonArrayFunctionContext);

                case JsonArrayAggFunctionContext jsonArrayAggFunctionContext:
                    return VisitJsonArrayAggFunction(jsonArrayAggFunctionContext);

                case JsonMergePatchFunctionContext jsonMergePatchFunctionContext:
                    return VisitJsonMergePatchFunction(jsonMergePatchFunctionContext);

                case JsonObjectFunctionContext jsonObjectFunctionContext:
                    return VisitJsonObjectFunction(jsonObjectFunctionContext);

                case JsonObjectaggFunctionContext jsonObjectaggFunctionContext:
                    return VisitJsonObjectaggFunction(jsonObjectaggFunctionContext);

                case JsonQueryFunctionContext jsonQueryFunctionContext:
                    return VisitJsonQueryFunction(jsonQueryFunctionContext);

                case JsonScalarFunctionContext jsonScalarFunctionContext:
                    return VisitJsonScalarFunction(jsonScalarFunctionContext);

                case JsonSerializeFunctionContext jsonSerializeFunctionContext:
                    return VisitJsonSerializeFunction(jsonSerializeFunctionContext);

                case JsonTableFunctionContext jsonTableFunctionContext:
                    return VisitJsonTableFunction(jsonTableFunctionContext);

                case JsonTransformFunctionContext jsonTransformFunctionContext:
                    return VisitJsonTransformFunction(jsonTransformFunctionContext);

                case JsonValueFunctionContext jsonValueFunctionContext:
                    return VisitJsonValueFunction(jsonValueFunctionContext);

                case LagFunctionContext lagFunctionContext:
                    return VisitLagFunction(lagFunctionContext);

                case LastFunctionContext lastFunctionContext:
                    return VisitLastFunction(lastFunctionContext);

                case LeadFunctionContext leadFunctionContext:
                    return VisitLeadFunction(leadFunctionContext);

                case ListaggFunctionContext listaggFunctionContext:
                    return VisitListaggFunction(listaggFunctionContext);

                case LocaltimestampFunctionContext localtimestampFunctionContext:
                    return VisitLocaltimestampFunction(localtimestampFunctionContext);

                case NtileFunctionContext ntileFunctionContext:
                    return VisitNtileFunction(ntileFunctionContext);

                case OraDmPartitionNameFunctionContext oraDmPartitionNameFunctionContext:
                    return VisitOraDmPartitionNameFunction(oraDmPartitionNameFunctionContext);

                case OraInvokingUserFunctionContext oraInvokingUserFunctionContext:
                    return VisitOraInvokingUserFunction(oraInvokingUserFunctionContext);

                case OraInvokingUserIdFunctionContext oraInvokingUserIdFunctionContext:
                    return VisitOraInvokingUserIdFunction(oraInvokingUserIdFunctionContext);

                case PercentRankAggregateFunctionContext percentRankAggregateFunctionContext:
                    return VisitPercentRankAggregateFunction(percentRankAggregateFunctionContext);

                case PercentRankAnalyticFunctionContext percentRankAnalyticFunctionContext:
                    return VisitPercentRankAnalyticFunction(percentRankAnalyticFunctionContext);

                case PercentileContFunctionContext percentileContFunctionContext:
                    return VisitPercentileContFunction(percentileContFunctionContext);

                case PercentileDiscFunctionContext percentileDiscFunctionContext:
                    return VisitPercentileDiscFunction(percentileDiscFunctionContext);

                case PredictionFunctionContext predictionFunctionContext:
                    return VisitPredictionFunction(predictionFunctionContext);

                case PredictionOrderedFunctionContext predictionOrderedFunctionContext:
                    return VisitPredictionOrderedFunction(predictionOrderedFunctionContext);

                case PredictionAnalyticFunctionContext predictionAnalyticFunctionContext:
                    return VisitPredictionAnalyticFunction(predictionAnalyticFunctionContext);

                case PredictionBoundsFunctionContext predictionBoundsFunctionContext:
                    return VisitPredictionBoundsFunction(predictionBoundsFunctionContext);

                case PredictionCostFunctionContext predictionCostFunctionContext:
                    return VisitPredictionCostFunction(predictionCostFunctionContext);

                case PredictionCostAnalyticFunctionContext predictionCostAnalyticFunctionContext:
                    return VisitPredictionCostAnalyticFunction(predictionCostAnalyticFunctionContext);

                case PredictionDetailsFunctionContext predictionDetailsFunctionContext:
                    return VisitPredictionDetailsFunction(predictionDetailsFunctionContext);

                case PredictionDetailsAnalyticFunctionContext predictionDetailsAnalyticFunctionContext:
                    return VisitPredictionDetailsAnalyticFunction(predictionDetailsAnalyticFunctionContext);

                case PredictionProbabilityFunctionContext predictionProbabilityFunctionContext:
                    return VisitPredictionProbabilityFunction(predictionProbabilityFunctionContext);

                case PredictionProbabilityOrderedFunctionContext predictionProbabilityOrderedFunctionContext:
                    return VisitPredictionProbabilityOrderedFunction(predictionProbabilityOrderedFunctionContext);

                case PredictionProbAnalyticFunctionContext predictionProbAnalyticFunctionContext:
                    return VisitPredictionProbAnalyticFunction(predictionProbAnalyticFunctionContext);

                case PredictionSetFunctionContext predictionSetFunctionContext:
                    return VisitPredictionSetFunction(predictionSetFunctionContext);

                case PredictionSetOrderedFunctionContext predictionSetOrderedFunctionContext:
                    return VisitPredictionSetOrderedFunction(predictionSetOrderedFunctionContext);

                case PredictionSetAnalyticFunctionContext predictionSetAnalyticFunctionContext:
                    return VisitPredictionSetAnalyticFunction(predictionSetAnalyticFunctionContext);

                case RankAggregateFunctionContext rankAggregateFunctionContext:
                    return VisitRankAggregateFunction(rankAggregateFunctionContext);

                case RankAnalyticFunctionContext rankAnalyticFunctionContext:
                    return VisitRankAnalyticFunction(rankAnalyticFunctionContext);

                case RatioToReportFunctionContext ratioToReportFunctionContext:
                    return VisitRatioToReportFunction(ratioToReportFunctionContext);

                case SessiontimezoneFunctionContext sessiontimezoneFunctionContext:
                    return VisitSessiontimezoneFunction(sessiontimezoneFunctionContext);

                case RowNumberFunctionContext rowNumberFunctionContext:
                    return VisitRowNumberFunction(rowNumberFunctionContext);

                case SkewnessPopFunctionContext skewnessPopFunctionContext:
                    return VisitSkewnessPopFunction(skewnessPopFunctionContext);

                case SkewnessSampFunctionContext skewnessSampFunctionContext:
                    return VisitSkewnessSampFunction(skewnessSampFunctionContext);

                case SysDburigenFunctionContext sysDburigenFunctionContext:
                    return VisitSysDburigenFunction(sysDburigenFunctionContext);

                case SysdateFunctionContext sysdateFunctionContext:
                    return VisitSysdateFunction(sysdateFunctionContext);

                case SystimestampFunctionContext systimestampFunctionContext:
                    return VisitSystimestampFunction(systimestampFunctionContext);

                case ToBinaryDoubleFunctionContext toBinaryDoubleFunctionContext:
                    return VisitToBinaryDoubleFunction(toBinaryDoubleFunctionContext);

                case ToBinaryFloatFunctionContext toBinaryFloatFunctionContext:
                    return VisitToBinaryFloatFunction(toBinaryFloatFunctionContext);

                case ToDateFunctionContext toDateFunctionContext:
                    return VisitToDateFunction(toDateFunctionContext);

                case ToDsintervalFunctionContext toDsintervalFunctionContext:
                    return VisitToDsintervalFunction(toDsintervalFunctionContext);

                case ToNumberFunctionContext toNumberFunctionContext:
                    return VisitToNumberFunction(toNumberFunctionContext);

                case ToTimestampFunctionContext toTimestampFunctionContext:
                    return VisitToTimestampFunction(toTimestampFunctionContext);

                case ToTimestampTzFunctionContext toTimestampTzFunctionContext:
                    return VisitToTimestampTzFunction(toTimestampTzFunctionContext);

                case ToYmintervalFunctionContext toYmintervalFunctionContext:
                    return VisitToYmintervalFunction(toYmintervalFunctionContext);

                case TranslateUsingFunctionContext translateUsingFunctionContext:
                    return VisitTranslateUsingFunction(translateUsingFunctionContext);

                case TreatFunctionContext treatFunctionContext:
                    return VisitTreatFunction(treatFunctionContext);

                case TrimFunctionContext trimFunctionContext:
                    return VisitTrimFunction(trimFunctionContext);

                case TzOffsetFunctionContext tzOffsetFunctionContext:
                    return VisitTzOffsetFunction(tzOffsetFunctionContext);

                case UidFunctionContext uidFunctionContext:
                    return VisitUidFunction(uidFunctionContext);

                case UserFunctionContext userFunctionContext:
                    return VisitUserFunction(userFunctionContext);

                case ValidateConversionFunctionContext validateConversionFunctionContext:
                    return VisitValidateConversionFunction(validateConversionFunctionContext);

                case XmlaggFunctionContext xmlaggFunctionContext:
                    return VisitXmlaggFunction(xmlaggFunctionContext);

                case XmlcastFunctionContext xmlcastFunctionContext:
                    return VisitXmlcastFunction(xmlcastFunctionContext);

                case XmlcorattvalFunctionContext xmlcorattvalFunctionContext:
                    return VisitXmlcorattvalFunction(xmlcorattvalFunctionContext);

                case XmlelementFunctionContext xmlelementFunctionContext:
                    return VisitXmlelementFunction(xmlelementFunctionContext);

                case XmlCdataFunctionContext xmlCdataFunctionContext:
                    return VisitXmlCdataFunction(xmlCdataFunctionContext);

                case XmlexistsFunctionContext xmlexistsFunctionContext:
                    return VisitXmlexistsFunction(xmlexistsFunctionContext);

                case XmlforestFunctionContext xmlforestFunctionContext:
                    return VisitXmlforestFunction(xmlforestFunctionContext);

                case XmlparseFunctionContext xmlparseFunctionContext:
                    return VisitXmlparseFunction(xmlparseFunctionContext);

                case XmlpiFunctionContext xmlpiFunctionContext:
                    return VisitXmlpiFunction(xmlpiFunctionContext);

                case XmlqueryFunctionContext xmlqueryFunctionContext:
                    return VisitXmlqueryFunction(xmlqueryFunctionContext);

                case XmlrootFunctionContext xmlrootFunctionContext:
                    return VisitXmlrootFunction(xmlrootFunctionContext);

                case XmlsequenceFunctionContext xmlsequenceFunctionContext:
                    return VisitXmlsequenceFunction(xmlsequenceFunctionContext);

                case XmlserializeFunctionContext xmlserializeFunctionContext:
                    return VisitXmlserializeFunction(xmlserializeFunctionContext);

                case XmlTableFunctionContext xmlTableFunctionContext:
                    return VisitXmlTableFunction(xmlTableFunctionContext);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }

        #region Known Functions
        public static QsiInvokeExpressionNode VisitCastFunction(CastFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CAST().GetText());

            if (context.castExpr is not null)
            {
                node.Parameters.Add(VisitExpr(context.castExpr));
            }
            else
            {
                var tableExpressionNode = OracleTree.CreateWithSpan<QsiTableExpressionNode>(context.castSubquery);
                tableExpressionNode.Table.Value = TableVisitor.VisitSubquery(context.castSubquery);

                node.Parameters.Add(tableExpressionNode);
            }

            node.Parameters.Add(VisitDataType(context.datatype()));

            if (context.defaultValue is not null)
            {
                var defaultValueNode = OracleTreeHelper.CreateNamedParameter(context.defaultValue, "defaultValue");
                defaultValueNode.Expression.Value = VisitExpr(context.defaultValue);

                node.Parameters.Add(defaultValueNode);
            }

            if (context.fmt is not null)
            {
                var fmtNode = OracleTreeHelper.CreateNamedParameter(context.fmt, "fmt");
                fmtNode.Expression.Value = VisitExpr(context.fmt);

                node.Parameters.Add(fmtNode);
            }

            if (context.nlsparam is not null)
            {
                var nlsparamNode = OracleTreeHelper.CreateNamedParameter(context.nlsparam, "nlsparam");
                nlsparamNode.Expression.Value = VisitExpr(context.nlsparam);

                node.Parameters.Add(nlsparamNode);
            }

            return node;
        }

        public static QsiExpressionNode VisitDataType(DatatypeContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiTypeExpressionNode>(context);

            node.Identifier = new QsiQualifiedIdentifier(
                new QsiIdentifier(context.GetInputText(), false)
            );

            return node;
        }

        public static QsiInvokeExpressionNode VisitApproxCountFunction(ApproxCountFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_COUNT().GetText());

            if (context.HasToken(MULT_SYMBOL))
            {
                var paramNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
                paramNode.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.MULT_SYMBOL().GetText(), false));

                node.Parameters.Add(paramNode);
            }
            else
            {
                node.Parameters.Add(VisitExpr(context.expr()));
            }

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static QsiInvokeExpressionNode VisitApproxMedianFunction(ApproxMedianFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_MEDIAN().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.DETERMINISTIC() is not null)
            {
                var deterministicNode = OracleTree.CreateWithSpan<QsiExpressionFragmentNode>(context);
                deterministicNode.Text = context.DETERMINISTIC().GetText();

                node.Parameters.Add(deterministicNode);
            }

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static QsiInvokeExpressionNode VisitApproxPercentileFunction(ApproxPercentileFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_PERCENTILE().GetText());

            node.Parameters.Add(VisitExpr(context.expr(0)));

            if (context.DETERMINISTIC() is not null)
            {
                var deterministicNode = OracleTree.CreateWithSpan<QsiExpressionFragmentNode>(context);
                deterministicNode.Text = context.DETERMINISTIC().GetText();

                node.Parameters.Add(deterministicNode);
            }

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            var orderByNode = OracleTree.CreateWithSpan<OracleOrderExpressionNode>(context);
            orderByNode.Expression.Value = VisitExpr(context.expr(1));

            if (context.HasToken(DESC))
                orderByNode.Order = QsiSortOrder.Descending;
            else if (context.HasToken(ASC))
                orderByNode.Order = QsiSortOrder.Ascending;

            node.Parameters.Add(orderByNode);

            return node;
        }

        public static QsiInvokeExpressionNode VisitApproxPercentileDetailFunction(ApproxPercentileDetailFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_PERCENTILE_DETAIL().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.DETERMINISTIC() is not null)
            {
                var deterministicNode = OracleTree.CreateWithSpan<QsiExpressionFragmentNode>(context);
                deterministicNode.Text = context.DETERMINISTIC().GetText();

                node.Parameters.Add(deterministicNode);
            }

            return node;
        }

        public static QsiInvokeExpressionNode VisitApproxRankFunction(ApproxRankFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_RANK().GetText());

            node.Parameters.Add(VisitExpr(context.rankExpr));

            if (context.partitionName() is not null)
            {
                var partitionNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context.partitionName());
                partitionNode.Identifier = new QsiQualifiedIdentifier(IdentifierVisitor.VisitIdentifier(context.partitionName().identifier()));

                node.Parameters.Add(partitionNode);
            }

            if (context.orderExpr is not null)
                node.Parameters.Add(VisitExpr(context.orderExpr));

            return node;
        }

        public static QsiInvokeExpressionNode VisitApproxSumFunction(ApproxSumFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_SUM().GetText());

            if (context.HasToken(MULT_SYMBOL))
            {
                var paramNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
                paramNode.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(context.MULT_SYMBOL().GetText(), false));

                node.Parameters.Add(paramNode);
            }
            else
            {
                node.Parameters.Add(VisitExpr(context.expr()));
            }

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static QsiInvokeExpressionNode VisitBinToNumFunction(BinToNumFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.BIN_TO_NUM().GetText());

            node.Parameters.Add(VisitExpressionList(context.expressionList()));

            node.Parameters.Add(VisitInsertIntoClause(context.insertIntoClause()));

            return node;
        }

        public static OracleAggregateFunctionExpressionNode VisitFirstFunction(FirstFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);
            node.Function.Value = VisitCommonFunction(context, context.functionName(), context.argumentList());

            if (context.orderByClause() is not null)
                node.Order.Value = VisitOrderByClause(context.orderByClause());

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            return node;
        }

        public static QsiInvokeExpressionNode VisitChrFunction(ChrFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.USING() is not null)
            {
                var usingNode = OracleTree.CreateWithSpan<OracleNamedParameterExpressionNode>(context);
                var fragment = TreeHelper.Fragment(context.USING().GetText(), context.NCHAR_CS().GetText());

                usingNode.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(fragment.Text, false));
                usingNode.Expression.Value = fragment;

                node.Parameters.Add(usingNode);
            }

            return node;
        }

        public static QsiInvokeExpressionNode VisitClusterDetailsFunction(ClusterDetailsFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterDetailsAnalyticFunction(ClusterDetailsAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterDistanceFunction(ClusterDistanceFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterIdFunction(ClusterIdFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterIdAnalyticFunction(ClusterIdAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterProbabilityFunction(ClusterProbabilityFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterProbAnalyticFunction(ClusterProbAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterSetFunction(ClusterSetFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitClusterSetAnalyticFunction(ClusterSetAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCollectFunction(CollectFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitConnectByRootFunction(ConnectByRootFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CONNECT_BY_ROOT().GetText());

            var columnNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(context);
            columnNode.Column.Value = IdentifierVisitor.VisitColumn(context.column());

            node.Parameters.Add(columnNode);

            return node;
        }

        public static QsiInvokeExpressionNode VisitCorrelationFunction(CorrelationFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCubeTableFunction(CubeTableFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCumeDistFunction(CumeDistFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCumeDistAnalyticFunction(CumeDistAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCurrentDateFunction(CurrentDateFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCurrentTimestampFunction(CurrentTimestampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitDbTimeZoneFunction(DbTimeZoneFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitDenseRankAggregateFunction(DenseRankAggregateFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitDenseRankAnalyticFunction(DenseRankAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitExtractDateTimeFunction(ExtractDateTimeFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureCompareFunction(FeatureCompareFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureDetailsFunction(FeatureDetailsFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureIdFunction(FeatureIdFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureIdAnalyticFunction(FeatureIdAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureSetFunction(FeatureSetFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureSetAnalyticFunction(FeatureSetAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureValueFunction(FeatureValueFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFeatureValueAnalyticFunction(FeatureValueAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitIterationNumberFunction(IterationNumberFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonArrayFunction(JsonArrayFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonArrayAggFunction(JsonArrayAggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonMergePatchFunction(JsonMergePatchFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonObjectFunction(JsonObjectFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonObjectaggFunction(JsonObjectaggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonQueryFunction(JsonQueryFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonScalarFunction(JsonScalarFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonSerializeFunction(JsonSerializeFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonTableFunction(JsonTableFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonTransformFunction(JsonTransformFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitJsonValueFunction(JsonValueFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitLagFunction(LagFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitLastFunction(LastFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitLeadFunction(LeadFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitListaggFunction(ListaggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitLocaltimestampFunction(LocaltimestampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitNtileFunction(NtileFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitOraDmPartitionNameFunction(OraDmPartitionNameFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitOraInvokingUserFunction(OraInvokingUserFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitOraInvokingUserIdFunction(OraInvokingUserIdFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPercentRankAggregateFunction(PercentRankAggregateFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPercentRankAnalyticFunction(PercentRankAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPercentileContFunction(PercentileContFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPercentileDiscFunction(PercentileDiscFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionFunction(PredictionFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionOrderedFunction(PredictionOrderedFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionAnalyticFunction(PredictionAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionBoundsFunction(PredictionBoundsFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionCostFunction(PredictionCostFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionCostAnalyticFunction(PredictionCostAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionDetailsFunction(PredictionDetailsFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionDetailsAnalyticFunction(PredictionDetailsAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionProbabilityFunction(PredictionProbabilityFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionProbabilityOrderedFunction(PredictionProbabilityOrderedFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionProbAnalyticFunction(PredictionProbAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionSetFunction(PredictionSetFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionSetOrderedFunction(PredictionSetOrderedFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitPredictionSetAnalyticFunction(PredictionSetAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitRankAggregateFunction(RankAggregateFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitRankAnalyticFunction(RankAnalyticFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitRatioToReportFunction(RatioToReportFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitSessiontimezoneFunction(SessiontimezoneFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitRowNumberFunction(RowNumberFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitSkewnessPopFunction(SkewnessPopFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitSkewnessSampFunction(SkewnessSampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitSysDburigenFunction(SysDburigenFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitSysdateFunction(SysdateFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitSystimestampFunction(SystimestampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToBinaryDoubleFunction(ToBinaryDoubleFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToBinaryFloatFunction(ToBinaryFloatFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToDateFunction(ToDateFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToDsintervalFunction(ToDsintervalFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToNumberFunction(ToNumberFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToTimestampFunction(ToTimestampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToTimestampTzFunction(ToTimestampTzFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitToYmintervalFunction(ToYmintervalFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitTranslateUsingFunction(TranslateUsingFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitTreatFunction(TreatFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitTrimFunction(TrimFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitTzOffsetFunction(TzOffsetFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitUidFunction(UidFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitUserFunction(UserFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitValidateConversionFunction(ValidateConversionFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlaggFunction(XmlaggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlcastFunction(XmlcastFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlcorattvalFunction(XmlcorattvalFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlelementFunction(XmlelementFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlCdataFunction(XmlCdataFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlexistsFunction(XmlexistsFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlforestFunction(XmlforestFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlparseFunction(XmlparseFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlpiFunction(XmlpiFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlqueryFunction(XmlqueryFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlrootFunction(XmlrootFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlsequenceFunction(XmlsequenceFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlserializeFunction(XmlserializeFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitXmlTableFunction(XmlTableFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }
        #endregion

        #region Analytic Functions
        public static QsiInvokeExpressionNode VisitAnyValueFunction(AnyValueFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitAvgFunction(AvgFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitBitAndAggFunction(BitAndAggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitBitOrAggFunction(BitOrAggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitBitXorAggFunction(BitXorAggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitChecksumFunction(ChecksumFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCorrFunction(CorrFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCountFunction(CountFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCovarPopFunction(CovarPopFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitCovarSampFunction(CovarSampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitFirstValueFunction(FirstValueFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitKurtosisPopFunction(KurtosisPopFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitKurtosisSampFunction(KurtosisSampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitLastValueFunction(LastValueFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitMaxFunction(MaxFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitMedianFunction(MedianFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitMinFunction(MinFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitNthValueFunction(NthValueFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitLinearRegrFunction(LinearRegrFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitStddevFunction(StddevFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitStddevPopFunction(StddevPopFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitStddevSampFunction(StddevSampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitSumFunction(SumFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitVarPopFunction(VarPopFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitVarSampFunction(VarSampFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static QsiInvokeExpressionNode VisitVarianceFunction(VarianceFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }
        #endregion

        public static QsiExpressionNode VisitWindowingClause(WindowingClauseContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OraclePartitionExpressionNode VisitQueryPartitionClause(QueryPartitionClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OraclePartitionExpressionNode>(context);

            node.Elements.AddRange(context.queryPartitionExpressions().expr().Select(VisitExpr));

            return node;
        }

        public static OraclePartitionExpressionNode VisitPartitionExtensionClause(PartitionExtensionClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OraclePartitionExpressionNode>(context);

            node.IsSubpartition = context.HasToken(SUBPARTITION);

            if (node.IsSubpartition)
            {
                node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.subpartition().identifier());

                node.Elements.AddRange(
                    context.subpartitionKeyValue().Select(c => VisitExpr(c.expr())
                    ));
            }
            else
            {
                node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.partition().identifier());

                node.Elements.AddRange(
                    context.partitionKeyValue().Select(c => VisitExpr(c.expr())
                    ));
            }

            return node;
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
            var node = OracleTree.CreateWithSpan<OracleTableExpressionNode>(context);
            node.IsCursor = true;

            node.Table.Value = TableVisitor.VisitSubquery(context.subquery());

            return node;
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
            var node = OracleTree.CreateWithSpan<OracleTableExpressionNode>(context);

            node.Table.Value = TableVisitor.VisitSubquery(context.subquery());

            return node;
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
                    return VisitIntevalLiteral(intervalLiteral);

                case DateTimeLiteralContext dateTimeLiteral:
                    return VisitDateTimeLiteral(dateTimeLiteral);

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

        public static QsiLiteralExpressionNode VisitIntevalLiteral(IntervalLiteralContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            // WARNING: Interval
            node.Type = QsiDataType.DateTime;
            node.Value = context.GetText();

            return node;
        }

        public static QsiLiteralExpressionNode VisitDateTimeLiteral(DateTimeLiteralContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            // WARNING: Interval
            node.Type = QsiDataType.DateTime;
            node.Value = context.GetInputText();

            return node;
        }

        public static QsiLiteralExpressionNode VisitInteger(IntegerContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiLiteralExpressionNode>(context);
            node.Type = QsiDataType.Numeric;
            node.Value = int.Parse(context.GetInputText());

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
                ComparisonConditionContext comparisonCondition => VisitGroupComparisonCondition(comparisonCondition.groupComparisonCondition()),
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
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.l);
            node.Operator = context.operator1().GetText();
            node.Right.Value = VisitExpr(context.r);

            return node;
        }

        public static QsiExpressionNode VisitSimpleComparisonCondition2(SimpleComparisonCondition2Context context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
            node.Left.Value = VisitExpressionList(context.expressionList(0));

            if (context.subquery() is not null)
                node.Right.Value = VisitSubquery(context.subquery());
            else
                node.Right.Value = VisitExpressionList(context.expressionList(1));

            node.Operator = context.operator2().GetText();

            return node;
        }

        public static QsiExpressionNode VisitGroupComparisonCondition(GroupComparisonConditionContext context)
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

        public static OracleHierarchiesExpressionNode VisitHierarchiesClause(HierarchiesClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleHierarchiesExpressionNode>(context);
            node.Identifiers.AddRange(VisitHierIds(context.hierIds()));

            return node;
        }

        public static IEnumerable<QsiQualifiedIdentifier> VisitHierIds(HierIdsContext context)
        {
            foreach (var hierId in context.hierId())
            {
                if (hierId.HasToken(MEASURES))
                    yield return new QsiQualifiedIdentifier(new QsiIdentifier("MEASURES", false));
                else
                    yield return IdentifierVisitor.CreateQualifiedIdentifier(hierId.identifier());
            }
        }

        public static OracleLimitExpressionNode VisitRowlimitingContexts(RowOffsetContext rowOffset, RowFetchOptionContext rowFetchOption)
        {
            OracleLimitExpressionNode node;

            if (rowOffset is not null && rowFetchOption is null)
                node = OracleTree.CreateWithSpan<OracleLimitExpressionNode>(rowOffset);
            else if (rowOffset is null && rowFetchOption is not null)
                node = OracleTree.CreateWithSpan<OracleLimitExpressionNode>(rowFetchOption);
            else
                node = OracleTree.CreateWithSpan<OracleLimitExpressionNode>(rowOffset!.Start, rowFetchOption!.Stop);

            if (rowOffset is not null)
            {
                node.Offset.Value = VisitExpr(rowOffset.offset);
            }

            if (rowFetchOption is not null)
            {
                if (rowFetchOption.HasToken(PERCENT))
                    node.LimitPercent.Value = VisitExpr(rowFetchOption.percent);
                else
                    node.Limit.Value = VisitExpr(rowFetchOption.rowcount);

                node.FetchOption = rowFetchOption.HasToken(ONLY) ? OracleFetchOption.Only : OracleFetchOption.WithTies;
            }

            return node;
        }

        public static OracleMultipleOrderExpressionNode VisitOrderByClause(OrderByClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleMultipleOrderExpressionNode>(context);

            node.IsSiblings = context.HasToken(SIBLINGS);
            node.Orders.AddRange(context._items.Select(VisitOrderByItem));

            return node;
        }

        public static QsiOrderExpressionNode VisitOrderByItem(OrderByItemContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleOrderExpressionNode>(context);
            node.Expression.Value = VisitExpr(context.expr());

            if (context.order != null)
                node.Order = context.order.Type == DESC ? QsiSortOrder.Descending : QsiSortOrder.Ascending;

            if (context.nullsOrder != null)
                node.NullsOrder = context.nullsOrder.Type == FIRST
                    ? OracleNullsOrder.First
                    : OracleNullsOrder.Last;

            return node;
        }

        public static QsiWhereExpressionNode VisitWhereClause(WhereClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiWhereExpressionNode>(context);
            node.Expression.Value = VisitCondition(context.condition());

            return node;
        }

        public static QsiGroupingExpressionNode VisitGroupByClause(GroupByClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiGroupingExpressionNode>(context);
            node.Items.AddRange(context.groupByItems().groupByItem().Select(VisitGroupByItem));
            var groupingByHavingClause = context.groupByHavingClause();

            if (groupingByHavingClause is not null)
                node.Having.Value = VisitCondition(groupingByHavingClause.condition());

            return node;
        }

        public static QsiExpressionNode VisitGroupByItem(GroupByItemContext context)
        {
            return context.children[0] switch
            {
                ExprContext expr => VisitExpr(expr),
                RollupCubeClauseContext rollupCubeClause => throw new NotImplementedException(),
                GroupingSetsClauseContext groupingSetsClause => throw new NotImplementedException(),
                _ => throw new NotSupportedException()
            };
        }

        public static QsiTableExpressionNode VisitSubquery(SubqueryContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiTableExpressionNode>(context);
            node.Table.Value = TableVisitor.VisitSubquery(context);

            return node;
        }

        public static QsiTableExpressionNode VisitInsertIntoClause(InsertIntoClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiTableExpressionNode>(context);

            var tableNode = OracleTree.CreateWithSpan<QsiDerivedTableNode>(context);

            tableNode.Source.Value = TableVisitor.VisitDmlTableExpressionClause(context.dmlTableExpressionClause());

            if (context.tAlias() is not null)
                tableNode.Alias.Value = IdentifierVisitor.VisitAlias(context.tAlias());

            if (context.columnList() is null)
            {
                tableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            }
            else
            {
                tableNode.Columns.Value = new QsiColumnsDeclarationNode();
                tableNode.Columns.Value.Columns.AddRange(IdentifierVisitor.VisitColumnList(context.columnList()));
            }

            node.Table.Value = tableNode;

            return node;
        }

        public static QsiInvokeExpressionNode VisitCommonFunction(ParserRuleContext context, FunctionNameContext functionNameContext, ArgumentListContext argumentListContext)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            var functionName = IdentifierVisitor.CreateQualifiedIdentifier(functionNameContext.identifier());

            var functionExpressionNode = OracleTree.CreateWithSpan<QsiFunctionExpressionNode>(context);
            functionExpressionNode.Identifier = functionName;

            node.Member.Value = functionExpressionNode;

            IEnumerable<QsiExpressionNode> argumentList = VisitArgumentList(argumentListContext);

            if (argumentList is not null)
                node.Parameters.AddRange(argumentList);

            return node;
        }
    }
}
