using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena.Common
{
    public class AthenaExistsExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiTableExpressionNode> Query { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Query);

        public AthenaExistsExpressionNode()
        {
            Query = new QsiTreeNodeProperty<QsiTableExpressionNode>(this);
        }
    }
}
