using System.Collections.Generic;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.Utilities;
using PgString = PgQuery.String;

namespace Qsi.PostgreSql.Extensions;

internal static class IdentifierExtension
{
    public static PgString ToPgString(this QsiIdentifier identifier)
    {
        var value = identifier.IsEscaped
            ? IdentifierUtility.Unescape(identifier.Value)
            : identifier.Value;

        return new PgString
        {
            Sval = value
        };
    }

    public static IEnumerable<PgString> ToPgString(this IEnumerable<QsiIdentifier> identifier)
    {
        return identifier.Select(ToPgString);
    }

    public static IEnumerable<Node> ToPgStringNode(this QsiQualifiedIdentifier? qualifiedIdentifier)
    {
        return qualifiedIdentifier?.ToPgString().ToNode() ?? Enumerable.Empty<Node>();
    }
}
