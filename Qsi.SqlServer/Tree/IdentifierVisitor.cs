using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;

namespace Qsi.SqlServer.Tree
{
    public sealed class IdentifierVisitor : VisitorBase
    {
        public IdentifierVisitor(IContext context) : base(context)
        {
        }
        
        public QsiQualifiedIdentifier VisitMultipartIdentifier(SqlMultipartIdentifier objectIdentifier)
        {
            return new QsiQualifiedIdentifier(objectIdentifier.Select(i => new QsiIdentifier(i.Value, false)));
        }

        public QsiIdentifier CreateIdentifier(SqlIdentifier identifier)
        {
            return new QsiIdentifier(identifier.Value, false);
        }
    }
}
