using Qsi.Tree;

namespace Qsi.Athena.Tree;

public sealed class AthenaDerivedTableNode : QsiDerivedTableNode
{
    public AthenaSetQuantifier? SetQuantifier { get; set; }
}
