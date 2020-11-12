using System.Collections.Generic;
using PhoenixSql;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Internal
{
    internal sealed class PUpsertActionNode : QsiDataInsertActionNode
    {
        public IReadOnlyList<HintNode.Types.Entry> Hints { get; set; }

        public QsiQualifiedIdentifier[] DynamicColumns { get; set; }
    }
}
