namespace Qsi.JSql.Tree
{
    public abstract class JSqlVisitorBase
    {
        protected JSqlTableVisitor TableVisitor => _set.TableVisitor;

        protected JSqlExpressionVisitor ExpressionVisitor => _set.ExpressionVisitor;

        protected JSqlIdentifierVisitor IdentifierVisitor => _set.IdentifierVisitor;

        private readonly IJSqlVisitorSet _set;

        protected JSqlVisitorBase(IJSqlVisitorSet set)
        {
            _set = set;
        }
    }
}
