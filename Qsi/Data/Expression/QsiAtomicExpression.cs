namespace Qsi.Data;

public class QsiAtomicExpression : IQsiExpression
{
    public QsiExpressionType ExpressionType { get; }

    public int StartIndex { get; set; }

    public int EndIndex { get; set; }

    public QsiAtomicExpression(QsiExpressionType type)
    {
        ExpressionType = type;
    }

    public QsiAtomicExpression(QsiExpressionType type, int startIndex, int endIndex)
    {
        ExpressionType = type;
        StartIndex = startIndex;
        EndIndex = endIndex;
    }
}
