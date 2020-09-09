using System;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.Tree.Base;

namespace Qsi.Utilities
{
    public class TreeHelper
    {
        public static TNode Create<TNode>(Action<TNode> action) where TNode : QsiTreeNode, new()
        {
            var node = new TNode();
            action(node);
            return node;
        }

        public static QsiLogicalExpressionNode CreateLogicalExpression<TContext>(
            string @operator,
            TContext left,
            TContext right,
            Func<TContext, QsiExpressionNode> visitor)
        {
            return Create<QsiLogicalExpressionNode>(n =>
            {
                n.Operator = @operator;
                n.Left.SetValue(visitor(left));
                n.Right.SetValue(visitor(right));
            });
        }

        public static QsiColumnsDeclarationNode CreateAllColumnsDeclaration()
        {
            var columns = new QsiColumnsDeclarationNode();
            columns.Columns.Add(new QsiAllColumnNode());
            return columns;
        }

        public static QsiFunctionAccessExpressionNode CreateFunctionAccess(string identifier)
        {
            return new QsiFunctionAccessExpressionNode
            {
                Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(identifier, false))
            };
        }

        public static QsiException NotSupportedTree(object tree)
        {
            return new QsiException(QsiError.NotSupportedTree, tree.GetType().FullName);
        }

        public static QsiException NotSupportedFeature(string feature)
        {
            return new QsiException(QsiError.NotSupportedFeature, feature);
        }
    }
}
