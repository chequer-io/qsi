using Qsi.Cql.Schema;
using Qsi.Tree;

namespace Qsi.Cql.Tree;

public sealed class CqlTypeExpressionNode : QsiTypeExpressionNode
{
    public CqlType Type { get; set; }
}