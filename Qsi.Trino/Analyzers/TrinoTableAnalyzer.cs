using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;
using Qsi.Trino.Tree;

namespace Qsi.Trino.Analyzers;

public sealed class TrinoTableAnalyzer : QsiTableAnalyzer
{
    public TrinoTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    public override ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
    {
        if (table is TrinoValuesTableNode valuesTableNode)
            return BuildTrinoValuesTableStructure(context, valuesTableNode);

        return base.BuildTableStructure(context, table);
    }

    private ValueTask<QsiTableStructure> BuildTrinoValuesTableStructure(TableCompileContext context, TrinoValuesTableNode table)
    {
        if (table.Rows.Count == 0)
            throw new QsiException(QsiError.Syntax);

        var structure = new QsiTableStructure
        {
            Type = QsiTableType.Inline
        };

        var columnCount = table.Rows[0].ColumnValues.Count;
        int colIndex = 0;

        foreach (var value in table.Rows[0].ColumnValues)
        {
            var column = structure.NewColumn();

            if (value is IQsiColumnExpressionNode { Column: IQsiDerivedColumnNode { Alias: { } } derivedColumnNode })
            {
                column.Name = derivedColumnNode.Alias.Name;
            }
            else
            {
                column.Name = new QsiIdentifier($"_col{colIndex}", false);
                column.IsExpression = true;
            }

            colIndex++;
        }

        foreach (var row in table.Rows)
        {
            if (columnCount != row.ColumnValues.Count)
                throw new QsiException(QsiError.DifferentColumnsCount);

            for (int i = 0; i < columnCount; i++)
            {
                var value = row.ColumnValues[i];

                IEnumerable<QsiTableColumn> references =
                    value is IQsiColumnExpressionNode columnExpressionNode ?
                        ResolveColumns(context, columnExpressionNode.Column, out _) :
                        ResolveColumnsInExpression(context, value);

                structure.Columns[i].References.AddRange(references);
            }
        }

        return new ValueTask<QsiTableStructure>(structure);
    }

    protected override IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        switch (expression)
        {
            case TrinoExistsExpressionNode:
                break;

            case TrinoInvokeExpressionNode e:
            {
                foreach (var column in base.ResolveColumnsInExpression(context, expression))
                    yield return column;

                foreach (var column in ResolveColumnsInExpression(context, e.OrderBy.Value))
                    yield return column;

                foreach (var column in ResolveColumnsInExpression(context, e.Filter.Value))
                    yield return column;

                foreach (var column in ResolveColumnsInExpression(context, e.Over.Value))
                    yield return column;

                break;
            }

            case TrinoWindowExpressionNode e:
            {
                foreach (var item in e.Items)
                {
                    foreach (var column in item.Children
                                 .OfType<IQsiExpressionNode>()
                                 .SelectMany(x => ResolveColumnsInExpression(context, x)))
                    {
                        yield return column;
                    }
                }

                break;
            }

            case TrinoIntervalExpressionNode:
            case TrinoLambdaExpressionNode:
            case TrinoMeasureExpressionNode:
            case TrinoSubscriptExpressionNode:
            case TrinoTypeConstructorExpressionNode:
            {
                foreach (var column in expression.Children
                             .OfType<IQsiExpressionNode>()
                             .SelectMany(x => ResolveColumnsInExpression(context, x)))
                {
                    yield return column;
                }

                break;
            }

            default:
                foreach (var c in base.ResolveColumnsInExpression(context, expression))
                    yield return c;

                break;
        }
    }
}