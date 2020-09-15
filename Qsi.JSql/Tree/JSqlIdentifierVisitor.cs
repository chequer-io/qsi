using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.schema;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.JSql.Tree
{
    public class JSqlIdentifierVisitor : JSqlVisitorBase
    {
        public JSqlIdentifierVisitor(IJSqlVisitorContext context) : base(context)
        {
        }

        public virtual QsiIdentifier Create(string value)
        {
            return new QsiIdentifier(value, IdentifierUtility.IsEscaped(value));
        }

        public virtual QsiIdentifier VisitAlias(Alias alias)
        {
            return Create(alias.getName());
        }

        public virtual QsiQualifiedIdentifier VisitMultiPartName(MultiPartName name)
        {
            return new QsiQualifiedIdentifier(IdentifierUtility.Parse(name.getFullyQualifiedName()));
        }

        public virtual QsiQualifiedIdentifier VisitFunction(Function function)
        {
            return new QsiQualifiedIdentifier(IdentifierUtility.Parse(function.getName()));
        }

        public virtual QsiQualifiedIdentifier VisitAnalyticExpression(AnalyticExpression expression)
        {
            return new QsiQualifiedIdentifier(IdentifierUtility.Parse(expression.getName()));
        }

        public virtual QsiQualifiedIdentifier VisitNextValExpression(NextValExpression expression)
        {
            return new QsiQualifiedIdentifier(IdentifierUtility.Parse(expression.getName()));
        }
    }
}
