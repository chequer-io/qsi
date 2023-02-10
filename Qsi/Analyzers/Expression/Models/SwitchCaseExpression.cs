using System.Diagnostics.CodeAnalysis;

namespace Qsi.Analyzers.Expression.Models;

public class SwitchCaseExpression : QsiExpression
{
    public QsiExpression Condition { get; }

    public QsiExpression Consequent { get; }

    public SwitchCaseExpression([AllowNull] QsiExpression condition, QsiExpression consequent)
    {
        Condition = condition;
        Consequent = consequent;
    }
}
