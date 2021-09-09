using System;
using System.Linq;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
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
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitLogicalNot(LogicalNotContext context)
        {
            throw new System.NotImplementedException();
        }

        private static QsiExpressionNode VisitPredicated(PredicatedContext context)
        {
            if (context.HasRule<PredicateContext>())
                throw new NotImplementedException();

            return VisitValueExpression(context.valueExpression());
        }

        public static QsiExpressionNode VisitValueExpression(ValueExpressionContext context)
        {
            switch (context)
            {
                case ValueExpressionDefaultContext valueExpressionDefault:
                    return VisitPrimaryExpression(valueExpressionDefault.primaryExpression());

                case AtTimeZoneContext atTimeZone:
                    throw new NotImplementedException();

                case ArithmeticUnaryContext arithmeticUnary:
                    throw new NotImplementedException();

                case ArithmeticBinaryContext arithmeticBinary:
                    throw new NotImplementedException();

                case ConcatenationContext concatenation:
                    throw new NotImplementedException();

                default:
                    throw TreeHelper.NotSupportedTree(context);
            }
        }

        public static QsiExpressionNode VisitPrimaryExpression(PrimaryExpressionContext context)
        {
            switch (context)
            {
                case ColumnReferenceContext columnReference:
                    return VisitColumnReference(columnReference);

                default:
                    throw new NotImplementedException();
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

        public static QsiExpressionNode VisitLambda(LambdaContext context)
        {
            var node = TrinoTree.CreateWithSpan<TrinoLambdaExpressionNode>(context);

            node.Identifiers.AddRange(context.identifier().Select(i => i.qi));
            node.Expression.Value = VisitExpression(context.expression());

            return node;
        }

        public static QsiTableNode VisitSubqueryExpression(SubqueryExpressionContext context)
        {
            return TableVisitor.VisitQuery(context.query());
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

            if (qualifiedNames is not null)
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
            var node = TrinoTree.CreateWithSpan<TrinoOrderByExpressionNode>(context);

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
