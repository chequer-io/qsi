using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public sealed class OracleDataUpdateActionNode : QsiActionNode, IQsiDataUpdateActionNode
    {
        public string Hint { get; set; }

        public QsiTreeNodeProperty<QsiTableNode> Target { get; }

        public QsiTreeNodeList<OracleSetValueExpressionNode> SetValueExpressions { get; }

        public override IEnumerable<IQsiTreeNode> Children => SetValueExpressions;

        #region Explicit
        IQsiTableNode IQsiDataUpdateActionNode.Target => Target.Value;

        IQsiRowValueExpressionNode IQsiDataUpdateActionNode.Value => null;

        IQsiSetColumnExpressionNode[] IQsiDataUpdateActionNode.SetValues => null;
        #endregion

        public OracleDataUpdateActionNode()
        {
            Target = new QsiTreeNodeProperty<QsiTableNode>(this);
            SetValueExpressions = new QsiTreeNodeList<OracleSetValueExpressionNode>(this);
        }
    }
}
