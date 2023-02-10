using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Qsi.Analyzers.Expression.Models;

public class SwitchExpression : QsiExpression
{
    public QsiExpression Condition { get; }

    public SwitchCaseExpression[] Cases { get; }

    public SwitchExpression([AllowNull] QsiExpression condition, IEnumerable<SwitchCaseExpression> cases)
    {
        Condition = condition;
        Cases = cases.ToArray();
    }

    public override IEnumerable<QsiExpression> GetChildren()
    {
        if (Condition is not null)
            yield return Condition;

        foreach (var c in Cases)
            yield return c;
    }
}
