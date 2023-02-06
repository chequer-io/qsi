using Qsi.Data;

namespace Qsi.Analyzers.Expression.Models;

public class LiteralExpression : QsiExpression
{
    public object Value { get; }

    public QsiDataType Type { get; }

    public LiteralExpression(object value, QsiDataType type)
    {
        Value = value;
        Type = type;
    }
}
