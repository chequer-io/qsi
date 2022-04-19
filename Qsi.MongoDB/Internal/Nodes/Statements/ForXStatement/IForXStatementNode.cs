namespace Qsi.MongoDB.Internal.Nodes;

internal interface IForXStatementNode : IStatementNode
{
    public INode Left { get; set; }

    public IExpressionNode Right { get; set; }

    public IStatementNode Body { get; set; }
}
