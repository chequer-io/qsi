namespace Qsi.Data;

public class QsiColumnExpression : QsiAtomicExpression
{
    public QsiQualifiedIdentifier Identifier { get; }

    public QsiColumnExpression(QsiQualifiedIdentifier identifier) : base(QsiExpressionType.Column)
    {
        Identifier = identifier;
    }

    public QsiColumnExpression(QsiQualifiedIdentifier identifier, int startIndex, int endIndex) : base(QsiExpressionType.Column, startIndex, endIndex)
    {
        Identifier = identifier;
    }
}
