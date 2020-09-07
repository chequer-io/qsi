using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Qsi.Data;

namespace Qsi.SqlServer.Tree
{
    internal static class IdentifierVisitor
    {
        public static QsiQualifiedIdentifier VisitMultipartIdentifier(SqlMultipartIdentifier objectIdentifier)
        {
            return new QsiQualifiedIdentifier(objectIdentifier.Select(i => new QsiIdentifier(i.Value, false)));
        }

        public static QsiQualifiedIdentifier VisitScalarExpression(SqlScalarExpression scalarExpression)
        {
            switch (scalarExpression)
            {
                case SqlScalarRefExpression scalarRefExpression:
                    return VisitMultipartIdentifier(scalarRefExpression.MultipartIdentifier);
            }

            return null;
        }
    }
}
