using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Qsi.Analyzers.Expression.Models;

public class TableExpression : QsiExpression
{
    public QsiExpression[] ColumnExpressions { get; }

    public QsiExpression WhereExpression { get; }

    public TableExpression(
        IEnumerable<QsiExpression> columnExpressions,
        [AllowNull] QsiExpression whereExpression
    )
    {
        ColumnExpressions = columnExpressions.ToArray();
        WhereExpression = whereExpression;
    }

    public override IEnumerable<QsiExpression> GetChildren()
    {
        foreach (var columnExpression in ColumnExpressions)
        {
            yield return columnExpression;
        }

        if (WhereExpression is { })
            yield return WhereExpression;
    }
}
