using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree
{
    public class TrinoMeasureExpressionNode : QsiExpressionNode
    {
        public QsiIdentifier Identifier { get; set; }

        public QsiTreeNodeProperty<TrinoWindowExpressionNode> Over { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Over);

        public TrinoMeasureExpressionNode()
        {
            Over = new QsiTreeNodeProperty<TrinoWindowExpressionNode>(this);
        }
    }
}
