using System;
using Qsi.Tree;

namespace Qsi.Impala.Tree
{
    public class ImpalaJoinedTableNode : QsiJoinedTableNode, IImpalaTableNode
    {
        public string PlanHints { get; set; }

        public string TableSample { get; set; }
    }
}
