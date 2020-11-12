using System.Collections.Generic;
using PhoenixSql;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Internal
{
    internal sealed class PDeleteActionNode : QsiDataDeleteActionNode
    {
        public IReadOnlyList<HintNode.Types.Entry> Hints { get; set; }
    }
}
