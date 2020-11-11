using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Analyzers
{
    internal sealed class PhoenixSqlTableAnalyzer : QsiTableAnalyzer
    {
        private const string scopeFieldList = "field list";

        public PhoenixSqlTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override QsiTableColumn ResolveDeclaredColumn(TableCompileContext context, IQsiDeclaredColumnNode columnn)
        {
            context.ThrowIfCancellationRequested();

            if (columnn.Name.Level > 1)
            {
                var identifier = new QsiQualifiedIdentifier(columnn.Name[..^1]);
                var name = columnn.Name[^1];

                if (context.SourceTable == null)
                    throw new QsiException(QsiError.UnknownTableIn, identifier, scopeFieldList);

                var queue = new Queue<QsiTableStructure>();
                queue.Enqueue(context.SourceTable);

                while (queue.TryDequeue(out var table))
                {
                    if (table.HasIdentifier && Match(context, table, identifier))
                    {
                        QsiTableColumn[] columns = table.Columns
                            .Where(c => Match(c.Name, name))
                            .Take(2)
                            .ToArray();

                        if (columns.Length == 0)
                            throw new QsiException(QsiError.UnknownColumnIn, name.Value, scopeFieldList);

                        if (columns.Length > 1)
                            throw new QsiException(QsiError.AmbiguousColumnIn, columnn.Name, scopeFieldList);

                        return columns[0];
                    }

                    foreach (var refTable in table.References)
                    {
                        queue.Enqueue(refTable);
                    }
                }
            }

            return base.ResolveDeclaredColumn(context, columnn);
        }
    }
}
