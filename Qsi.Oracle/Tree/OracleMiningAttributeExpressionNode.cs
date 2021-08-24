using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleMiningAttributeExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleMiningAttributeExpressionNode()
        {
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
        }
    }
}
