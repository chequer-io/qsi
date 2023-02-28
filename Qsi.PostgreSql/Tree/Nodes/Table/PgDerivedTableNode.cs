using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgDerivedTableNode : QsiDerivedTableNode
{
    public QsiTreeNodeList<QsiExpressionNode> DisinictExpressions { get; }

    public override IEnumerable<IQsiTreeNode> Children => base.Children.Concat(DisinictExpressions);

    public PgDerivedTableNode()
    {
        DisinictExpressions = new QsiTreeNodeList<QsiExpressionNode>(this);
    }
}
