namespace Qsi.Data;

public class QsiLiteralExpression : QsiAtomicExpression
{
    public object Value { get; set; }

    public QsiDataType DataType { get; set; }

    public QsiLiteralExpression() : base(QsiExpressionType.Literal)
    {
    }

    public QsiLiteralExpression(int startIndex, int endIndex) : base(QsiExpressionType.Literal, startIndex, endIndex)
    {
    }
}
