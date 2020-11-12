using System.Collections.Generic;
using PhoenixSql;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Internal
{
    internal sealed class PDerivedTableNode : QsiDerivedTableNode
    {
        public IReadOnlyList<HintNode.Types.Entry> Hints { get; set; }
    }
}
