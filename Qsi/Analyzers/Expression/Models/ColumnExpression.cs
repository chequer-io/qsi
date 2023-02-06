using Qsi.Data;

namespace Qsi.Analyzers.Expression.Models;

public class ColumnExpression : QsiExpression
{
    public QsiTableColumn Column { get; }

    public ColumnExpression(QsiTableColumn column)
    {
        Column = column;
    }
}
