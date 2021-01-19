using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public sealed class MySqlProcedureAnalyseNode : QsiTreeNode
    {
        public int MaxElements { get; set; }

        public int? MaxMemory { get; set; }

        public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
    }
}
