namespace Qsi.JSql.Tree
{
    public abstract class JSqlVisitorBase
    {
        protected JSqlTableVisitor TableVisitor => _context.TableVisitor;

        protected JSqlExpressionVisitor ExpressionVisitor => _context.ExpressionVisitor;

        protected JSqlIdentifierVisitor IdentifierVisitor => _context.IdentifierVisitor;

        private readonly IJSqlVisitorContext _context;

        protected JSqlVisitorBase(IJSqlVisitorContext context)
        {
            _context = context;
        }
    }
}
