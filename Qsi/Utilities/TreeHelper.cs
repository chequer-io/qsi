using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Utilities
{
    public static class TreeHelper
    {
        public static TNode Create<TNode>(Action<TNode> action) where TNode : QsiTreeNode, new()
        {
            var node = new TNode();
            action(node);
            return node;
        }

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

        public static QsiColumnsDeclarationNode CreateAllColumnsDeclaration()
        {
            var columns = new QsiColumnsDeclarationNode();
            columns.Columns.Add(new QsiAllColumnNode());
            return columns;
        }

        public static QsiFunctionExpressionNode CreateFunction(string identifier)
        {
            return new QsiFunctionExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(identifier, false))
            };
        }

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
        public static QsiLiteralExpressionNode CreateDefaultLiteral()
        {
            return CreateLiteral(null, QsiDataType.Default);
        }

        public static QsiLiteralExpressionNode CreateNullLiteral()
        {
            return CreateLiteral(null, QsiDataType.Null);
        }

        public static QsiExpressionNode CreateConstantLiteral(object value)
        {
            return CreateLiteral(value, QsiDataType.Constant);
        }

        public static QsiLiteralExpressionNode CreateLiteral(string value)
        {
            return CreateLiteral(value, QsiDataType.String);
        }

        public static QsiLiteralExpressionNode CreateLiteral(long value)
        {
            return CreateLiteral(value, QsiDataType.Numeric);
        }

        public static QsiLiteralExpressionNode CreateLiteral(double value)
        {
            return CreateLiteral(value, QsiDataType.Decimal);
        }

        public static QsiLiteralExpressionNode CreateLiteral(bool value)
        {
            return CreateLiteral(value, QsiDataType.Boolean);
        }

        public static QsiLiteralExpressionNode CreateLiteral(object value, QsiDataType type)
        {
            return new QsiLiteralExpressionNode
            {
                Type = type,
                Value = value
            };
        }
        #endregion

        public static QsiException NotSupportedTree(object tree)
        {
            return new QsiException(QsiError.NotSupportedTree, tree.GetType().FullName);
        }

        public static QsiException NotSupportedFeature(string feature)
        {
            return new QsiException(QsiError.NotSupportedFeature, feature);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<IQsiTreeNode> YieldChildren(params IQsiTreeNodeProperty<QsiTreeNode>[] properties)
        {
            return properties
                .Where(p => !p.IsEmpty)
                .Select(p => p.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<IQsiTreeNode> YieldChildren(params IQsiTreeNode[] ndoes)
        {
            return ndoes
                .Where(n => n != null)
                .Select(n => n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<IQsiTreeNode> YieldChildren(IEnumerable<IQsiTreeNode> source, IQsiTreeNodeProperty<QsiTreeNode> property)
        {
            if (property.IsEmpty)
                return source;

            return source.Append(property.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<IQsiTreeNode> YieldChildren(IQsiTreeNodeProperty<QsiTreeNode> property, IEnumerable<IQsiTreeNode> source)
        {
            if (property.IsEmpty)
                return source;

            return source.Prepend(property.Value);
        }
    }
}
