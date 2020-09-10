namespace Qsi.SqlServer.Tree
{
    public interface IContext
    {
        TableVisitor TableVisitor { get; }
        
        ExpressionVisitor ExpressionVisitor { get; }
        
        IdentifierVisitor IdentifierVisitor { get; }
        
        SqlServerParser ScriptParser { get; }
    }
}
