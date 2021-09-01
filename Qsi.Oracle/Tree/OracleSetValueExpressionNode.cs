using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleSetValueExpressionNode : QsiTreeNode, IQsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiSetColumnExpressionNode> SetValue { get; }

        public QsiTreeNodeProperty<OracleSetColumnsExpressionNode> SetValueFromTable { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(SetValue, SetValueFromTable);

        public OracleSetValueExpressionNode()
        {
            SetValue = new QsiTreeNodeProperty<QsiSetColumnExpressionNode>(this);
            SetValueFromTable = new QsiTreeNodeProperty<OracleSetColumnsExpressionNode>(this);
        }
    }
}
