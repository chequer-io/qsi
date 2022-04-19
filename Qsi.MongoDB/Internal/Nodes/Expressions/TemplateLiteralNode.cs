using System.Collections.Generic;

namespace Qsi.MongoDB.Internal.Nodes;

internal class TemplateLiteralNode : BaseNode, IExpressionNode
{
    public TemplateElementNode[] Quasis { get; set; }

    public IExpressionNode[] Expressions { get; set; }

    public override IEnumerable<INode> Children
    {
        get
        {
            foreach (var quasi in Quasis)
                yield return quasi;

            foreach (var expression in Expressions)
                yield return expression;
        }
    }
}
