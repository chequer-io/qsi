using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaTableReferenceNode : QsiTableReferenceNode
    {
        public string PlanHints { get; set; }

        public string TableSample { get; set; }
    }
}
