using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Cql.Tree;

public sealed class CqlUsingExpressionNode : QsiExpressionNode
{
    public CqlUsingType Type { get; set; }

    public int Value { get; set; }

    public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
}

public enum CqlUsingType
{
    Timestamp,
    Ttl
}