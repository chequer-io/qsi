using System.Collections.Generic;
using Qsi.PostgreSql.Extensions;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgSetColumnExpressionNode : QsiSetColumnExpressionNode
{
    public QsiTreeNodeList<QsiExpressionNode?> Indirections { get; }

    public override IEnumerable<IQsiTreeNode> Children => base.Children.ConcatWhereNotNull(Indirections);

    public PgSetColumnExpressionNode()
    {
        Indirections = new QsiTreeNodeList<QsiExpressionNode?>(this);
    }
}
