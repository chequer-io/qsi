using System.Collections.Generic;
using System.Linq;

namespace Qsi.Data.Object.Function;

public class QsiFunctionObject : QsiObject
{
    private readonly QsiFunctionParameterCollection _inParameters;
    private readonly QsiFunctionParameterCollection _outParameters;

    public IList<QsiFunctionParameter> InParameters => _inParameters;

    public IList<QsiFunctionParameter> OutParameters => _outParameters;

    public override QsiObjectType Type => QsiObjectType.Function;

    public bool HasDefinition => !string.IsNullOrEmpty(Definition);

    public string Definition { get; set; }

    public int InParametersCount => InParameters.Count;

    public int InDefaultParametersCount => InParameters.Count(p => p.IsDefault);

    public int OutParametersCount => OutParameters.Count;

    public QsiFunctionObject()
    {
        _inParameters = new QsiFunctionParameterCollection(this);
        _outParameters = new QsiFunctionParameterCollection(this);
    }

    public QsiFunctionParameter NewInParameter()
    {
        var column = new QsiFunctionParameter();
        _inParameters.Add(column);

        return column;
    }

    public QsiFunctionParameter NewOutParameter()
    {
        var column = new QsiFunctionParameter();
        _outParameters.Add(column);

        return column;
    }
}
