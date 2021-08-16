using System.Linq;
using Qsi.Data;
using Qsi.Oracle.Internal;
using Qsi.Tree;

namespace Qsi.Oracle.Tree.Visitors
{
    using static OracleParserInternal;

    internal static class IdentifierVisitor
    {
        public static QsiIdentifier VisitIdentifierFragment(IdentifierFragmentContext context)
        {
            var text = context.GetText();
            return new QsiIdentifier(text, text.StartsWith('"'));
        }

        // [[database] .] object [@ dblink]
        public static QsiQualifiedIdentifier VisitFullObjectPath(FullObjectPathContext context)
        {
            var dbIdentifier = context.identifierFragment() != null ?
                VisitIdentifierFragment(context.identifierFragment())
                : null;

            var objIdentifier = VisitIdentifier(context.identifier());

            return dbIdentifier != null ?
                new QsiQualifiedIdentifier(dbIdentifier, objIdentifier) :
                new QsiQualifiedIdentifier(objIdentifier);
        }

        public static QsiIdentifier VisitIdentifier(IdentifierContext context)
        {
            if (context.dblink() != null)
                throw new QsiException(QsiError.NotSupportedFeature, "dblink");

            return VisitIdentifierFragment(context.identifierFragment());
        }

        public static QsiIdentifier VisitAlias(AliasContext context)
        {
            return VisitIdentifier(context.identifier());
        }

        public static QsiQualifiedIdentifier CreateQualifiedIdentifier(params IdentifierContext[] identifiers)
        {
            return new(identifiers.Select(VisitIdentifier));
        }
    }
}
