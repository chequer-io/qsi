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

        public static IEnumerable<QsiColumnNode> VisitSelectList(MiningAttributeClauseContext context)
        {
            return VisitSelectList(context.selectList());
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
        public static OracleInvokeExpressionNode VisitCastFunction(CastFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
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

        public static OracleInvokeExpressionNode VisitApproxCountFunction(ApproxCountFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_COUNT().GetText());

            if (context.HasToken(MULT_SYMBOL))
            {
                var paramNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(context);
                paramNode.Column.Value = OracleTree.CreateWithSpan<QsiAllColumnNode>(context);

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

        public static OracleInvokeExpressionNode VisitApproxMedianFunction(ApproxMedianFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_MEDIAN().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.DETERMINISTIC() is not null)
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DETERMINISTIC"));

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitApproxPercentileFunction(ApproxPercentileFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_PERCENTILE().GetText());

            node.Parameters.Add(VisitExpr(context.expr(0)));

            if (context.DETERMINISTIC() is not null)
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DETERMINISTIC"));

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

        public static OracleInvokeExpressionNode VisitApproxPercentileDetailFunction(ApproxPercentileDetailFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_PERCENTILE_DETAIL().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.DETERMINISTIC() is not null)
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DETERMINISTIC"));

            return node;
        }

        public static OracleInvokeExpressionNode VisitApproxRankFunction(ApproxRankFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
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

        public static OracleInvokeExpressionNode VisitApproxSumFunction(ApproxSumFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.APPROX_SUM().GetText());

            if (context.HasToken(MULT_SYMBOL))
            {
                var paramNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(context);
                paramNode.Column.Value = OracleTree.CreateWithSpan<QsiAllColumnNode>(context);

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

        public static OracleInvokeExpressionNode VisitBinToNumFunction(BinToNumFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
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

        public static OracleInvokeExpressionNode VisitChrFunction(ChrFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CHR().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.USING() is not null)
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("USING NCHAR_CS"));

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterDetailsFunction(ClusterDetailsFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_DETAILS().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.clusterId() is not null)
            {
                var clusterIdNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
                clusterIdNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.clusterId().identifier());

                node.Parameters.Add(clusterIdNode);
            }

            if (context.topN is not null)
                node.Parameters.Add(VisitExpr(context.topN));

            if (context.HasToken(DESC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DESC"));
            else if (context.HasToken(ASC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ASC"));
            else if (context.HasToken(ABS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ABS"));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterDetailsAnalyticFunction(ClusterDetailsAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_DETAILS().GetText());

            node.Parameters.Add(VisitNumberLiteral(context.numberLiteral()));

            if (context.clusterId() is not null)
            {
                var clusterIdNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
                clusterIdNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.clusterId().identifier());

                node.Parameters.Add(clusterIdNode);
            }

            if (context.topN is not null)
                node.Parameters.Add(VisitExpr(context.topN));

            if (context.HasToken(DESC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DESC"));
            else if (context.HasToken(ASC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ASC"));
            else if (context.HasToken(ABS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ABS"));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterDistanceFunction(ClusterDistanceFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_DISTANCE().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(new[]
                {
                    context.schema()?.identifier(),
                    context.model().identifier(),
                    context.identifier()
                }
                .Where(c => c is not null)
                .ToArray()
            );

            node.Parameters.Add(modelNode);

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterIdFunction(ClusterIdFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_ID().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterIdAnalyticFunction(ClusterIdAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_ID().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterProbabilityFunction(ClusterProbabilityFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_PROBABILITY().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(new[]
                {
                    context.schema()?.identifier(),
                    context.model().identifier(),
                    context.identifier()
                }
                .Where(c => c is not null)
                .ToArray()
            );

            node.Parameters.Add(modelNode);

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterProbAnalyticFunction(ClusterProbAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_PROBABILITY().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.identifier() is not null)
            {
                var identNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
                identNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.identifier());

                node.Parameters.Add(identNode);
            }

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterSetFunction(ClusterSetFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_SET().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.topN is not null)
                node.Parameters.Add(VisitExpr(context.topN));

            if (context.cutoff is not null)
                node.Parameters.Add(VisitExpr(context.cutoff));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitClusterSetAnalyticFunction(ClusterSetAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CLUSTER_SET().GetText());

            node.Parameters.Add(VisitExpr(context.intoExpr));

            if (context.topN is not null)
                node.Parameters.Add(VisitExpr(context.topN));

            if (context.cutoff is not null)
                node.Parameters.Add(VisitExpr(context.cutoff));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static QsiExpressionNode VisitCollectFunction(CollectFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.COLLECT().GetText());

            if (context.HasToken(DISTINCT))
                invokeNode.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(UNIQUE))
                invokeNode.QueryBehavior = OracleQueryBehavior.Unique;

            var columnNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(context);
            columnNode.Column.Value = IdentifierVisitor.VisitColumn(context.column());

            invokeNode.Parameters.Add(columnNode);

            node.Function.Value = invokeNode;

            if (context.orderByClause() is not null)
                node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleInvokeExpressionNode VisitConnectByRootFunction(ConnectByRootFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CONNECT_BY_ROOT().GetText());

            var columnNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(context);
            columnNode.Column.Value = IdentifierVisitor.VisitColumn(context.column());

            node.Parameters.Add(columnNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitCorrelationFunction(CorrelationFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(context.HasToken(CORR_K)
                ? context.CORR_K().GetText()
                : context.CORR_S().GetText());

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            if (context.HasToken(COEFFICIENT))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("COEFFICIENT"));
            else if (context.HasToken(ONE_SIDED_SIG))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ONE_SIDED_SIG"));
            else if (context.HasToken(ONE_SIDED_SIG_POS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ONE_SIDED_SIG_POS"));
            else if (context.HasToken(ONE_SIDED_SIG_NEG))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ONE_SIDED_SIG_NEG"));
            else if (context.HasToken(TWO_SIDED_SIG))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("TWO_SIDED_SIG"));

            return node;
        }

        public static OracleInvokeExpressionNode VisitCubeTableFunction(CubeTableFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CUBE_TABLE().GetText());

            node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitCumeDistFunction(CumeDistFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.CUME_DIST().GetText());

            invokeNode.Parameters.AddRange(context.expr().Select(VisitExpr));

            node.Function.Value = invokeNode;
            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return invokeNode;
        }

        public static OracleInvokeExpressionNode VisitCumeDistAnalyticFunction(CumeDistAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.CUME_DIST().GetText());

            node.Function.Value = invokeNode;

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return invokeNode;
        }

        public static OracleInvokeExpressionNode VisitCurrentDateFunction(CurrentDateFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CURRENT_DATE().GetText());

            return node;
        }

        public static OracleInvokeExpressionNode VisitCurrentTimestampFunction(CurrentTimestampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CURRENT_TIMESTAMP().GetText());

            if (context.precision() is not null)
                node.Parameters.Add(new QsiLiteralExpressionNode
                {
                    Type = QsiDataType.Numeric,
                    Value = long.Parse(context.precision().GetInputText())
                });

            return node;
        }

        public static OracleInvokeExpressionNode VisitDbTimeZoneFunction(DbTimeZoneFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.DBTIMEZONE().GetText());

            return node;
        }

        public static OracleInvokeExpressionNode VisitDenseRankAggregateFunction(DenseRankAggregateFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.DENSE_RANK().GetText());

            invokeNode.Parameters.AddRange(context.expr().Select(VisitExpr));

            node.Function.Value = invokeNode;
            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return invokeNode;
        }

        public static OracleInvokeExpressionNode VisitDenseRankAnalyticFunction(DenseRankAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.DENSE_RANK().GetText());

            node.Function.Value = invokeNode;

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return invokeNode;
        }

        public static OracleInvokeExpressionNode VisitExtractDateTimeFunction(ExtractDateTimeFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.EXTRACT().GetText());

            node.Parameters.Add(TreeHelper.CreateConstantLiteral(context.children[2].GetText()));

            node.Parameters.Add(VisitExpressionList(context.expressionList()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureCompareFunction(FeatureCompareFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_COMPARE().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            foreach (var miningAttributeClauseContext in context.miningAttributeClause())
            {
                var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(miningAttributeClauseContext);
                miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(miningAttributeClauseContext);

                foreach (var columnNode in VisitSelectList(miningAttributeClauseContext))
                    miningNode.Columns.Value.Columns.Add(columnNode);

                node.Parameters.Add(miningNode);
            }

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureDetailsFunction(FeatureDetailsFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_DETAILS().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            if (context.HasToken(DESC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DESC"));
            else if (context.HasToken(ASC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ASC"));
            else if (context.HasToken(ABS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ABS"));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureIdFunction(FeatureIdFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_ID().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureIdAnalyticFunction(FeatureIdAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_ID().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
            modelNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.identifier());

            node.Parameters.Add(modelNode);

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureSetFunction(FeatureSetFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_SET().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureSetAnalyticFunction(FeatureSetAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_SET().GetText());

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureValueFunction(FeatureValueFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_VALUE().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            node.Parameters.Add(VisitExpr(context.expr()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitFeatureValueAnalyticFunction(FeatureValueAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FEATURE_VALUE().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.featureId() is not null)
            {
                var featureIdNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
                featureIdNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.featureId().identifier());

                node.Parameters.Add(featureIdNode);
            }

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitIterationNumberFunction(IterationNumberFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.ITERATION_NUMBER().GetText());

            return node;
        }

        public static OracleAnalyticFunctionExpressionNode VisitLagFunction(LagFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);
            var functionNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);

            functionNode.Member.Value = TreeHelper.CreateFunction(context.LAG().GetText());

            functionNode.Parameters.AddRange(context.expr().Select(VisitExpr));

            // skip nulls

            node.Function.Value = functionNode;

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleAggregateFunctionExpressionNode VisitLastFunction(LastFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);
            node.Function.Value = VisitCommonFunction(context, context.functionName(), context.argumentList());

            if (context.orderByClause() is not null)
                node.Order.Value = VisitOrderByClause(context.orderByClause());

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            return node;
        }

        public static OracleAnalyticFunctionExpressionNode VisitLeadFunction(LeadFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);
            var functionNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);

            functionNode.Member.Value = TreeHelper.CreateFunction(context.LEAD().GetText());

            functionNode.Parameters.AddRange(context.expr().Select(VisitExpr));

            // skip nulls

            node.Function.Value = functionNode;

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleInvokeExpressionNode VisitListaggFunction(ListaggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitLocaltimestampFunction(LocaltimestampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.LOCALTIMESTAMP().GetText());

            if (context.expr() is not null)
                node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleAggregateFunctionExpressionNode VisitNtileFunction(NtileFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);

            var functionNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            functionNode.Member.Value = TreeHelper.CreateFunction(context.NTILE().GetText());

            functionNode.Parameters.Add(VisitExpr(context.expr()));

            node.Function.Value = functionNode;

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleInvokeExpressionNode VisitOraDmPartitionNameFunction(OraDmPartitionNameFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.ORA_DM_PARTITION_NAME().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitOraInvokingUserFunction(OraInvokingUserFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.ORA_INVOKING_USER().GetText());

            return node;
        }

        public static OracleInvokeExpressionNode VisitOraInvokingUserIdFunction(OraInvokingUserIdFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.ORA_INVOKING_USERID().GetText());

            return node;
        }

        public static OracleAggregateFunctionExpressionNode VisitPercentRankAggregateFunction(PercentRankAggregateFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);
            var functionNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            functionNode.Member.Value = TreeHelper.CreateFunction(context.PERCENT_RANK().GetText());

            functionNode.Parameters.AddRange(context.expr().Select(VisitExpr));

            if (context.orderByClause() is not null)
                node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleAnalyticFunctionExpressionNode VisitPercentRankAnalyticFunction(PercentRankAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);
            var functionNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            functionNode.Member.Value = TreeHelper.CreateFunction(context.PERCENT_RANK().GetText());

            if (context.orderByClause() is not null)
                node.Order.Value = VisitOrderByClause(context.orderByClause());

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            return node;
        }

        public static OracleAggregateFunctionExpressionNode VisitPercentileContFunction(PercentileContFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);
            var functionNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            functionNode.Member.Value = TreeHelper.CreateFunction(context.PERCENTILE_CONT().GetText());

            functionNode.Parameters.Add(VisitExpr(context.expr()));

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            return node;
        }

        public static OracleAggregateFunctionExpressionNode VisitPercentileDiscFunction(PercentileDiscFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);
            var functionNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            functionNode.Member.Value = TreeHelper.CreateFunction(context.PERCENTILE_DISC().GetText());

            functionNode.Parameters.Add(VisitExpr(context.expr()));

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionFunction(PredictionFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionOrderedFunction(PredictionOrderedFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            node.Parameters.AddRange(context.orderByClause().Select(VisitOrderByClause));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionAnalyticFunction(PredictionAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION().GetText());

            node.Parameters.Add(context.expr() is not null
                ? VisitExpr(context.expr())
                : TreeHelper.CreateConstantLiteral("OF ANOMALY")
            );

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionBoundsFunction(PredictionBoundsFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_BOUNDS().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.expr() is not null)
                node.Parameters.AddRange(context.expr().Select(VisitExpr));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionCostFunction(PredictionCostFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_COST().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.expr() is not null)
                node.Parameters.Add(VisitExpr(context.expr()));

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            node.Parameters.AddRange(context.orderByClause().Select(VisitOrderByClause));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionCostAnalyticFunction(PredictionCostAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_COST().GetText());

            node.Parameters.Add(context.expr() is not null
                ? VisitExpr(context.expr())
                : TreeHelper.CreateConstantLiteral("OF ANOMALY")
            );

            if (context.identifier() is not null)
            {
                var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);
                modelNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.identifier());

                node.Parameters.Add(modelNode);
            }

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionDetailsFunction(PredictionDetailsFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_DETAILS().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.expr() is not null)
                node.Parameters.AddRange(context.expr().Select(VisitExpr));

            if (context.HasToken(DESC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DESC"));
            else if (context.HasToken(ASC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ASC"));
            else if (context.HasToken(ABS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ABS"));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionDetailsAnalyticFunction(PredictionDetailsAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_DETAILS().GetText());

            if (context.HasToken(ANOMALY))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("OF ANOMALY"));

            if (context.expr() is not null)
                node.Parameters.AddRange(context.expr().Select(VisitExpr));

            if (context.HasToken(DESC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DESC"));
            else if (context.HasToken(ASC))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ASC"));
            else if (context.HasToken(ABS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("ABS"));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionProbabilityFunction(PredictionProbabilityFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_PROBABILITY().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.expr() is not null)
                node.Parameters.Add(VisitExpr(context.expr()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionProbabilityOrderedFunction(PredictionProbabilityOrderedFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_PROBABILITY().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.expr() is not null)
                node.Parameters.Add(VisitExpr(context.expr()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            node.Parameters.AddRange(context.orderByClause().Select(VisitOrderByClause));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionProbAnalyticFunction(PredictionProbAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_PROBABILITY().GetText());

            if (context.HasToken(ANOMALY))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("OF ANOMALY"));

            if (context.expr() is not null)
                node.Parameters.AddRange(context.expr().Select(VisitExpr));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionSetFunction(PredictionSetFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_SET().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.identifier() is not null)
            {
                foreach (var identifierContext in context.identifier())
                {
                    var identNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

                    identNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(identifierContext);

                    node.Parameters.Add(identNode);
                }
            }

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionSetOrderedFunction(PredictionSetOrderedFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_SET().GetText());

            var modelNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

            modelNode.Identifier = context.schema() is not null
                ? IdentifierVisitor.CreateQualifiedIdentifier(context.schema().identifier(), context.model().identifier())
                : IdentifierVisitor.CreateQualifiedIdentifier(context.model().identifier());

            node.Parameters.Add(modelNode);

            if (context.identifier() is not null)
            {
                foreach (var identifierContext in context.identifier())
                {
                    var identNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

                    identNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(identifierContext);

                    node.Parameters.Add(identNode);
                }
            }

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            node.Parameters.AddRange(context.orderByClause().Select(VisitOrderByClause));

            return node;
        }

        public static OracleInvokeExpressionNode VisitPredictionSetAnalyticFunction(PredictionSetAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.PREDICTION_SET().GetText());

            node.Parameters.Add(context.stringLiteral() is not null
                ? VisitStringLiteral(context.stringLiteral())
                : TreeHelper.CreateConstantLiteral("OF ANOMALY")
            );

            if (context.identifier() is not null)
            {
                foreach (var identifierContext in context.identifier())
                {
                    var identNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(context);

                    identNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(identifierContext);

                    node.Parameters.Add(identNode);
                }
            }

            if (context.costMatrixClause() is not null)
                node.Parameters.Add(VisitCostMatrixClause(context.costMatrixClause()));

            var miningNode = OracleTree.CreateWithSpan<OracleMiningAttributeExpressionNode>(context.miningAttributeClause());
            miningNode.Columns.Value = OracleTree.CreateWithSpan<QsiColumnsDeclarationNode>(context.miningAttributeClause());

            foreach (var columnNode in VisitSelectList(context.miningAttributeClause()))
                miningNode.Columns.Value.Columns.Add(columnNode);

            node.Parameters.Add(miningNode);

            var analyticClause = context.miningAnalyticClause();

            if (analyticClause.queryPartitionClause() is not null)
                node.Parameters.Add(VisitQueryPartitionClause(analyticClause.queryPartitionClause()));

            if (analyticClause.orderByClause() is not null)
                node.Parameters.Add(VisitOrderByClause(analyticClause.orderByClause()));

            return node;
        }

        public static OracleAggregateFunctionExpressionNode VisitRankAggregateFunction(RankAggregateFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAggregateFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.RANK().GetText());

            invokeNode.Parameters.AddRange(context.expr().Select(VisitExpr));

            node.Function.Value = invokeNode;
            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleAnalyticFunctionExpressionNode VisitRankAnalyticFunction(RankAnalyticFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.RANK().GetText());

            node.Function.Value = invokeNode;
            node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());
            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleAnalyticFunctionExpressionNode VisitRatioToReportFunction(RatioToReportFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.RATIO_TO_REPORT().GetText());

            invokeNode.Parameters.Add(VisitExpr(context.expr()));

            node.Function.Value = invokeNode;

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            return node;
        }

        public static OracleInvokeExpressionNode VisitSessiontimezoneFunction(SessiontimezoneFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SESSIONTIMEZONE().GetText());

            return node;
        }

        public static OracleAnalyticFunctionExpressionNode VisitRowNumberFunction(RowNumberFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleAnalyticFunctionExpressionNode>(context);

            var invokeNode = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            invokeNode.Member.Value = TreeHelper.CreateFunction(context.ROW_NUMBER().GetText());

            node.Function.Value = invokeNode;

            if (context.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(context.queryPartitionClause());

            node.Order.Value = VisitOrderByClause(context.orderByClause());

            return node;
        }

        public static OracleInvokeExpressionNode VisitSkewnessPopFunction(SkewnessPopFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SKEWNESS_POP().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;
            else if (context.HasToken(UNIQUE))
                node.QueryBehavior = OracleQueryBehavior.Unique;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitSkewnessSampFunction(SkewnessSampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SKEWNESS_SAMP().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;
            else if (context.HasToken(UNIQUE))
                node.QueryBehavior = OracleQueryBehavior.Unique;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitSysDburigenFunction(SysDburigenFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SYS_DBURIGEN().GetText());

            node.Parameters.AddRange(context.fullObjectPath().Select(c =>
            {
                var objectNode = OracleTree.CreateWithSpan<QsiFieldExpressionNode>(c);
                objectNode.Identifier = IdentifierVisitor.VisitFullObjectPath(c);

                return objectNode;
            }));

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitSysdateFunction(SysdateFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SYSDATE().GetText());

            return node;
        }

        public static OracleInvokeExpressionNode VisitSystimestampFunction(SystimestampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SYSTIMESTAMP().GetText());

            return node;
        }

        public static OracleInvokeExpressionNode VisitToBinaryDoubleFunction(ToBinaryDoubleFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_BINARY_DOUBLE().GetText());

            ExprContext[] expressions = context.expr();

            if (context.HasToken(DEFAULT))
            {
                node.Parameters.Add(VisitExpr(expressions[0]));
                node.DefaultExpressionOnError.Value = VisitExpr(expressions[1]);

                if (expressions.Length > 2)
                    node.Parameters.AddRange(expressions.Skip(2).Select(VisitExpr));
            }
            else
            {
                node.Parameters.AddRange(expressions.Select(VisitExpr));
            }

            return node;
        }

        public static OracleInvokeExpressionNode VisitToBinaryFloatFunction(ToBinaryFloatFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_BINARY_FLOAT().GetText());

            ExprContext[] expressions = context.expr();

            if (context.HasToken(DEFAULT))
            {
                node.Parameters.Add(VisitExpr(expressions[0]));
                node.DefaultExpressionOnError.Value = VisitExpr(expressions[1]);

                if (expressions.Length > 2)
                    node.Parameters.AddRange(expressions.Skip(2).Select(VisitExpr));
            }
            else
            {
                node.Parameters.AddRange(expressions.Select(VisitExpr));
            }

            return node;
        }

        public static OracleInvokeExpressionNode VisitToDateFunction(ToDateFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_DATE().GetText());

            ExprContext[] expressions = context.expr();

            if (context.HasToken(DEFAULT))
            {
                node.Parameters.Add(VisitExpr(expressions[0]));
                node.DefaultExpressionOnError.Value = VisitExpr(expressions[1]);

                if (expressions.Length > 2)
                    node.Parameters.AddRange(expressions.Skip(2).Select(VisitExpr));
            }
            else
            {
                node.Parameters.AddRange(expressions.Select(VisitExpr));
            }

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitToDsintervalFunction(ToDsintervalFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_DSINTERVAL().GetText());

            node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            if (context.HasToken(DEFAULT))
                node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitToNumberFunction(ToNumberFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_NUMBER().GetText());

            ExprContext[] expressions = context.expr();

            if (context.HasToken(DEFAULT))
            {
                node.Parameters.Add(VisitExpr(expressions[0]));
                node.DefaultExpressionOnError.Value = VisitExpr(expressions[1]);

                if (expressions.Length > 2)
                    node.Parameters.AddRange(expressions.Skip(2).Select(VisitExpr));
            }
            else
            {
                node.Parameters.AddRange(expressions.Select(VisitExpr));
            }

            return node;
        }

        public static OracleInvokeExpressionNode VisitToTimestampFunction(ToTimestampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_TIMESTAMP().GetText());

            ExprContext[] expressions = context.expr();

            if (context.HasToken(DEFAULT))
            {
                node.Parameters.Add(VisitExpr(expressions[0]));
                node.DefaultExpressionOnError.Value = VisitExpr(expressions[1]);

                if (expressions.Length > 2)
                    node.Parameters.AddRange(expressions.Skip(2).Select(VisitExpr));
            }
            else
            {
                node.Parameters.AddRange(expressions.Select(VisitExpr));
            }

            return node;
        }

        public static OracleInvokeExpressionNode VisitToTimestampTzFunction(ToTimestampTzFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_TIMESTAMP_TZ().GetText());

            ExprContext[] expressions = context.expr();

            if (context.HasToken(DEFAULT))
            {
                node.Parameters.Add(VisitExpr(expressions[0]));
                node.DefaultExpressionOnError.Value = VisitExpr(expressions[1]);

                if (expressions.Length > 2)
                    node.Parameters.AddRange(expressions.Skip(2).Select(VisitExpr));
            }
            else
            {
                node.Parameters.AddRange(expressions.Select(VisitExpr));
            }

            return node;
        }

        public static OracleInvokeExpressionNode VisitToYmintervalFunction(ToYmintervalFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TO_YMINTERVAL().GetText());

            node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            if (context.HasToken(DEFAULT))
                node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitTranslateUsingFunction(TranslateUsingFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTypeCastFunctionExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TRANSLATE().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.HasToken(CHAR_CS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("USING CHAR_CS"));
            else if (context.HasToken(NCHAR_CS))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("USING NCHAR_CS"));

            return node;
        }

        public static OracleInvokeExpressionNode VisitTrimFunction(TrimFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TRIM().GetText());

            if (context.HasToken(FROM))
            {
                if (context.HasToken(LEADING))
                    node.Parameters.Add(TreeHelper.CreateConstantLiteral("LEADING"));
                else if (context.HasToken(TRAILING))
                    node.Parameters.Add(TreeHelper.CreateConstantLiteral("TRAILING"));
                else if (context.HasToken(BOTH))
                    node.Parameters.Add(TreeHelper.CreateConstantLiteral("BOTH"));

                if (context.expr().Length == 2)
                    node.Parameters.Add(VisitExpr(context.expr(0)));

                node.Parameters.Add(TreeHelper.CreateConstantLiteral("FROM"));
            }

            node.Parameters.Add(VisitExpr(context.expr(1)));

            return node;
        }

        public static OracleInvokeExpressionNode VisitTzOffsetFunction(TzOffsetFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.TZ_OFFSET().GetText());

            if (context.HasToken(DBTIMEZONE))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("DBTIMEZONE"));
            else if (context.HasToken(SESSIONTIMEZONE))
                node.Parameters.Add(TreeHelper.CreateConstantLiteral("SESSIONTIMEZONE"));
            else if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitUidFunction(UidFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.UID().GetText());

            return node;
        }

        public static OracleInvokeExpressionNode VisitUserFunction(UserFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.USER().GetText());

            return node;
        }

        public static OracleInvokeExpressionNode VisitValidateConversionFunction(ValidateConversionFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.VALIDATE_CONVERSION().GetText());

            ExprContext[] expressions = context.expr();

            node.Parameters.Add(VisitExpr(expressions[0]));
            node.Parameters.Add(TreeHelper.CreateConstantLiteral(context.validateConversionTypeName().GetInputText()));

            if (expressions.Length == 2)
                node.Parameters.Add(VisitExpr(expressions[1]));

            if (context.stringLiteral() is not null)
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            return node;
        }
        #endregion

        #region Json Functions
        public static OracleInvokeExpressionNode VisitJsonArrayFunction(JsonArrayFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonArrayAggFunction(JsonArrayAggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonMergePatchFunction(JsonMergePatchFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonObjectFunction(JsonObjectFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonObjectaggFunction(JsonObjectaggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonQueryFunction(JsonQueryFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonScalarFunction(JsonScalarFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonSerializeFunction(JsonSerializeFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonTableFunction(JsonTableFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonTransformFunction(JsonTransformFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitJsonValueFunction(JsonValueFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitTreatFunction(TreatFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }
        #endregion

        #region XML Functions
        public static OracleInvokeExpressionNode VisitXmlaggFunction(XmlaggFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlcastFunction(XmlcastFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlcorattvalFunction(XmlcorattvalFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlelementFunction(XmlelementFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlCdataFunction(XmlCdataFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlexistsFunction(XmlexistsFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlforestFunction(XmlforestFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlparseFunction(XmlparseFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlpiFunction(XmlpiFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlqueryFunction(XmlqueryFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlrootFunction(XmlrootFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlsequenceFunction(XmlsequenceFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlserializeFunction(XmlserializeFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }

        public static OracleInvokeExpressionNode VisitXmlTableFunction(XmlTableFunctionContext context)
        {
            throw TreeHelper.NotSupportedTree(context);
        }
        #endregion

        #region Analytic Functions
        public static OracleInvokeExpressionNode VisitAnyValueFunction(AnyValueFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(context.ANY_VALUE().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitAvgFunction(AvgFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(context.AVG().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitBitAndAggFunction(BitAndAggFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.BIT_AND_AGG().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;
            else if (context.HasToken(UNIQUE))
                node.QueryBehavior = OracleQueryBehavior.Unique;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitBitOrAggFunction(BitOrAggFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.BIT_OR_AGG().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;
            else if (context.HasToken(UNIQUE))
                node.QueryBehavior = OracleQueryBehavior.Unique;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitBitXorAggFunction(BitXorAggFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.BIT_XOR_AGG().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;
            else if (context.HasToken(UNIQUE))
                node.QueryBehavior = OracleQueryBehavior.Unique;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitChecksumFunction(ChecksumFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CHECKSUM().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitCorrFunction(CorrFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.CORR().GetText());

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static OracleInvokeExpressionNode VisitCountFunction(CountFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.COUNT().GetText());

            if (context.HasToken(MULT_SYMBOL))
            {
                var columnNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(context);
                columnNode.Column.Value = OracleTree.CreateWithSpan<QsiAllColumnNode>(context);

                node.Parameters.Add(columnNode);

                return node;
            }

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitCovarPopFunction(CovarPopFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.COVAR_POP().GetText());

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static OracleInvokeExpressionNode VisitCovarSampFunction(CovarSampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.COVAR_SAMP().GetText());

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static OracleInvokeExpressionNode VisitFirstValueFunction(FirstValueFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.FIRST_VALUE().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            // skip nulls

            return node;
        }

        public static OracleInvokeExpressionNode VisitKurtosisPopFunction(KurtosisPopFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.KURTOSIS_POP().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;
            else if (context.HasToken(UNIQUE))
                node.QueryBehavior = OracleQueryBehavior.Unique;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitKurtosisSampFunction(KurtosisSampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.KURTOSIS_SAMP().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;
            else if (context.HasToken(UNIQUE))
                node.QueryBehavior = OracleQueryBehavior.Unique;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitLastValueFunction(LastValueFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.LAST_VALUE().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            // skip nulls

            return node;
        }

        public static OracleInvokeExpressionNode VisitMaxFunction(MaxFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.MAX().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitMedianFunction(MedianFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.MEDIAN().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitMinFunction(MinFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.MIN().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitNthValueFunction(NthValueFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.NTH_VALUE().GetText());

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            // skip nulls, Order

            return node;
        }

        public static OracleInvokeExpressionNode VisitLinearRegrFunction(LinearRegrFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.children[0].GetText());

            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static OracleInvokeExpressionNode VisitStddevFunction(StddevFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.STDDEV().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitStddevPopFunction(StddevPopFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.STDDEV_POP().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitStddevSampFunction(StddevSampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.STDDEV_SAMP().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitSumFunction(SumFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.SUM().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitVarPopFunction(VarPopFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.VAR_POP().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitVarSampFunction(VarSampFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.VAR_SAMP().GetText());

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static OracleInvokeExpressionNode VisitVarianceFunction(VarianceFunctionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(context.VARIANCE().GetText());

            if (context.HasToken(DISTINCT))
                node.QueryBehavior = OracleQueryBehavior.Distinct;
            else if (context.HasToken(ALL))
                node.QueryBehavior = OracleQueryBehavior.All;

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }
        #endregion

        public static OracleWindowExpressionNode VisitWindowClause(WindowClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleWindowExpressionNode>(context);
            node.Items.AddRange(context.windowClauseItem().Select(VisitWindowClauseItem));

            return node;
        }

        public static OracleWindowItemNode VisitWindowClauseItem(WindowClauseItemContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleWindowItemNode>(context);
            node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.windowName);

            var specification = context.windowSpecification();

            if (specification.existingWindowName is not null)
                node.ExistingWindow = IdentifierVisitor.CreateQualifiedIdentifier(specification.existingWindowName);

            if (specification.queryPartitionClause() is not null)
                node.Partition.Value = VisitQueryPartitionClause(specification.queryPartitionClause());

            if (specification.orderByClause() is not null)
                node.Order.Value = VisitOrderByClause(specification.orderByClause());

            if (specification.windowingClause() is not null)
                node.Windowing.Value = VisitWindowingClause(specification.windowingClause());

            return node;
        }

        public static OracleWindowingExpressionNode VisitWindowingClause(WindowingClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleWindowingExpressionNode>(context);

            node.Type = Enum.Parse<OracleWindowingType>(context.children[0].GetText(), true);

            node.Items.AddRange(context.windowingClauseItem().Select(VisitWindowingClauseItem));

            if (context.HasToken(EXCLUDE))
            {
                if (context.HasToken(CURRENT))
                    node.Exclude.Value = TreeHelper.CreateConstantLiteral("CURRENT ROW");
                else if (context.HasToken(GROUPS))
                    node.Exclude.Value = TreeHelper.CreateConstantLiteral("GROUPS");
                else if (context.HasToken(TIES))
                    node.Exclude.Value = TreeHelper.CreateConstantLiteral("TIES");
                else if (context.HasToken(NO))
                    node.Exclude.Value = TreeHelper.CreateConstantLiteral("NO OTHERS");
            }

            return node;
        }

        private static OracleWindowingItemNode VisitWindowingClauseItem(WindowingClauseItemContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleWindowingItemNode>(context);

            switch (context.children[0])
            {
                case ExprContext exprContext:
                    node.Left.Value = VisitExpr(exprContext);

                    if (context.HasToken(PRECEDING))
                        node.Right.Value = TreeHelper.CreateConstantLiteral("PRECEDING");
                    else if (context.HasToken(FOLLOWING))
                        node.Right.Value = TreeHelper.CreateConstantLiteral("FOLLOWING");

                    return node;

                case ITerminalNode { Symbol: { Type: UNBOUNDED } }:
                    node.Left.Value = TreeHelper.CreateConstantLiteral("UNBOUNDED");
                    node.Right.Value = TreeHelper.CreateConstantLiteral("PRECEDING");
                    return node;

                case ITerminalNode { Symbol: { Type: CURRENT } }:
                    node.Left.Value = TreeHelper.CreateConstantLiteral("CURRENT");
                    node.Right.Value = TreeHelper.CreateConstantLiteral("ROW");
                    return node;

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
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
            QsiExpressionNode valueNode = null;
            QsiExpressionNode[] whenNodes;
            QsiExpressionNode[] thenNodes;
            QsiExpressionNode elseNode = null;

            switch (context.children[1])
            {
                case SimpleCaseExpressionContext simpleCaseExpression:
                    valueNode = VisitExpr(simpleCaseExpression.caseExpr);
                    whenNodes = simpleCaseExpression._comparisonExpr.Select(VisitExpr).ToArray();
                    thenNodes = simpleCaseExpression._returnExpr.Select(VisitExpr).ToArray();

                    break;

                case SearchedCaseExpressionContext searchCaseExpression:
                    whenNodes = searchCaseExpression._comparisonExpr.Select(VisitCondition).ToArray();
                    thenNodes = searchCaseExpression._returnExpr.Select(VisitExpr).ToArray();

                    break;

                default:
                    throw TreeHelper.NotSupportedTree(context.children[1]);
            }

            if (context.elseClause() is not null)
                elseNode = VisitExpr(context.elseClause().elseExpr);

            var node = OracleTree.CreateWithSpan<QsiSwitchExpressionNode>(context);

            if (valueNode is not null)
                node.Value.SetValue(valueNode);

            for (int i = 0; i < whenNodes.Length; i++)
            {
                var caseNode = new QsiSwitchCaseExpressionNode();
                caseNode.Condition.SetValue(whenNodes[i]);
                caseNode.Consequent.SetValue(thenNodes[i]);
                node.Cases.Add(caseNode);
            }

            if (elseNode is not null)
            {
                var caseNode = new QsiSwitchCaseExpressionNode();
                caseNode.Consequent.SetValue(elseNode);
                node.Cases.Add(caseNode);
            }

            return node;
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
            var node = OracleTree.CreateWithSpan<OracleIntervalExpressionNode>(context);

            node.From.Value = VisitExpr(context.from);
            node.To.Value = VisitExpr(context.to);

            node.Cycle = context.HasToken(DAY)
                ? OracleIntervalCycle.DayToSecond
                : OracleIntervalCycle.YearToMonth;

            if (context.leadingFieldPrecision is not null)
                node.LeadingFieldPrecision.Value = VisitExpr(context.leadingFieldPrecision);

            if (context.fractionalSecondPrecision is not null)
                node.FractionalSecondPrecision.Value = VisitExpr(context.fractionalSecondPrecision);

            return node;
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
            var node = OracleTree.CreateWithSpan<QsiVariableExpressionNode>(context);
            node.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.namedBindVariable().identifier());
            // skip indicator
            return node;
        }

        public static QsiExpressionNode VisitScalarSubqueryExpr(ScalarSubqueryExprContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleTableExpressionNode>(context);

            node.Table.Value = TableVisitor.VisitSubquery(context.subquery());

            return node;
        }

        public static QsiExpressionNode VisitTypeConstructorExpr(TypeConstructorExpressionContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);

            var typeConstructorNode = OracleTree.CreateWithSpan<QsiFunctionExpressionNode>(context);
            typeConstructorNode.Identifier = IdentifierVisitor.CreateQualifiedIdentifier(context.typeName().identifier());

            node.Member.Value = typeConstructorNode;
            node.Parameters.Add(VisitExpressionList(context.expressionList()));

            return node;
        }

        public static QsiExpressionNode VisitDatetimeExpr(DatetimeExprContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleDatetimeExpressionNode>(context);
            node.Expression.Value = VisitExpr(context.l);

            if (context.HasToken(LOCAL))
                node.TimeZone.Value = TreeHelper.CreateConstantLiteral("LOCAL");
            else if (context.HasToken(DBTIMEZONE))
                node.TimeZone.Value = TreeHelper.CreateConstantLiteral("TIME ZONE DBTIMEZONE");
            else if (context.HasToken(SESSIONTIMEZONE))
                node.TimeZone.Value = TreeHelper.CreateConstantLiteral("TIME ZONE SESSIONTIMEZONE");
            else if (context.timeZoneNameOrFormat is not null)
                node.TimeZone.Value = TreeHelper.CreateConstantLiteral("TIME ZONE " + context.timeZoneNameOrFormat.GetInputText());
            else
                node.TimeZone.Value = VisitExpr(context.timeZoneExpr);

            return node;
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
                    node.Value = long.Parse(text);
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
            node.Value = long.Parse(context.GetInputText());

            return node;
        }

        public static QsiExpressionNode VisitBindVariable(BindVariableContext context)
        {
            return context.children[0] switch
            {
                IndexBindVariableContext c1 => VisitIndexBindVariable(c1),
                NamedBindVariableContext c2 => VisitNamedBindVariable(c2),
                QuestionBindVariableContext c3 => VisitQuestionBindVariable(c3),
                _ => throw TreeHelper.NotSupportedTree(context.children[0])
            };
        }

        public static QsiBindParameterExpressionNode VisitIndexBindVariable(IndexBindVariableContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBindParameterExpressionNode>(context);

            node.Prefix = ":";
            node.Index = int.Parse(context.TK_INTEGER().GetText());
            node.NoSuffix = true;
            node.Type = QsiParameterType.Index;

            return node;
        }

        public static QsiBindParameterExpressionNode VisitNamedBindVariable(NamedBindVariableContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBindParameterExpressionNode>(context);

            node.Prefix = ":";
            node.Name = context.identifier().GetText();
            node.Type = QsiParameterType.Name;

            return node;
        }

        // WARNING: not used, only for parse
        public static QsiBindParameterExpressionNode VisitQuestionBindVariable(QuestionBindVariableContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBindParameterExpressionNode>(context);

            node.Prefix = "?";
            node.NoSuffix = true;
            node.Index = 0;
            node.Type = QsiParameterType.Index;

            return node;
        }

        public static QsiExpressionNode VisitMultisetExceptExpr(MultisetExceptExprContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            var leftColumnNode = new QsiColumnExpressionNode();
            var rightColumnNode = new QsiColumnExpressionNode();

            leftColumnNode.Column.Value = new QsiColumnReferenceNode
            {
                Name = IdentifierVisitor.CreateQualifiedIdentifier(context.identifier(0))
            };

            rightColumnNode.Column.Value = new QsiColumnReferenceNode
            {
                Name = IdentifierVisitor.CreateQualifiedIdentifier(context.identifier(1))
            };

            node.Left.Value = leftColumnNode;
            node.Operator = string.Join(" ", context.children.Skip(1).Take(context.ChildCount - 2).Select(c => c.GetText()));
            node.Right.Value = rightColumnNode;

            return node;
        }

        public static QsiExpressionNode VisitColumnOuterJoinExpr(ColumnOuterJoinExprContext context)
        {
            var columnRefNode = new QsiColumnReferenceNode
            {
                Name = IdentifierVisitor.VisitFullObjectPath(context.fullObjectPath())
            };

            var node = OracleTree.CreateWithSpan<OracleColumnOuterJoinExpressionNode>(context);

            node.Column.Value = columnRefNode;

            return node;
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
                InCondition2Context inCondition2 => VisitInCondition2(inCondition2),
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
            return context switch
            {
                ExprGroupComparisonConditionContext context1 => VisitExprGroupComparisonCondition(context1),
                ListGroupComparisonConditionContext context2 => VisitListGroupComparisonCondition(context2),
                LnnvlGroupComparisonConditionContext context3 => VisitLnnvlGroupComparisonCondition(context3),
                _ => throw new InvalidOperationException()
            };
        }

        public static QsiExpressionNode VisitExprGroupComparisonCondition(ExprGroupComparisonConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpr(context.expr());
            node.Operator = string.Join(" ", context.operator1().GetText(), context.option.Text);

            if (context.expressionList() is not null)
                node.Right.Value = VisitExpressionList(context.expressionList());
            else
                node.Right.Value = VisitSubquery(context.subquery());

            return node;
        }

        public static QsiExpressionNode VisitListGroupComparisonCondition(ListGroupComparisonConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);

            node.Left.Value = VisitExpressionList(context.l);

            node.Operator = string.Join(" ", context.operator2().GetText(), context.option.Text);

            if (context.subquery() is not null)
            {
                node.Right.Value = VisitSubquery(context.subquery());
            }
            else
            {
                var multipleExpressionNode = OracleTree.CreateWithSpan<QsiMultipleExpressionNode>(context.OPEN_PAR_SYMBOL(1).Symbol, context.CLOSE_PAR_SYMBOL(1).Symbol);

                foreach (var exprList in context._exprList)
                    multipleExpressionNode.Elements.Add(VisitExpressionList(exprList));

                node.Right.Value = multipleExpressionNode;
            }

            return node;
        }

        public static QsiExpressionNode VisitLnnvlGroupComparisonCondition(LnnvlGroupComparisonConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction("LNNVL");
            node.Parameters.Add(VisitCondition(context.condition()));

            return node;
        }

        public static QsiExpressionNode VisitFloatingPointCondition(FloatingPointConditionContext context)
        {
            string functionName;
            bool isNot = context.HasToken(NOT);

            if (context.HasToken(NAN))
                functionName = isNot ? OracleKnownFunction.IsNotNaN : OracleKnownFunction.IsNaN;
            else
                functionName = isNot ? OracleKnownFunction.IsNotInfinite : OracleKnownFunction.IsInfinite;

            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            node.Member.Value = TreeHelper.CreateFunction(functionName);
            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static QsiExpressionNode VisitDanglingCondition(DanglingConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            string functionName = context.HasToken(NOT) ? OracleKnownFunction.IsNotDangling : OracleKnownFunction.IsDangling;

            node.Member.Value = TreeHelper.CreateFunction(functionName);
            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static QsiExpressionNode VisitLogicalNotCondition(LogicalNotConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiUnaryExpressionNode>(context);
            node.Operator = "NOT";
            node.Expression.Value = VisitCondition(context.condition());

            return node;
        }

        public static QsiExpressionNode VisitLogicalAndCondition(LogicalAndConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
            node.Left.Value = VisitCondition(context.l);
            node.Operator = "AND";
            node.Right.Value = VisitCondition(context.r);

            return node;
        }

        public static QsiExpressionNode VisitLogicalOrCondition(LogicalOrConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
            node.Left.Value = VisitCondition(context.l);
            node.Operator = "OR";
            node.Right.Value = VisitCondition(context.r);

            return node;
        }

        public static QsiExpressionNode VisitModelIsAnyCondition(ModelIsAnyConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(OracleKnownFunction.IsAny);

            if (context.HasToken(IS))
            {
                var columnExprNode = new QsiColumnExpressionNode();

                columnExprNode.Column.Value = new QsiColumnReferenceNode
                {
                    Name = new QsiQualifiedIdentifier(IdentifierVisitor.VisitIdentifier(context.identifier()))
                };

                node.Parameters.Add(columnExprNode);
            }

            return node;
        }

        public static QsiExpressionNode VisitModelIsPresentCondition(ModelIsPresentConditionContext context)
        {
            throw TreeHelper.NotSupportedFeature("Cell Assignment");
        }

        public static QsiExpressionNode VisitMultisetIsASetCondition(MultisetIsASetConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            string functionName = context.HasToken(NOT) ? OracleKnownFunction.IsNotASet : OracleKnownFunction.IsASet;

            node.Member.Value = TreeHelper.CreateFunction(functionName);
            node.Parameters.Add(IdentifierVisitor.VisitNestedTable(context.nestedTable()));

            return node;
        }

        public static QsiExpressionNode VisitMultisetIsEmptyCondition(MultisetIsEmptyConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);

            string functionName = context.HasToken(NOT) ? OracleKnownFunction.IsNotEmpty : OracleKnownFunction.IsEmpty;

            node.Member.Value = TreeHelper.CreateFunction(functionName);
            node.Parameters.Add(IdentifierVisitor.VisitNestedTable(context.nestedTable()));

            return node;
        }

        public static QsiExpressionNode VisitMultisetMemberCondition(MultisetMemberConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
            node.Left.Value = VisitExpr(context.expr());
            node.Operator = context.HasToken(NOT) ? "NOT MEMBER OF" : "MEMBER OF";
            node.Right.Value = IdentifierVisitor.VisitNestedTable(context.nestedTable());

            return node;
        }

        public static QsiExpressionNode VisitMultisetSubmultisetCondition(MultisetSubmultisetConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiBinaryExpressionNode>(context);
            node.Left.Value = IdentifierVisitor.VisitNestedTable(context.l);
            node.Operator = context.HasToken(NOT) ? "NOT SUBMULTISET OF" : "SUBMULTISET OF";
            node.Right.Value = IdentifierVisitor.VisitNestedTable(context.r);

            return node;
        }

        public static QsiExpressionNode VisitPatternMatchingLikeCondition(PatternMatchingLikeConditionContext context)
        {
            bool isNot = context.HasToken(NOT);

            string functionName = context.likeType.Type switch
            {
                LIKE => isNot ? OracleKnownFunction.NotLike : OracleKnownFunction.Like,
                LIKEC => isNot ? OracleKnownFunction.NotLikeC : OracleKnownFunction.LikeC,
                LIKE2 => isNot ? OracleKnownFunction.NotLike2 : OracleKnownFunction.Like2,
                LIKE4 => isNot ? OracleKnownFunction.NotLike4 : OracleKnownFunction.Like4,
                _ => throw new InvalidOperationException()
            };

            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(functionName);

            node.Parameters.Add(VisitExpr(context.l));
            node.Parameters.Add(VisitExpr(context.r));

            if (context.HasToken(ESCAPE))
            {
                node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));
            }

            return node;
        }

        public static QsiExpressionNode VisitPatternMatchingRegexpLikeCondition(PatternMatchingRegexpLikeConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction(OracleKnownFunction.RegexpLike);
            node.Parameters.AddRange(context.regLikeParameter().Select(VisitRegLikeParameter));

            return node;
        }

        public static QsiExpressionNode VisitRegLikeParameter(RegLikeParameterContext context)
        {
            if (context.column() is not null)
            {
                var columnExprNode = new QsiColumnExpressionNode();
                columnExprNode.Column.Value = IdentifierVisitor.VisitColumn(context.column());

                return columnExprNode;
            }

            return VisitStringLiteral(context.stringLiteral());
        }

        public static QsiExpressionNode VisitIsNullCondition(IsNullConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            var functionName = context.HasToken(NOT) ? OracleKnownFunction.IsNotNull : OracleKnownFunction.IsNull;
            node.Member.Value = TreeHelper.CreateFunction(functionName);

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static QsiExpressionNode VisitXmlEqualsPathCondition(XmlEqualsPathConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction("EQUALS_PATH");
            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static QsiExpressionNode VisitXmlUnderPathCondition(XmlUnderPathConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction("UNDER_PATH");
            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static QsiExpressionNode VisitJsonIsJsonCondition(JsonIsJsonConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            var functionName = context.HasToken(NOT) ? OracleKnownFunction.IsJson : OracleKnownFunction.IsNotJson;
            node.Member.Value = TreeHelper.CreateFunction(functionName);

            node.Parameters.Add(VisitExpr(context.expr()));

            return node;
        }

        public static QsiExpressionNode VisitJsonEqualCondition(JsonEqualConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction("JSON_EQUAL");
            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static QsiExpressionNode VisitJsonExistsCondition(JsonExistsConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction("JSON_EXISTS");
            node.Parameters.Add(VisitExpr(context.expr()));
            node.Parameters.Add(VisitStringLiteral(context.stringLiteral()));

            // FORMAT JSON, jsonPassingClause, jsonExistsOnErrorClause, jsonExistsOnEmptyClause ignored

            return node;
        }

        public static QsiExpressionNode VisitJsonTextContainsCondition(JsonTextContainsConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction("JSON_TEXTCONTAINS");

            var columnNode = OracleTree.CreateWithSpan<QsiColumnExpressionNode>(context);
            columnNode.Column.Value = IdentifierVisitor.VisitColumn(context.column());

            node.Parameters.Add(columnNode);
            node.Parameters.Add(VisitStringLiteral(context.stringLiteral(0)));
            node.Parameters.Add(VisitStringLiteral(context.stringLiteral(1)));

            // FORMAT JSON, jsonPassingClause, jsonExistsOnErrorClause, jsonExistsOnEmptyClause ignored

            return node;
        }

        public static QsiExpressionNode VisitCompoundParenthesisCondition(CompoundParenthesisConditionContext context)
        {
            return VisitCondition(context.condition());
        }

        public static QsiExpressionNode VisitBetweenCondition(BetweenConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            var functionName = context.HasToken(NOT) ? OracleKnownFunction.NotBetween : OracleKnownFunction.Between;
            node.Member.Value = TreeHelper.CreateFunction(functionName);
            node.Parameters.AddRange(context.expr().Select(VisitExpr));

            return node;
        }

        public static QsiExpressionNode VisitExistsCondition(ExistsConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            node.Member.Value = TreeHelper.CreateFunction("EXISTS");
            node.Parameters.Add(VisitSubquery(context.subquery()));

            return node;
        }

        public static QsiExpressionNode VisitInCondition1(InCondition1Context context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            var functionName = context.HasToken(NOT) ? OracleKnownFunction.NotIn : OracleKnownFunction.In;
            node.Member.Value = TreeHelper.CreateFunction(functionName);

            node.Parameters.Add(VisitExpr(context.expr()));

            if (context.expressionList() is not null)
                node.Parameters.Add(VisitExpressionList(context.expressionList()));
            else
                node.Parameters.Add(VisitSubquery(context.subquery()));

            return node;
        }

        public static QsiExpressionNode VisitInCondition2(InCondition2Context context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            var functionName = context.HasToken(NOT) ? OracleKnownFunction.NotIn : OracleKnownFunction.In;
            node.Member.Value = TreeHelper.CreateFunction(functionName);

            node.Parameters.Add(VisitExpressionList(context.l));

            if (context.expressionList() is not null)
                node.Parameters.AddRange(context.expressionList().Select(VisitExpressionList));
            else
                node.Parameters.Add(VisitSubquery(context.subquery()));

            return node;
        }

        public static QsiExpressionNode VisitIsOfTypeCondition(IsOfTypeConditionContext context)
        {
            var node = OracleTree.CreateWithSpan<QsiInvokeExpressionNode>(context);
            var functionName = context.HasToken(NOT) ? OracleKnownFunction.NotIn : OracleKnownFunction.In;
            node.Member.Value = TreeHelper.CreateFunction(functionName);

            node.Parameters.Add(VisitExpr(context.expr()));

            foreach (var item in context.isOfTypeConditionItem())
            {
                // Node name is TableReference, but isOfTypeConditionItem is type
                var typeNode = new OracleTableReferenceNode
                {
                    Identifier = IdentifierVisitor.VisitFullObjectPath(item.fullObjectPath()),
                    IsOnly = item.HasToken(ONLY)
                };

                var parameter = new OracleTableExpressionNode();
                parameter.Table.Value = typeNode;

                node.Parameters.Add(parameter);
            }

            return node;
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

        public static OracleInvokeExpressionNode VisitCommonFunction(ParserRuleContext context, FunctionNameContext functionNameContext, ArgumentListContext argumentListContext)
        {
            var node = OracleTree.CreateWithSpan<OracleInvokeExpressionNode>(context);
            var functionName = IdentifierVisitor.CreateQualifiedIdentifier(functionNameContext.identifier());

            var functionExpressionNode = OracleTree.CreateWithSpan<QsiFunctionExpressionNode>(context);
            functionExpressionNode.Identifier = functionName;

            node.Member.Value = functionExpressionNode;

            IEnumerable<QsiExpressionNode> argumentList = VisitArgumentList(argumentListContext);

            if (argumentList is not null)
                node.Parameters.AddRange(argumentList);

            return node;
        }

        public static IEnumerable<OracleSetValueExpressionNode> VisitUpdateSetClause(UpdateSetClauseContext context)
        {
            if (context.HasToken(VALUE))
                throw TreeHelper.NotSupportedFeature("Update Object Table");

            return context.updateSetSubstituteClause().Select(VisitUpdateSetSubstituteClause);
        }

        public static OracleSetValueExpressionNode VisitUpdateSetSubstituteClause(UpdateSetSubstituteClauseContext context)
        {
            var oracleNode = OracleTree.CreateWithSpan<OracleSetValueExpressionNode>(context);

            switch (context)
            {
                case MultipleUpdateSetSubstituteClauseContext multipleUpdateSetContext:
                {
                    var node = OracleTree.CreateWithSpan<OracleSetColumnsExpressionNode>(context);

                    node.Targets = multipleUpdateSetContext.column().Select(c => IdentifierVisitor.CreateQualifiedIdentifier(c.identifier())).ToArray();
                    node.Value.Value = TableVisitor.VisitSubquery(multipleUpdateSetContext.subquery());

                    oracleNode.SetValueFromTable.Value = node;
                    break;
                }

                case SingleUpdateSetSubstituteClauseContext singleUpdateSetContext:
                {
                    var node = OracleTree.CreateWithSpan<QsiSetColumnExpressionNode>(context);

                    node.Target = IdentifierVisitor.CreateQualifiedIdentifier(singleUpdateSetContext.column().identifier());

                    if (singleUpdateSetContext.expr() is not null)
                        node.Value.Value = VisitExpr(singleUpdateSetContext.expr());
                    else if (singleUpdateSetContext.subquery() is not null)
                        node.Value.Value = VisitSubquery(singleUpdateSetContext.subquery());
                    else
                        node.Value.Value = TreeHelper.CreateDefaultLiteral();

                    oracleNode.SetValue.Value = node;
                    break;
                }

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }

            return oracleNode;
        }

        public static QsiExpressionNode VisitCostMatrixClause(CostMatrixClauseContext context)
        {
            var node = OracleTree.CreateWithSpan<OracleCostMatrixExpressionNode>(context);

            if (context.HasToken(MODEL))
            {
                node.IsSpecifiedModel = false;
                node.IsAuto = context.HasToken(AUTO);
            }
            else
            {
                node.Models.Value = VisitExpressionList(context.models);
                node.ModelValues.AddRange(context._values.Select(VisitExpressionList));
            }

            return node;
        }
    }
}
