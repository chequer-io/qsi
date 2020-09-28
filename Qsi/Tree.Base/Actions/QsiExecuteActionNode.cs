using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiExecuteActionNode : QsiActionNode, IQsiExecuteActionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }

        public QsiTreeNodeProperty<QsiMultipleExpressionNode> Variables { get; }

        IQsiMultipleExpressionNode IQsiExecuteActionNode.Variables => Variables.Value;

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Variables.Value);

        public QsiExecuteActionNode()
        {
            Variables = new QsiTreeNodeProperty<QsiMultipleExpressionNode>(this);
        }
    }
}
