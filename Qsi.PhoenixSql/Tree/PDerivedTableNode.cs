using System.Collections.Generic;
using PhoenixSql;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PDerivedTableNode : QsiDerivedTableNode
    {
        public IReadOnlyList<HintNode.Types.Entry> Hints { get; set; }
    }
}
