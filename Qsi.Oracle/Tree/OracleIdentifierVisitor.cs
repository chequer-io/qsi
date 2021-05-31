using System.Linq;
using net.sf.jsqlparser.expression;
using net.sf.jsqlparser.schema;
using Qsi.Data;
using Qsi.JSql.Tree;

namespace Qsi.Oracle.Tree
{
    internal sealed class OracleIdentifierVisitor : JSqlIdentifierVisitor
    {
        public OracleIdentifierVisitor(IJSqlVisitorSet set) : base(set)
        {
        }

        public override QsiIdentifier Create(string value)
        {
            return PatchQualifiedIdentifier(base.Create(value));
        }

        public override QsiQualifiedIdentifier VisitMultiPartName(MultiPartName name)
        {
            return PatchQualifiedIdentifier(base.VisitMultiPartName(name));
        }

        public override QsiQualifiedIdentifier VisitFunction(Function function)
        {
            return PatchQualifiedIdentifier(base.VisitFunction(function));
        }

        public override QsiQualifiedIdentifier VisitAnalyticExpression(AnalyticExpression expression)
        {
            return PatchQualifiedIdentifier(base.VisitAnalyticExpression(expression));
        }

        public override QsiQualifiedIdentifier VisitNextValExpression(NextValExpression expression)
        {
            return PatchQualifiedIdentifier(base.VisitNextValExpression(expression));
        }

        private QsiIdentifier PatchQualifiedIdentifier(QsiIdentifier identifier)
        {
            if (identifier.IsEscaped)
                return identifier;

            return new QsiIdentifier(identifier.Value.ToUpper(), false);
        }

        private QsiQualifiedIdentifier PatchQualifiedIdentifier(QsiQualifiedIdentifier qualifiedIdentifier)
        {
            QsiIdentifier[] identifiers = qualifiedIdentifier
                .Select(PatchQualifiedIdentifier)
                .ToArray();

            return new QsiQualifiedIdentifier(identifiers);
        }
    }
}
