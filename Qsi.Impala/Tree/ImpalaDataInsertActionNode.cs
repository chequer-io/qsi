using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaDataInsertActionNode : QsiDataInsertActionNode
    {
        public string PlanHints { get; set; }
    }
}
