using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class AssignmentExpressionNode : BaseNode, IExpressionNode
{
    public string Operator { get; set; }

    // Pattern, MemberExpression
    public INode Left { get; set; }

    public IExpressionNode Right { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Left;
            yield return Right;
        }
    }
}
