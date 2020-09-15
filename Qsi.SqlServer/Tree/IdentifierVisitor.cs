using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;

namespace Qsi.SqlServer.Tree
{
    public class IdentifierVisitor : VisitorBase
    {
        public IdentifierVisitor(IContext context) : base(context)
        {
        }
        
        
        public QsiQualifiedIdentifier CreateQualifiedIdentifier(MultiPartIdentifier objectIdentifier)
        {
            return new QsiQualifiedIdentifier(objectIdentifier.Identifiers.Select(CreateIdentifier));
        }

        public QsiIdentifier CreateIdentifier(Identifier identifier)
        {
            return new QsiIdentifier(identifier.Value, false);
        }
    }
}
