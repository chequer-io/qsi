using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiDerivedColumnNode : QsiColumnNode, IQsiDerivedColumnNode
    {
        public QsiTreeNodeProperty<QsiColumnNode> Column { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Column.IsEmpty)
                    yield return Column.Value;

                if (!Expression.IsEmpty)
                    yield return Expression.Value;

                if (!Alias.IsEmpty)
                    yield return Alias.Value;
            }
        }

        #region Explicit
        IQsiColumnNode IQsiDerivedColumnNode.Column => Column.Value;

        IQsiExpressionNode IQsiDerivedColumnNode.Expression => Expression.Value;

        IQsiAliasNode IQsiDerivedColumnNode.Alias => Alias.Value;
        #endregion

        public QsiDerivedColumnNode()
        {
            Column = new QsiTreeNodeProperty<QsiColumnNode>(this);
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
