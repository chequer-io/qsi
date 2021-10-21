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
                            QsiTableStructure[] tableName = null;

                            if (context.SourceTable is not null)
                                sources = new[] { context.SourceTable };

                            if (column.Name.Level == 1)
                                tableName = sources.Where(s => s.Identifier[^1].Value == column.Name[0].Value).ToArray();

                            if (tableName is { Length: 1 })
                            {
                                foreach (var tableColumn in ResolveAllColumns(
                                    context,
                                    new QsiAllColumnNode
                                    {
                                        Path = tableName[0].Identifier
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
        }
    }
}
