namespace Qsi.JSql.Tree
{
    public interface IJSqlVisitorContext
    {
        JSqlTableVisitor TableVisitor { get; }

        JSqlExpressionVisitor ExpressionVisitor { get; }

        JSqlIdentifierVisitor IdentifierVisitor { get; }
    }
}
