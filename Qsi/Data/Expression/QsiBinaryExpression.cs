namespace Qsi.Data;

public class QsiBinaryExpression : IQsiExpression
{
    public QsiExpressionType ExpressionType { get; }

    public IQsiExpression Left { get; }

    public IQsiExpression Right { get; }

    public int StartIndex { get; set; }

    public int EndIndex { get; set; }

    public QsiBinaryExpression(QsiExpressionType type, IQsiExpression left, IQsiExpression right)
    {
        ExpressionType = type;
        Left = left;
        Right = right;
    }

    public QsiBinaryExpression(QsiExpressionType type, int startIndex, int endIndex, IQsiExpression left, IQsiExpression right)
        : this(type, left, right)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
    }
}
