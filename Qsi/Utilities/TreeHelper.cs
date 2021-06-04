using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Utilities
{
    public static class TreeHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiExpressionFragmentNode Fragment(string value)
        {
            return new() { Value = value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TNode Create<TNode>(Action<TNode> action) where TNode : QsiTreeNode, new()
        {
            var node = new TNode();
            action(node);
            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiDerivedTableNode CreateAliasedTableNode(QsiTableNode node, QsiIdentifier alias)
        {
            return CreateAliasedTableNode(node, new QsiAliasNode { Name = alias });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiDerivedTableNode CreateAliasedTableNode(QsiTableNode node, QsiAliasNode alias)
        {
            return new()
            {
                Source =
                {
                    Value = node
                },
                Alias =
                {
                    Value = alias
                },
                Columns =
                {
                    Value = CreateAllColumnsDeclaration()
                }
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiBinaryExpressionNode CreateBinaryExpression<TContext>(
            string @operator,
            TContext left,
            TContext right,
            Func<TContext, QsiExpressionNode> visitor)
        {
            return Create<QsiBinaryExpressionNode>(n =>
            {
                n.Operator = @operator;
                n.Left.SetValue(visitor(left));
                n.Right.SetValue(visitor(right));
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiExpressionNode CreateChainedBinaryExpression<TContext>(
            string @operator,
            IEnumerable<TContext> contexts,
            Func<TContext, QsiExpressionNode> visitor,
            Action<TContext, QsiExpressionNode> callback = null)
        {
            QsiExpressionNode node = null;

            foreach (var context in contexts)
            {
                var elementNode = visitor(context);

                if (node == null)
                {
                    node = elementNode;
                }
                else
                {
                    var binaryNode = new QsiBinaryExpressionNode
                    {
                        Operator = @operator
                    };

                    binaryNode.Left.SetValue(node);
                    binaryNode.Right.SetValue(elementNode);

                    node = binaryNode;
                }

                callback?.Invoke(context, node);
            }

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiColumnsDeclarationNode CreateAllColumnsDeclaration()
        {
            var columns = new QsiColumnsDeclarationNode();
            columns.Columns.Add(new QsiAllColumnNode());
            return columns;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiFunctionExpressionNode CreateFunction(string identifier)
        {
            return new()
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(identifier, false))
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiUnaryExpressionNode CreateUnary(string @operator, QsiExpressionNode expression)
        {
            var node = new QsiUnaryExpressionNode
            {
                Operator = @operator
            };

            node.Expression.SetValue(expression);

            return node;
        }

        #region Literal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiLiteralExpressionNode CreateDefaultLiteral()
        {
            return CreateLiteral(null, QsiDataType.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiLiteralExpressionNode CreateNullLiteral()
        {
            return CreateLiteral(null, QsiDataType.Null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiExpressionNode CreateConstantLiteral(object value)
        {
            return CreateLiteral(value, QsiDataType.Constant);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiLiteralExpressionNode CreateLiteral(string value)
        {
            return CreateLiteral(value, QsiDataType.String);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiLiteralExpressionNode CreateLiteral(long value)
        {
            return CreateLiteral(value, QsiDataType.Numeric);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiLiteralExpressionNode CreateLiteral(double value)
        {
            return CreateLiteral(value, QsiDataType.Decimal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiLiteralExpressionNode CreateLiteral(bool value)
        {
            return CreateLiteral(value, QsiDataType.Boolean);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiLiteralExpressionNode CreateLiteral(object value, QsiDataType type)
        {
            return new()
            {
                Type = type,
                Value = value
            };
        }
        #endregion

        public static QsiException NotSupportedTree(object tree)
        {
            return new(QsiError.NotSupportedTree, tree.GetType().FullName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QsiException NotSupportedFeature(string feature)
        {
            return new(QsiError.NotSupportedFeature, feature);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IQsiTreeNode> YieldChildren(params IQsiTreeNodeProperty<QsiTreeNode>[] properties)
        {
            return properties
                .Where(p => !p.IsEmpty)
                .Select(p => p.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IQsiTreeNode> YieldChildren(params IQsiTreeNode[] ndoes)
        {
            return ndoes
                .Where(n => n != null)
                .Select(n => n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IQsiTreeNode> YieldChildren(IEnumerable<IQsiTreeNode> source, IQsiTreeNodeProperty<QsiTreeNode> property)
        {
            if (property.IsEmpty)
                return source;

            return source.Append(property.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IQsiTreeNode> YieldChildren(IQsiTreeNodeProperty<QsiTreeNode> property, IEnumerable<IQsiTreeNode> source)
        {
            if (property.IsEmpty)
                return source;

            return source.Prepend(property.Value);
        }

        public static void VerifyTokenType(IToken token, int type)
        {
            if (token.Type != type)
                throw new ArgumentException(null, nameof(token));
        }
    }
}
