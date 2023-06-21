using Qsi.Tree;

namespace Qsi.Impala.Tree;

public class ImpalaJoinedTableNode : QsiJoinedTableNode
{
    public string PlanHints { get; set; }
}