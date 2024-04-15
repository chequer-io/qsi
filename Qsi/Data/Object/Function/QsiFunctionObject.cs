namespace Qsi.Data.Object.Function;

public class QsiFunctionObject : QsiObject
{
    public override QsiObjectType Type => QsiObjectType.Function;

    public string Definition { get; }

    public int ArgumentsCount { get; }

    public int DefaultArgumentsCount { get; }

    public QsiFunctionObject(QsiQualifiedIdentifier identifier, string definition, int argumentsCount) : base(identifier)
    {
        Definition = definition;
        ArgumentsCount = argumentsCount;
    }

    public QsiFunctionObject(QsiQualifiedIdentifier identifier, string definition, int argumentsCount, int defaultArgumentsCount)
        : this(identifier, definition, argumentsCount)
    {
        DefaultArgumentsCount = defaultArgumentsCount;
    }
}
