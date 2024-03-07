using System.Collections.Generic;
using System.Threading.Tasks;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.Impala.Tree;
using Qsi.Tree;

namespace Qsi.Impala.Analyzers;

public class ImpalaTableAnalyzer : QsiTableAnalyzer
{
    public ImpalaTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    public override ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
    {
        if (table is ImpalaValuesTableNode valuesTableNode)
            return BuildImpalaValuesTableStructure(context, valuesTableNode);

        return base.BuildTableStructure(context, table);
    }

    private ValueTask<QsiTableStructure> BuildImpalaValuesTableStructure(TableCompileContext context, ImpalaValuesTableNode table)
    {
        if (table.Rows.Count == 0)
            throw new QsiException(QsiError.Syntax);

        var structure = new QsiTableStructure
        {
            Type = QsiTableType.Inline
        };

        var columnCount = table.Rows[0].ColumnValues.Count;

        foreach (var value in table.Rows[0].ColumnValues)
        {
            var column = structure.NewColumn();

            if (value is IQsiColumnExpressionNode { Column: IQsiDerivedColumnNode { Alias: { } } derivedColumnNode })
                column.Name = derivedColumnNode.Alias.Name;
            else
                column.IsExpression = true;
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
}