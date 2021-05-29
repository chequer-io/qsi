namespace Qsi.JSql.Tree
{
    public interface IJSqlVisitorSet
    {
        JSqlTableVisitor TableVisitor { get; }

        JSqlExpressionVisitor ExpressionVisitor { get; }

        JSqlIdentifierVisitor IdentifierVisitor { get; }
    }
}
