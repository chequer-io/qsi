namespace Qsi.SqlServer.Tree
{
    public interface IContext
    {
        TableVisitor_Legacy TableVisitor { get; }
        
        ExpressionVisitor_Legacy ExpressionVisitor { get; }
        
        IdentifierVisitor_Legacy IdentifierVisitor { get; }
        
        SqlServerParser SqlParser { get; }
    }
}
