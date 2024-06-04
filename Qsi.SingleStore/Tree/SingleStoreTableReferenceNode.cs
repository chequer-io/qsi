using Qsi.Data;
using Qsi.Tree;

namespace Qsi.SingleStore.Tree;

public sealed class SingleStoreTableReferenceNode : QsiTableReferenceNode
{
    public QsiIdentifier[] Partitions { get; set; }
}
