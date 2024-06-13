namespace Qsi.Data.Object.Sequence;

public class QsiSequenceObject : QsiObject
{
    public override QsiObjectType Type => QsiObjectType.Sequence;

    public QsiSequenceObject()
    {
    }

    public QsiSequenceObject(QsiQualifiedIdentifier identifier)
    {
        Identifier = identifier;
    }
}
