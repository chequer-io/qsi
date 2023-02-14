using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Athena.Common;
using Qsi.Athena.Tree.Nodes;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;
using AthenaValuesTableNode = Qsi.Athena.Tree.AthenaValuesTableNode;

namespace Qsi.Athena.Analyzers;

public sealed class AthenaTableAnalyzer : QsiTableAnalyzer
{
    public AthenaTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    public override ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
    {
        if (table is AthenaValuesTableNode valuesTableNode)
            return BuildAthenaValuesTableStructure(context, valuesTableNode);

        return base.BuildTableStructure(context, table);
    }

    private ValueTask<QsiTableStructure> BuildAthenaValuesTableStructure(TableCompileContext context, AthenaValuesTableNode table)
    {
        if (table.Rows.Count == 0)
            throw new QsiException(QsiError.Syntax);

        var structure = new QsiTableStructure
        {
            Type = QsiTableType.Inline
        };

        var columnCount = table.Rows[0].ColumnValues.Count;
        var colIndex = 0;

        foreach (var value in table.Rows[0].ColumnValues)
        {
            var column = structure.NewColumn();
            column.Expression = ExpressionAnalyzer.Resolve(context, value);

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

            for (var i = 0; i < columnCount; i++)
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
            case AthenaExistsExpressionNode exists:
                foreach (var column in ResolveColumnsInExpression(context, exists.Query.Value))
                    yield return column;

                break;

            case AthenaIntervalExpressionNode interval:
                foreach (var column in ResolveColumnsInExpression(context, interval.Time.Value))
                    yield return column;

                break;

            case AthenaInvokeExpressionNode invoke:
                foreach (var column in base.ResolveColumnsInExpression(context, invoke))
                    yield return column;

                foreach (var column in ResolveColumnsInExpression(context, invoke.Filter.Value))
                    yield return column;

                foreach (var column in ResolveColumnsInExpression(context, invoke.Over.Value))
                    yield return column;

                foreach (var column in ResolveColumnsInExpression(context, invoke.OrderBy.Value))
                    yield return column;

                break;

            case AthenaLambdaExpressionNode lambda:
                foreach (var column in ResolveColumnsInExpression(context, lambda.Expression.Value))
                    yield return column;

                break;

            case AthenaOrderExpressionNode order:
                foreach (var column in base.ResolveColumnsInExpression(context, order.Expression.Value))
                    yield return column;

                break;

            case AthenaSubscriptExpressionNode subscript:
                foreach (var column in ResolveColumnsInExpression(context, subscript.Value.Value))
                    yield return column;

                foreach (var column in ResolveColumnsInExpression(context, subscript.Index.Value))
                    yield return column;

                break;

            case AthenaTypeConstructorExpressionNode typeConstructor:
                foreach (var column in ResolveColumnsInExpression(context, typeConstructor.Expression.Value))
                    yield return column;

                break;

            case AthenaWindowExpressionNode window:
                foreach (var item in window.Items)
                {
                    foreach (var column in ResolveColumnsInExpression(context, item.Order.Value))
                        yield return column;

                    foreach (var column in ResolveColumnsInExpression(context, item.Partition.Value))
                        yield return column;

                    foreach (var column in ResolveColumnsInExpression(context, item.Windowing.Value))
                        yield return column;
                }

                break;

            default:
                foreach (var column in base.ResolveColumnsInExpression(context, expression))
                    yield return column;

                break;
        }
    }
}
