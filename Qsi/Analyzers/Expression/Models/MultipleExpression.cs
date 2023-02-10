using System.Collections.Generic;
using System.Linq;

namespace Qsi.Analyzers.Expression.Models;

public class MultipleExpression : QsiExpression
{
    public QsiExpression[] Expressions { get; }

    public MultipleExpression(params QsiExpression[] expressions)
    {
        Expressions = expressions;
    }

    public MultipleExpression(IEnumerable<QsiExpression> expressions) : this(expressions.ToArray())
    {
    }

    public override IEnumerable<QsiExpression> GetChildren()
    {
        return Expressions;
    }
}
