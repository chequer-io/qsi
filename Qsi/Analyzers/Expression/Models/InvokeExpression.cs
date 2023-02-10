namespace Qsi.Analyzers.Expression.Models;

public class InvokeExpression : QsiExpression
{
    public MultipleExpression Parameters { get; }

    public InvokeExpression(MultipleExpression parameters)
    {
        Parameters = parameters;
    }
}
