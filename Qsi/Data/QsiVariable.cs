namespace Qsi.Data;

public sealed class QsiVariable
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public QsiDataType Type { get; set; }

    public object Value { get; set; }
}