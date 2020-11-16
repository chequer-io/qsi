using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiDerivedColumnNode : QsiColumnNode, IQsiDerivedColumnNode
    {
        public QsiTreeNodeProperty<QsiColumnNode> Column { get; }

        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Column, Expression, Alias);

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
