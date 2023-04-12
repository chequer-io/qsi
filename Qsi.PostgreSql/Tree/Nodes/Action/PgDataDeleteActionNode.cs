using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgDataUpdateActionNode : QsiDataUpdateActionNode
{
    public QsiTreeNodeList<QsiTableNode> FromSources { get; }

    public PgDataUpdateActionNode()
    {
        FromSources = new QsiTreeNodeList<QsiTableNode>(this);
    }
}
