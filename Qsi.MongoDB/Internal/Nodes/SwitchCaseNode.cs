using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class SwitchCaseNode : BaseNode, INode
{
    public IExpressionNode Test { get; set; }

    public IStatementNode[] Consequent { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Test;

            foreach (var node in Consequent)
                yield return node;
        }
    }
}
