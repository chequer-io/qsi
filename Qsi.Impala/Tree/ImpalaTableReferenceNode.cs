using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaTableReferenceNode : QsiTableReferenceNode, IImpalaTableNode
    {
        public string TableSample { get; set; }

        public string PlanHints { get; set; }
    }
}
