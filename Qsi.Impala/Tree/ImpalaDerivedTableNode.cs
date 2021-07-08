using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaDerivedTableNode : QsiDerivedTableNode, IImpalaTableNode
    {
        public string PlanHints { get; set; }

        public string TableSample { get; set; }

        public bool? IsDistinct { get; set; }
    }
}
