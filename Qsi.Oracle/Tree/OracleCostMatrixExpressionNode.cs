using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree
{
    public class OracleCostMatrixExpressionNode : QsiExpressionNode
    {
        public bool IsSpecifiedModel { get; set; } = true;

        public bool IsAuto { get; set; }

        public QsiTreeNodeProperty<QsiExpressionNode> Models { get; }

        public QsiTreeNodeList<QsiExpressionNode> ModelValues { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Models, ModelValues);

        public OracleCostMatrixExpressionNode()
        {
            Models = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            ModelValues = new QsiTreeNodeList<QsiExpressionNode>(this);
        }
    }
}
