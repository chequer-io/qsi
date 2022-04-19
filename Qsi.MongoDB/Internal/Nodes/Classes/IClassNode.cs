namespace Qsi.MongoDB.Internal.Nodes;

internal interface IClassNode : INode
{
    IExpressionNode SuperClass { get; set; }

    ClassBodyNode Body { get; set; }
}
