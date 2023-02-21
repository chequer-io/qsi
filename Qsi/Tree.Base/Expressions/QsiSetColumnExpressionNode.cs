using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiSetColumnExpressionNode : QsiExpressionNode, IQsiSetColumnExpressionNode
    {
        public QsiQualifiedIdentifier Target { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Value { get; }

        public QsiTreeNodeList<QsiExpressionNode> Indirections { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value, Indirections);

        #region Explicit
        IQsiExpressionNode IQsiSetColumnExpressionNode.Value => Value.Value;
        #endregion

        public QsiSetColumnExpressionNode()
        {
            Value = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Indirections = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
