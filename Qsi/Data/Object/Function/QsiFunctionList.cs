using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data.Object.Function;

public class QsiFunctionList : QsiObject
{
    public QsiFunctionObject[] Functions { get; }

    public override QsiObjectType Type => QsiObjectType.Function;

    public QsiFunctionList(IEnumerable<QsiFunctionObject> functions) : this(functions.ToArray())
    {
    }

    public QsiFunctionList(params QsiFunctionObject[] functions) : base(new QsiQualifiedIdentifier())
    {
        Functions = functions;
    }
}
