using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.PhoenixSql.Internal;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Analyzers
{
    internal sealed class PhoenixSqlTableAnalyzer : QsiTableAnalyzer
    {
        private const string scopeFieldList = "field list";

        public PhoenixSqlTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override async ValueTask<QsiTableStructure> BuildTableAccessStructure(TableCompileContext context, IQsiTableAccessNode table)
        {
            var structure = await base.BuildTableAccessStructure(context, table);

            if (table is PDynamicTableAccessNode dynamicTableAccessNode)
            {
                foreach (var dynamicColumn in dynamicTableAccessNode.DynamicColumns)
                {
                    var column = structure.NewColumn();
                    column.Name = dynamicColumn.Name[^1];
                    column.IsDynamic = true;
                }
            }

            return structure;
        }

        protected override QsiTableColumn ResolveDeclaredColumn(TableCompileContext context, IQsiDeclaredColumnNode column)
        {
            context.ThrowIfCancellationRequested();

            if (column.Name.Level > 1)
            {
                var identifier = new QsiQualifiedIdentifier(column.Name[..^1]);
                var name = column.Name[^1];

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
                            throw new QsiException(QsiError.AmbiguousColumnIn, column.Name, scopeFieldList);

                        return columns[0];
                    }

                    foreach (var refTable in table.References)
                    {
                        queue.Enqueue(refTable);
                    }
                }
            }

            return base.ResolveDeclaredColumn(context, column);
        }
    }
}
