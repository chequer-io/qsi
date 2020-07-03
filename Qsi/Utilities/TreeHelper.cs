using System;
using Antlr4.Runtime.Tree;
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

        public static Exception NotSupportedTree(IParseTree tree)
        {
            return new QsiException($"Not supported tree type: {tree.GetType().FullName}");
        }
    }
}
