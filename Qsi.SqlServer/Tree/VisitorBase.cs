namespace Qsi.SqlServer.Tree
{
    public abstract class VisitorBase
    {
        protected TableVisitor TableVisitor => _context.TableVisitor;

        protected ExpressionVisitor ExpressionVisitor => _context.ExpressionVisitor;

        protected IdentifierVisitor IdentifierVisitor => _context.IdentifierVisitor;

        private readonly IVisitorContext _context;

        protected VisitorBase(IVisitorContext context)
        {
            _context = context;
        }
    }
}
