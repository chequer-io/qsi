using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgDerivedTableNode : QsiDerivedTableNode
{
    public bool IsDistinct { get; set; }

    public QsiTreeNodeList<QsiExpressionNode> DistinctExpressions { get; }

    public override IEnumerable<IQsiTreeNode> Children => base.Children.Concat(DistinctExpressions);

    public PgDerivedTableNode()
    {
        DistinctExpressions = new QsiTreeNodeList<QsiExpressionNode>(this);
    }
}
