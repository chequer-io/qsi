using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.Internal.PG10.Types;

namespace Qsi.PostgreSql.Tree.PG10
{
    internal class PgIdentifierVisitor : PgVisitorBase
    {
        public PgIdentifierVisitor(IPgVisitorSet set) : base(set)
        {
        }

        public QsiQualifiedIdentifier VisitStrings(IEnumerable<PgString> values)
        {
            return new(values.Select(v => new QsiIdentifier(v.str, false)));
        }

        public QsiQualifiedIdentifier VisitRangeVar(RangeVar var)
        {
            string[] names =
            {
                var.catalogname,
                var.schemaname,
                var.relname
            };

            int start = Array.FindIndex(names, n => !string.IsNullOrEmpty(n));
            int end = Array.FindLastIndex(names, n => !string.IsNullOrEmpty(n));

            if (start == -1 || end == -1)
                throw new QsiException(QsiError.SyntaxError, $"Invalid identifier '{string.Join(".", names.Select(n => n ?? "null"))}'");

            return new QsiQualifiedIdentifier(names[start..(end + 1)].Select(n => new QsiIdentifier(n, false)));
        }
    }
}
