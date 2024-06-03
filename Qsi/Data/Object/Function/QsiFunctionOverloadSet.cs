using System.Collections.Generic;

namespace Qsi.Data.Object.Function;

public class QsiFunctionOverloadSet : QsiObject
{
    public override QsiObjectType Type => QsiObjectType.Function;

    public List<QsiFunctionObject> Functions { get; } = new();

    public QsiFunctionOverloadSet(IEnumerable<QsiFunctionObject> functions)
    {
        Functions.AddRange(functions);
    }

    public QsiFunctionOverloadSet(params QsiFunctionObject[] functions)
    {
        Functions.AddRange(functions);
    }
}
