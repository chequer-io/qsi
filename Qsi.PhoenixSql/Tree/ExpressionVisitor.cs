using System;
using PhoenixSql;
using PhoenixSql.Extensions;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PhoenixSql.Tree
{
    internal static class ExpressionVisitor
    {
        public static QsiExpressionNode Visit(IParseNode node)
        {
            switch (node.Unwrap())
            {
                case INamedParseNode namedParseNode:
                    return VisitNamedParseNode(namedParseNode);

                case IComparisonParseNode comparisonParseNode:
                    return VisitComparisonParseNode(comparisonParseNode);

                case IArrayAllAnyComparisonNode arrayAllAnyComparisonNode:
                    return VisitArrayAllAnyComparisonNode(arrayAllAnyComparisonNode);

                case StringConcatParseNode stringConcatParseNode:
                    return VisitStringConcatParseNode(stringConcatParseNode);

                case FunctionParseNode functionParseNode:
                    return VisitFunctionParseNode(functionParseNode);

                case AggregateFunctionParseNode aggregateFunctionParseNode:
                    return VisitAggregateFunctionParseNode(aggregateFunctionParseNode);

                case AggregateFunctionWithinGroupParseNode aggregateFunctionWithinGroupParseNode:
                    return VisitAggregateFunctionWithinGroupParseNode(aggregateFunctionWithinGroupParseNode);

                case SumAggregateParseNode sumAggregateParseNode:
                    return VisitSumAggregateParseNode(sumAggregateParseNode);

                case LastValuesAggregateParseNode lastValuesAggregateParseNode:
                    return VisitLastValuesAggregateParseNode(lastValuesAggregateParseNode);

                case FirstValuesAggregateParseNode firstValuesAggregateParseNode:
                    return VisitFirstValuesAggregateParseNode(firstValuesAggregateParseNode);

                case NthValueAggregateParseNode nthValueAggregateParseNode:
                    return VisitNthValueAggregateParseNode(nthValueAggregateParseNode);

                case MinAggregateParseNode minAggregateParseNode:
                    return VisitMinAggregateParseNode(minAggregateParseNode);

                case MaxAggregateParseNode maxAggregateParseNode:
                    return VisitMaxAggregateParseNode(maxAggregateParseNode);

                case LastValueAggregateParseNode lastValueAggregateParseNode:
                    return VisitLastValueAggregateParseNode(lastValueAggregateParseNode);

                case DistinctCountHyperLogLogAggregateParseNode distinctCountHyperLogLogAggregateParseNode:
                    return VisitDistinctCountHyperLogLogAggregateParseNode(distinctCountHyperLogLogAggregateParseNode);

                case FirstValueAggregateParseNode firstValueAggregateParseNode:
                    return VisitFirstValueAggregateParseNode(firstValueAggregateParseNode);

                case DistinctCountParseNode distinctCountParseNode:
                    return VisitDistinctCountParseNode(distinctCountParseNode);

                case AvgAggregateParseNode avgAggregateParseNode:
                    return VisitAvgAggregateParseNode(avgAggregateParseNode);

                case ToTimeParseNode toTimeParseNode:
                    return VisitToTimeParseNode(toTimeParseNode);

                case CurrentTimeParseNode currentTimeParseNode:
                    return VisitCurrentTimeParseNode(currentTimeParseNode);

                case ToCharParseNode toCharParseNode:
                    return VisitToCharParseNode(toCharParseNode);

                case RegexpSplitParseNode regexpSplitParseNode:
                    return VisitRegexpSplitParseNode(regexpSplitParseNode);

                case ToTimestampParseNode toTimestampParseNode:
                    return VisitToTimestampParseNode(toTimestampParseNode);

                case CurrentDateParseNode currentDateParseNode:
                    return VisitCurrentDateParseNode(currentDateParseNode);

                case RegexpReplaceParseNode regexpReplaceParseNode:
                    return VisitRegexpReplaceParseNode(regexpReplaceParseNode);

                case ToNumberParseNode toNumberParseNode:
                    return VisitToNumberParseNode(toNumberParseNode);

                case ArrayModifierParseNode arrayModifierParseNode:
                    return VisitArrayModifierParseNode(arrayModifierParseNode);

                case RegexpSubstrParseNode regexpSubstrParseNode:
                    return VisitRegexpSubstrParseNode(regexpSubstrParseNode);

                case FloorParseNode floorParseNode:
                    return VisitFloorParseNode(floorParseNode);

                case UDFParseNode uDFParseNode:
                    return VisitUDFParseNode(uDFParseNode);

                case ToDateParseNode toDateParseNode:
                    return VisitToDateParseNode(toDateParseNode);

                case RoundParseNode roundParseNode:
                    return VisitRoundParseNode(roundParseNode);

                case CeilParseNode ceilParseNode:
                    return VisitCeilParseNode(ceilParseNode);

                case MultiplyParseNode multiplyParseNode:
                    return VisitMultiplyParseNode(multiplyParseNode);

                case AddParseNode addParseNode:
                    return VisitAddParseNode(addParseNode);

                case SubtractParseNode subtractParseNode:
                    return VisitSubtractParseNode(subtractParseNode);

                case ModulusParseNode modulusParseNode:
                    return VisitModulusParseNode(modulusParseNode);

                case DivideParseNode diviVisitNode:
                    return VisitDivideParseNode(diviVisitNode);

                case InParseNode inParseNode:
                    return VisitInParseNode(inParseNode);

                case LikeParseNode likeParseNode:
                    return VisitLikeParseNode(likeParseNode);

                case OrParseNode orParseNode:
                    return VisitOrParseNode(orParseNode);

                case ArrayElemRefNode arrayElemRefNode:
                    return VisitArrayElemRefNode(arrayElemRefNode);

                case InListParseNode inListParseNode:
                    return VisitInListParseNode(inListParseNode);

                case ExistsParseNode existsParseNode:
                    return VisitExistsParseNode(existsParseNode);

                case NotParseNode notParseNode:
                    return VisitNotParseNode(notParseNode);

                case IsNullParseNode isNullParseNode:
                    return VisitIsNullParseNode(isNullParseNode);

                case CastParseNode castParseNode:
                    return VisitCastParseNode(castParseNode);

                case RowValueConstructorParseNode rowValueConstructorParseNode:
                    return VisitRowValueConstructorParseNode(rowValueConstructorParseNode);

                case ArrayConstructorNode arrayConstructorNode:
                    return VisitArrayConstructorNode(arrayConstructorNode);

                case AndParseNode andParseNode:
                    return VisitAndParseNode(andParseNode);

                case CaseParseNode caseParseNode:
                    return VisitCaseParseNode(caseParseNode);

                case BetweenParseNode betweenParseNode:
                    return VisitBetweenParseNode(betweenParseNode);

                case SubqueryParseNode subqueryParseNode:
                    return VisitSubqueryParseNode(subqueryParseNode);

                case WildcardParseNode wildcardParseNode:
                    return VisitWildcardParseNode(wildcardParseNode);

                case SequenceValueParseNode sequenceValueParseNode:
                    return VisitSequenceValueParseNode(sequenceValueParseNode);

                case LiteralParseNode literalParseNode:
                    return VisitLiteralParseNode(literalParseNode);

                default:
                    throw TreeHelper.NotSupportedTree(node);
            }
        }

        #region INamedParseNode
        private static QsiExpressionNode VisitNamedParseNode(INamedParseNode node)
        {
            switch (node.UnwrapAs<INamedParseNode>())
            {
                case FamilyWildcardParseNode familyWildcardParseNode:
                    return VisitFamilyWildcardParseNode(familyWildcardParseNode);

                case BindParseNode bindParseNode:
                    return VisitBindParseNode(bindParseNode);

                case ColumnParseNode columnParseNode:
                    return VisitColumnParseNode(columnParseNode);

                case TableWildcardParseNode tableWildcardParseNode:
                    return VisitTableWildcardParseNode(tableWildcardParseNode);

                default:
                    throw TreeHelper.NotSupportedTree(node);
            }
        }

        private static QsiExpressionNode VisitFamilyWildcardParseNode(FamilyWildcardParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitBindParseNode(BindParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitColumnParseNode(ColumnParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitTableWildcardParseNode(TableWildcardParseNode node)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IComparisonParseNode
        private static QsiExpressionNode VisitComparisonParseNode(IComparisonParseNode node)
        {
            // NotEqualParseNode
            // EqualParseNode
            // GreaterThanOrEqualParseNode
            // GreaterThanParseNode
            // LessThanOrEqualParseNode
            // LessThanParseNode
            throw new NotImplementedException();
        }
        #endregion

        #region IArrayAllAnyComparisonNode
        private static QsiExpressionNode VisitArrayAllAnyComparisonNode(IArrayAllAnyComparisonNode node)
        {
            switch (node.UnwrapAs<IArrayAllAnyComparisonNode>())
            {
                case ArrayAnyComparisonNode arrayAnyComparisonNode:
                    return VisitArrayAnyComparisonNode(arrayAnyComparisonNode);

                case ArrayAllComparisonNode arrayAllComparisonNode:
                    return VisitArrayAllComparisonNode(arrayAllComparisonNode);

                default:
                    throw TreeHelper.NotSupportedTree(node);
            }
        }

        private static QsiExpressionNode VisitArrayAnyComparisonNode(ArrayAnyComparisonNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitArrayAllComparisonNode(ArrayAllComparisonNode node)
        {
            throw new NotImplementedException();
        }
        #endregion

        private static QsiExpressionNode VisitStringConcatParseNode(StringConcatParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitFunctionParseNode(FunctionParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitAggregateFunctionParseNode(AggregateFunctionParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitAggregateFunctionWithinGroupParseNode(AggregateFunctionWithinGroupParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitSumAggregateParseNode(SumAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitLastValuesAggregateParseNode(LastValuesAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitFirstValuesAggregateParseNode(FirstValuesAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitNthValueAggregateParseNode(NthValueAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitMinAggregateParseNode(MinAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitMaxAggregateParseNode(MaxAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitLastValueAggregateParseNode(LastValueAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitDistinctCountHyperLogLogAggregateParseNode(DistinctCountHyperLogLogAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitFirstValueAggregateParseNode(FirstValueAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitDistinctCountParseNode(DistinctCountParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitAvgAggregateParseNode(AvgAggregateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitToTimeParseNode(ToTimeParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitCurrentTimeParseNode(CurrentTimeParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitToCharParseNode(ToCharParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitRegexpSplitParseNode(RegexpSplitParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitToTimestampParseNode(ToTimestampParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitCurrentDateParseNode(CurrentDateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitRegexpReplaceParseNode(RegexpReplaceParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitToNumberParseNode(ToNumberParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitArrayModifierParseNode(ArrayModifierParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitRegexpSubstrParseNode(RegexpSubstrParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitFloorParseNode(FloorParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitUDFParseNode(UDFParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitToDateParseNode(ToDateParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitRoundParseNode(RoundParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitCeilParseNode(CeilParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitMultiplyParseNode(MultiplyParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitAddParseNode(AddParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitSubtractParseNode(SubtractParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitModulusParseNode(ModulusParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitDivideParseNode(DivideParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitInParseNode(InParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitLikeParseNode(LikeParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitOrParseNode(OrParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitArrayElemRefNode(ArrayElemRefNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitInListParseNode(InListParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitExistsParseNode(ExistsParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitNotParseNode(NotParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitIsNullParseNode(IsNullParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitCastParseNode(CastParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitRowValueConstructorParseNode(RowValueConstructorParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitArrayConstructorNode(ArrayConstructorNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitAndParseNode(AndParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitCaseParseNode(CaseParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitBetweenParseNode(BetweenParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitSubqueryParseNode(SubqueryParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitWildcardParseNode(WildcardParseNode node)
        {
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitSequenceValueParseNode(SequenceValueParseNode node)
        {
            // CURRENT VALUE FOR
            throw new NotImplementedException();
        }

        private static QsiExpressionNode VisitLiteralParseNode(LiteralParseNode node)
        {
            var literalNode = new QsiLiteralExpressionNode
            {
                Type = QsiDataType.Raw,
                Value = node.Value
            };

            PhoenixSqlTree.SetRawNode(literalNode, node);

            return literalNode;
        }

        public static QsiWhereExpressionNode VisitWhere(IParseNode node)
        {
            return TreeHelper.Create<QsiWhereExpressionNode>(n =>
            {
                n.Expression.SetValue(Visit(node));
            });
        }

        public static QsiLimitExpressionNode VisitLimitOffset(LimitNode limitNode, OffsetNode offsetNode)
        {
            return TreeHelper.Create<QsiLimitExpressionNode>(n =>
            {
                var limit = limitNode?.LimitParseNode;
                var offset = offsetNode?.OffsetParseNode;

                if (limit != null)
                    n.Limit.SetValue(Visit(limit));

                if (offsetNode != null)
                    n.Offset.SetValue(Visit(offset));
            });
        }
    }
}
