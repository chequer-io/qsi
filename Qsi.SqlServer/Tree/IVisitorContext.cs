namespace Qsi.SqlServer.Tree
{
    public interface IVisitorContext
    {
        TableVisitor TableVisitor { get; }
        
        ExpressionVisitor ExpressionVisitor { get; }
        
        IdentifierVisitor IdentifierVisitor { get; }
    }
}
