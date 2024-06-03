namespace Qsi.Data.Object;

public abstract class QsiObject
{
    public abstract QsiObjectType Type { get; }

    public QsiQualifiedIdentifier Identifier { get; set; }
}
