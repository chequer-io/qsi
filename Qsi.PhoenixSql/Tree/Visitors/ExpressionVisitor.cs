using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                case IFunctionParseNode functionParseNode:
                    return VisitFunctionParseNode(functionParseNode);

                case IArithmeticParseNode arithmeticParseNode:
                    return VisitArithmeticParseNode(arithmeticParseNode);

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
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(TableVisitor.VisitNamedParseNode(node));
            });
        }
        #endregion

        #region IComparisonParseNode
        private static QsiExpressionNode VisitComparisonParseNode(IComparisonParseNode node)
        {
            var binaryNode = TreeHelper.CreateBinaryExpression(node.FilterOp.ToSql(), node.LHS, node.RHS, Visit);

            PTree.RawNode[binaryNode] = node;

            return binaryNode;
        }
        #endregion

        #region IArrayAllAnyComparisonNode
        private static QsiExpressionNode VisitArrayAllAnyComparisonNode(IArrayAllAnyComparisonNode node)
        {
            var left = node.Children[0];
            var comp = (IComparisonParseNode)node.Children[1];

            var leftExpressionNode = Visit(left);

            var rightExpressionNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(node.Type));
                n.Parameters.Add(Visit(comp.LHS));
            });

            return TreeHelper.Create<QsiBinaryExpressionNode>(n =>
            {
                n.Operator = comp.FilterOp.ToSql();
                n.Left.SetValue(leftExpressionNode);
                n.Right.SetValue(rightExpressionNode);

                PTree.RawNode[n] = node;
            });
        }
        #endregion

        private static QsiExpressionNode VisitStringConcatParseNode(StringConcatParseNode node)
        {
            return CreateChainedBinaryExpression(node, node.Children, PhoenixSqlKnownOperator.StringConcat);
        }

        private static QsiExpressionNode VisitFunctionParseNode(IFunctionParseNode node)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(node.Name));
                n.Parameters.AddRange(node.Children.Select(Visit));

                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitArithmeticParseNode(IArithmeticParseNode node)
        {
            var op = node switch
            {
                MultiplyParseNode _ => PhoenixSqlKnownOperator.Multiply,
                AddParseNode _ => PhoenixSqlKnownOperator.Add,
                SubtractParseNode _ => PhoenixSqlKnownOperator.Subtract,
                ModulusParseNode _ => PhoenixSqlKnownOperator.Modulus,
                DivideParseNode _ => PhoenixSqlKnownOperator.Divide,
                _ => throw TreeHelper.NotSupportedTree(node)
            };

            return CreateChainedBinaryExpression(node, node.Children, op);
        }

        private static QsiExpressionNode VisitInParseNode(InParseNode node)
        {
            QsiExpressionNode expressionNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(PhoenixSqlKnownFunction.In));
                n.Parameters.AddRange(node.Children.Select(Visit));
            });

            if (node.IsNegate)
                expressionNode = TreeHelper.CreateUnary(PhoenixSqlKnownOperator.UnaryNot, expressionNode);

            PTree.RawNode[expressionNode] = node;

            return expressionNode;
        }

        private static QsiExpressionNode VisitLikeParseNode(LikeParseNode node)
        {
            QsiExpressionNode expressionNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                var function = node.LikeType == LikeType.CaseSensitive ?
                    PhoenixSqlKnownFunction.Like : PhoenixSqlKnownFunction.ILike;

                n.Member.SetValue(TreeHelper.CreateFunction(function));
                n.Parameters.AddRange(node.Children.Select(Visit));
            });

            if (node.IsNegate)
                expressionNode = TreeHelper.CreateUnary(PhoenixSqlKnownOperator.UnaryNot, expressionNode);

            PTree.RawNode[expressionNode] = node;

            return expressionNode;
        }

        private static QsiExpressionNode VisitOrParseNode(OrParseNode node)
        {
            return CreateChainedBinaryExpression(node, node.Children, PhoenixSqlKnownOperator.Or);
        }

        private static QsiExpressionNode VisitArrayElemRefNode(ArrayElemRefNode node)
        {
            return TreeHelper.Create<QsiMemberAccessExpressionNode>(n =>
            {
                n.Target.SetValue(Visit(node.Children[0]));
                n.Member.SetValue(Visit(node.Children[1]));

                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitInListParseNode(InListParseNode node)
        {
            QsiExpressionNode expressionNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(PhoenixSqlKnownFunction.ArrayIn));
                n.Parameters.AddRange(node.Children.Select(Visit));
            });

            if (node.IsNegate)
                expressionNode = TreeHelper.CreateUnary(PhoenixSqlKnownOperator.UnaryNot, expressionNode);

            PTree.RawNode[expressionNode] = node;

            return expressionNode;
        }

        private static QsiExpressionNode VisitExistsParseNode(ExistsParseNode node)
        {
            QsiExpressionNode expressionNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(PhoenixSqlKnownFunction.Exists));
                n.Parameters.AddRange(node.Children.Select(Visit));
            });

            if (node.IsNegate)
                expressionNode = TreeHelper.CreateUnary(PhoenixSqlKnownOperator.UnaryNot, expressionNode);

            PTree.RawNode[expressionNode] = node;

            return expressionNode;
        }

        private static QsiExpressionNode VisitNotParseNode(NotParseNode node)
        {
            var expressionNode = TreeHelper.CreateUnary(PhoenixSqlKnownOperator.UnaryNot, Visit(node.Children[0]));
            PTree.RawNode[expressionNode] = node;
            return expressionNode;
        }

        private static QsiExpressionNode VisitIsNullParseNode(IsNullParseNode node)
        {
            QsiExpressionNode expressionNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(PhoenixSqlKnownFunction.IsNull));
                n.Parameters.AddRange(node.Children.Select(Visit));
            });

            if (node.IsNegate)
                expressionNode = TreeHelper.CreateUnary(PhoenixSqlKnownOperator.UnaryNot, expressionNode);

            PTree.RawNode[expressionNode] = node;

            return expressionNode;
        }

        private static QsiExpressionNode VisitCastParseNode(CastParseNode node)
        {
            var typeName = new StringBuilder();
            var dataType = node.DataType;
            var isArray = dataType.IsArray();

            if (isArray)
                dataType = node.DataType.GetElementType();

            typeName.Append(dataType.ToSqlTypeName());

            if (node.MaxLength != 0)
            {
                typeName.Append('(');
                typeName.Append(node.MaxLength);

                if (node.Scale != 0)
                {
                    typeName.Append(',');
                    typeName.Append(node.Scale);
                }

                typeName.Append(')');
            }

            if (isArray)
                typeName.Append(" ARRAY");

            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(PhoenixSqlKnownFunction.Cast));
                n.Parameters.Add(Visit(node.Children[0]));
                n.Parameters.Add(CreateTypeAccessExpression(typeName.ToString()));

                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitRowValueConstructorParseNode(RowValueConstructorParseNode node)
        {
            var expressionNode = new QsiRowValueExpressionNode();

            expressionNode.ColumnValues.AddRange(node.Children.Select(Visit));

            PTree.RawNode[expressionNode] = node;

            return expressionNode;
        }

        private static QsiExpressionNode VisitArrayConstructorNode(ArrayConstructorNode node)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(PhoenixSqlKnownFunction.Array));
                n.Parameters.AddRange(node.Children.Select(Visit));

                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitAndParseNode(AndParseNode node)
        {
            return CreateChainedBinaryExpression(node, node.Children, PhoenixSqlKnownOperator.Or);
        }

        private static QsiExpressionNode VisitCaseParseNode(CaseParseNode node)
        {
            return TreeHelper.Create<QsiSwitchExpressionNode>(n =>
            {
                for (int i = 0; i < node.Children.Count - 1; i += 2)
                {
                    var caseExpressionNode = new QsiSwitchCaseExpressionNode();
                    caseExpressionNode.Condition.SetValue(Visit(node.Children[i + 1]));
                    caseExpressionNode.Consequent.SetValue(Visit(node.Children[i]));
                    n.Cases.Add(caseExpressionNode);
                }

                if (node.Children.Count % 2 != 0)
                {
                    var caseExpressionNode = new QsiSwitchCaseExpressionNode();
                    caseExpressionNode.Consequent.SetValue(Visit(node.Children[^1]));
                    n.Cases.Add(caseExpressionNode);
                }

                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitBetweenParseNode(BetweenParseNode node)
        {
            QsiExpressionNode expressionNode = TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                n.Member.SetValue(TreeHelper.CreateFunction(PhoenixSqlKnownFunction.Between));
                n.Parameters.AddRange(node.Children.Select(Visit));
            });

            if (node.IsNegate)
                expressionNode = TreeHelper.CreateUnary(PhoenixSqlKnownOperator.UnaryNot, expressionNode);

            PTree.RawNode[expressionNode] = node;

            return expressionNode;
        }

        private static QsiExpressionNode VisitSubqueryParseNode(SubqueryParseNode node)
        {
            return TreeHelper.Create<QsiTableExpressionNode>(n =>
            {
                n.Table.SetValue(TableVisitor.VisitSelectStatement(node.SelectNode));
                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitWildcardParseNode(WildcardParseNode node)
        {
            return TreeHelper.Create<QsiColumnExpressionNode>(n =>
            {
                n.Column.SetValue(TableVisitor.VisitWildcardParseNode(node));
                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitSequenceValueParseNode(SequenceValueParseNode node)
        {
            return TreeHelper.Create<QsiInvokeExpressionNode>(n =>
            {
                var function = node.Op == Op.CurrentValue ?
                    PhoenixSqlKnownFunction.CurrentValueFor : PhoenixSqlKnownFunction.NextValueFor;

                n.Member.SetValue(TreeHelper.CreateFunction(function));

                // SKIP: node.TableName -> string literal
                n.Parameters.Add(TreeHelper.CreateLiteral(IdentifierVisitor.Visit(node.TableName).ToString()));

                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode VisitLiteralParseNode(LiteralParseNode node)
        {
            var literalNode = new QsiLiteralExpressionNode
            {
                Type = QsiDataType.Raw,
                Value = node.Value
            };

            PTree.RawNode[literalNode] = node;

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

        public static QsiMultipleOrderExpressionNode VisitOrderBy(IEnumerable<OrderByNode> nodes)
        {
            var node = new QsiMultipleOrderExpressionNode();
            node.Orders.AddRange(nodes.Select(VisitOrderBy));
            return node;
        }

        public static QsiOrderExpressionNode VisitOrderBy(OrderByNode node)
        {
            return TreeHelper.Create<QsiOrderExpressionNode>(n =>
            {
                n.Order = node.IsAscending ? QsiSortOrder.Ascending : QsiSortOrder.Descending;
                n.Expression.SetValue(Visit(node.Node));

                PTree.RawNode[n] = node;
            });
        }

        private static QsiExpressionNode CreateChainedBinaryExpression(IPhoenixNode node, IReadOnlyList<IParseNode> children, string op)
        {
            var anchor = Visit(children[0]);

            foreach (var child in children.Skip(1))
            {
                var binaryNode = new QsiBinaryExpressionNode
                {
                    Operator = op
                };

                binaryNode.Left.SetValue(anchor);
                binaryNode.Right.SetValue(Visit(child));

                anchor = binaryNode;
            }

            PTree.RawNode[anchor] = node;
            return anchor;
        }

        private static QsiTypeExpressionNode CreateTypeAccessExpression(string value)
        {
            return new()
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(value, false))
            };
        }

        public static QsiBindParameterExpressionNode VisitBind(BindParseNode node)
        {
            return TreeHelper.Create<QsiBindParameterExpressionNode>(n =>
            {
                n.Type = QsiParameterType.Name;
                n.Prefix = ":";
                n.Name = node.Name;

                PTree.RawNode[n] = node;
            });
        }
    }
}
