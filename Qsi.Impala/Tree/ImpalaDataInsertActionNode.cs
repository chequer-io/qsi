using Qsi.Tree;

namespace Qsi.Impala.Tree;

public class ImpalaDataInsertActionNode : QsiDataInsertActionNode
{
    public string PlanHints { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Partition { get; }

    public ImpalaDataInsertActionNode()
    {
        Partition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}