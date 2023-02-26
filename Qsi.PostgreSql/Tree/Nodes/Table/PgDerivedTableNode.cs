using System.Collections.Generic;
using Qsi.PostgreSql.Extensions;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgDerivedTableNode : QsiDerivedTableNode
{
    public QsiTreeNodeList<QsiExpressionNode?> DisinictExpressions { get; }

    public override IEnumerable<IQsiTreeNode> Children => base.Children.ConcatWhereNotNull(DisinictExpressions);

    public PgDerivedTableNode()
    {
        DisinictExpressions = new QsiTreeNodeList<QsiExpressionNode?>(this);
    }
}
