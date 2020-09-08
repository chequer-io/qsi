using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.SqlServer.Tree
{
    internal static class IdentifierVisitor
    {
        public static QsiQualifiedIdentifier VisitMultipartIdentifier(SqlMultipartIdentifier objectIdentifier)
        {
            return new QsiQualifiedIdentifier(objectIdentifier.Select(i => new QsiIdentifier(i.Value, false)));
        }

        public static QsiIdentifier CreateIdentifier(SqlIdentifier identifier)
        {
            return new QsiIdentifier(identifier.Value, false);
        }
    }
}
