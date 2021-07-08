using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaDerivedTableNode : QsiDerivedTableNode
    {
        public string PlanHints { get; set; }

        public string TableSample { get; set; }

        public bool? IsDistinct { get; set; }
    }
}
