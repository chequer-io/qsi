using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;

namespace Qsi.SqlServer.Tree
{
    public sealed class IdentifierVisitor : VisitorBase
    {
        public IdentifierVisitor(IContext context) : base(context)
        {
        }
        
        public QsiQualifiedIdentifier ConcatIdentifier(MultiPartIdentifier multiIdentifier, Identifier identifier)
        {
            return CreateQualifiedIdentifier(multiIdentifier.Identifiers.Concat(new[] { identifier }));
        }

        public QsiQualifiedIdentifier ConcatIdentifier(MultiPartIdentifier firstMultiIdentifier, MultiPartIdentifier secondMultiIdentifier)
        {
            return CreateQualifiedIdentifier(firstMultiIdentifier.Identifiers.Concat(secondMultiIdentifier.Identifiers));
        }

        public QsiQualifiedIdentifier CreateQualifiedIdentifier(MultiPartIdentifier multiIdentifier)
        {
            return CreateQualifiedIdentifier(multiIdentifier.Identifiers);
        }

        public QsiQualifiedIdentifier CreateQualifiedIdentifier(IEnumerable<Identifier> identifiers)
        {
            return new QsiQualifiedIdentifier(identifiers.Select(CreateIdentifier));
        }
        
        public QsiQualifiedIdentifier CreateQualifiedIdentifier(params Identifier[] identifiers)
        {
            return new QsiQualifiedIdentifier(identifiers.Select(CreateIdentifier));
        }
        
        public QsiIdentifier CreateIdentifier(Identifier identifier)
        {
            return new QsiIdentifier(identifier.Value, false);
        }
    }
}
