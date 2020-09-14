namespace Qsi.SqlServer.Tree
{
    public abstract class VisitorBase
    {
        protected TableVisitor_Legacy TableVisitor => _context.TableVisitor;

        protected ExpressionVisitor_Legacy ExpressionVisitor => _context.ExpressionVisitor;

        protected IdentifierVisitor_Legacy IdentifierVisitor => _context.IdentifierVisitor;

        protected SqlServerParser SqlParser => _context.SqlParser;
        
        private readonly IContext _context;

        protected VisitorBase(IContext context)
        {
            _context = context;
        }
    }
}
