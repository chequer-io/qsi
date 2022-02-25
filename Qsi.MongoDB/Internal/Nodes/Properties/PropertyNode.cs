using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

public class PropertyNode : BaseNode, INode
{
    public IExpressionNode Key { get; set; }

    // BaseExpression, Pattern
    public INode Value { get; set; }

    public string Kind { get; set; }

    public bool Method { get; set; }

    public bool Shorthand { get; set; }

    public bool Computed { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Key;
            yield return Value;
        }
    }
}
