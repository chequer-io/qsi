using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;

namespace Qsi.PostgreSql.Analyzers
{
    public class PgTableAnalyzer : QsiTableAnalyzer
    {
        public PgTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
        {
            switch (expression)
            {
                case IQsiColumnExpressionNode e:

                    switch (e.Column)
                    {
                        case IQsiColumnReferenceNode column:
                            IEnumerable<QsiTableStructure> sources = Enumerable.Empty<QsiTableStructure>();

                            if (context.SourceTable is not null)
                                sources = new[] { context.SourceTable };

                            QsiTableStructure result = null;

                            if (column.Name.Level == 1 && sources.FirstOrDefault(s => TryResolveTableNameFromColumn(s, column, out result)) is not null)
                            {
                                foreach (var tableColumn in ResolveAllColumns(
                                    context,
                                    new QsiAllColumnNode
                                    {
                                        Path = result.Identifier
                                    },
                                    false
                                ))
                                {
                                    yield return tableColumn;
                                }
                            }
                            else
                            {
                                foreach (var qsiTableColumn in base.ResolveColumnsInExpression(context, expression))
                                    yield return qsiTableColumn;
                            }

                            break;
                    }

                    break;

                default:
                    foreach (var qsiTableColumn in base.ResolveColumnsInExpression(context, expression))
                        yield return qsiTableColumn;

                    break;
            }

            static bool TryResolveTableNameFromColumn(QsiTableStructure table, IQsiColumnReferenceNode column, out QsiTableStructure result)
            {
                if (table.Identifier is not null && table.Identifier[^1].Value == column.Name[0].Value)
                {
                    result = table;
                    return true;
                }

                foreach (var tableStructure in table.References)
                {
                    if (TryResolveTableNameFromColumn(tableStructure, column, out result))
                        return true;
                }

                result = default;
                return false;
            }
        }

        protected override QsiTableColumn ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column)
        {
            try
            {
                return base.ResolveColumnReference(context, column);
            }
            catch (QsiException e)
            {
                if (e.Error is QsiError.UnknownColumnIn)
                {
                    if (context.SourceTable.Columns.Count == 0)
                        return new QsiTableColumn
                        {
                            Name = column.Name[^1],
                            IsVisible = true,
                            Parent = context.SourceTable
                        };

                    var table = context.SourceTables.FirstOrDefault(s => s.Columns.Count == 0);

                    if (table is not null)
                        return new QsiTableColumn
                        {
                            Name = column.Name[^1],
                            IsVisible = true,
                            Parent = table
                        };
                }

                throw;
            }
        }
    }
}
