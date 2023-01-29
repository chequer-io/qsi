using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgOrderExpressionNode : QsiOrderExpressionNode
{
    public PgOrderExpressionNode()
    {
        UsingOperator = new QsiTreeNodeList<QsiExpressionNode?>(this);
    }

    public bool SoryByUsing { get; set; }

    public SortByNulls SortByNulls { get; set; }

    public QsiTreeNodeList<QsiExpressionNode?> UsingOperator { get; }
}
