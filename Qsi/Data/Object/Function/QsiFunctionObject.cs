namespace Qsi.Data.Object.Function;

public class QsiFunctionObject : QsiObject
{
    public override QsiObjectType Type => QsiObjectType.Function;

    public string Definition { get; }

    public QsiFunctionObject(QsiQualifiedIdentifier identifier, string definition) : base(identifier)
    {
        Definition = definition;
    }
}
