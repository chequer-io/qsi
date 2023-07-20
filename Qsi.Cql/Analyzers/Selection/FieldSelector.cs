using Newtonsoft.Json.Linq;

namespace Qsi.Cql.Analyzers.Selection;

internal sealed class FieldSelector : ISelector
{
    public string Field { get; }

    public FieldSelector(string field)
    {
        Field = field;
    }

    public JToken Run(JToken value)
    {
        if (value is JObject jObject)
            return jObject.Property(Field);

        throw new QsiException(QsiError.NotSupportedFeature, $"MemberAccess by field to {value.Type}");
    }
}