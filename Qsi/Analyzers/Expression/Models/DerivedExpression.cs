using System.Collections.Generic;

namespace Qsi.Analyzers.Expression.Models;

public class DerivedExpression : QsiExpression
{
    public QsiExpression Expression { get; }

    public DerivedExpression(QsiExpression expression)
    {
        Expression = expression;
    }

    public override IEnumerable<QsiExpression> GetChildren()
    {
        yield return Expression;
    }
}
