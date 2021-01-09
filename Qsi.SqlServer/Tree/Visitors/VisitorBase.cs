namespace Qsi.SqlServer.Tree
{
    internal abstract class VisitorBase
    {
        protected TableVisitor TableVisitor => _visitorContext.TableVisitor;

        protected ExpressionVisitor ExpressionVisitor => _visitorContext.ExpressionVisitor;

        protected IdentifierVisitor IdentifierVisitor => _visitorContext.IdentifierVisitor;

        private readonly IVisitorContext _visitorContext;

        protected VisitorBase(IVisitorContext visitorContext)
        {
            _visitorContext = visitorContext;
        }
    }
}
