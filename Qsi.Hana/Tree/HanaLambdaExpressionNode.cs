using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaLambdaExpressionNode : QsiExpressionNode
    {
        public string Argument { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Body { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Body.IsEmpty)
                    yield return Body.Value;
            }
        }

        public HanaLambdaExpressionNode()
        {
            Body = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
}
