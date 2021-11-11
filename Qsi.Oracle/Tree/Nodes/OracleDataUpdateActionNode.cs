using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public sealed class OracleDataUpdateActionNode : QsiDataUpdateActionNode
    {
        public string Hint { get; set; }

        public QsiTreeNodeList<OracleSetValueExpressionNode> SetValueExpressions { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                foreach (var child in base.Children)
                    yield return child;

                foreach (var setValueExpression in SetValueExpressions)
                    yield return setValueExpression;
            }
        }

        public OracleDataUpdateActionNode()
        {
            SetValueExpressions = new QsiTreeNodeList<OracleSetValueExpressionNode>(this);
        }
    }
}
