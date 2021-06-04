using System.Linq;
using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiDataUpdateActionNode : QsiActionNode, IQsiDataUpdateActionNode
    {
        public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

        public QsiTreeNodeProperty<QsiTableNode> Target { get; }

        public QsiTreeNodeProperty<QsiRowValueExpressionNode> Value { get; }

        public QsiTreeNodeList<QsiSetColumnExpressionNode> SetValues { get; }

        public override IEnumerable<IQsiTreeNode> Children =>
            TreeHelper.YieldChildren(Directives, Target)
                .Concat(SetValues);

        #region Explicit
        IQsiTableNode IQsiDataUpdateActionNode.Target => Target.Value;

        IQsiRowValueExpressionNode IQsiDataUpdateActionNode.Value => Value.Value;

        IQsiSetColumnExpressionNode[] IQsiDataUpdateActionNode.SetValues => SetValues.Cast<IQsiSetColumnExpressionNode>().ToArray();
        #endregion

        public QsiDataUpdateActionNode()
        {
            Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
            Target = new QsiTreeNodeProperty<QsiTableNode>(this);
            Value = new QsiTreeNodeProperty<QsiRowValueExpressionNode>(this);
            SetValues = new QsiTreeNodeList<QsiSetColumnExpressionNode>(this);
        }
    }
}
