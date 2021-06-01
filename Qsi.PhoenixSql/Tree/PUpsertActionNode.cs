using System.Collections.Generic;
using PhoenixSql;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Tree
{
    internal sealed class PUpsertActionNode : QsiDataInsertActionNode
    {
        public IReadOnlyList<HintNode.Types.Entry> Hints { get; set; }
    }
}
