using System.Collections.Generic;

namespace Qsi.Analyzers.Expression.Models;

public class BinaryExpression : QsiExpression
{
    public QsiExpression Left { get; }

    public QsiExpression Right { get; }

    public string Operator { get; }

    public BinaryExpression(QsiExpression left, QsiExpression right, string @operator)
    {
        Left = left;
        Right = right;
        Operator = @operator;
    }

    public override IEnumerable<QsiExpression> GetChildren()
    {
        yield return Left;
        yield return Right;
    }
}
