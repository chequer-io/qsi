using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal static class IdentifierVisitor
    {
        public static QsiQualifiedIdentifier VisitColumnRef(ColumnRef columnRef)
        {
            return VisitStrings(columnRef.fields.Cast<PgString>());
        }

        public static QsiQualifiedIdentifier VisitStrings(IEnumerable<PgString> values)
        {
            return new QsiQualifiedIdentifier(values.Select(v => new QsiIdentifier(v.str, false)));
        }
    }
}
