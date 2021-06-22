using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.PhoenixSql.Tree;
using Qsi.Tree;

namespace Qsi.PhoenixSql.Analyzers
{
    internal sealed class PhoenixSqlTableAnalyzer : QsiTableAnalyzer
    {
        private const string scopeFieldList = "field list";

        public PhoenixSqlTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override async ValueTask<QsiTableStructure> BuildViewDefinitionStructure(TableCompileContext context, IQsiViewDefinitionNode viewDefinition)
        {
            var structure = await base.BuildViewDefinitionStructure(context, viewDefinition);

            if (viewDefinition is IDynamicColumnsNode dynamicColumnsNode)
            {
                structure = structure.Clone();
                PatchDynamicColumns(structure, dynamicColumnsNode);
            }

            return structure;
        }

        protected override async ValueTask<QsiTableStructure> BuildDerivedTableStructure(TableCompileContext context, IQsiDerivedTableNode table)
        {
            var structure = await base.BuildDerivedTableStructure(context, table);

            if (table is IDynamicColumnsNode dynamicColumnsNode)
            {
                structure = structure.Clone();
                PatchDynamicColumns(structure, dynamicColumnsNode);
            }

            return structure;
        }

        protected override async ValueTask<QsiTableStructure> BuildTableReferenceStructure(TableCompileContext context, IQsiTableReferenceNode table)
        {
            var structure = await base.BuildTableReferenceStructure(context, table);

            if (table is IDynamicColumnsNode dynamicColumnsNode)
            {
                structure = structure.Clone();
                PatchDynamicColumns(structure, dynamicColumnsNode);
            }

            return structure;
        }

        private void PatchDynamicColumns(QsiTableStructure structure, IDynamicColumnsNode dynamicColumnsNode)
        {
            foreach (var dynamicColumn in dynamicColumnsNode.DynamicColumns.Columns.Cast<PDynamicColumnReferenceNode>())
            {
                var column = structure.NewColumn();
                column.Name = dynamicColumn.Name[^1];
                column.IsDynamic = true;
            }
        }

        protected override QsiTableColumn ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column)
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

                        return columns.Length switch
                        {
                            0 => throw new QsiException(QsiError.UnknownColumnIn, name.Value, scopeFieldList),
                            > 1 => throw new QsiException(QsiError.AmbiguousColumnIn, column.Name, scopeFieldList),
                            _ => columns[0]
                        };
                    }

                    foreach (var refTable in table.References)
                    {
                        queue.Enqueue(refTable);
                    }
                }
            }

            return base.ResolveColumnReference(context, column);
        }
    }
}
