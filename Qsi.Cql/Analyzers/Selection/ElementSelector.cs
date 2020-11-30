using Newtonsoft.Json.Linq;

namespace Qsi.Cql.Analyzers.Selection
{
    internal sealed class ElementSelector : ISelector
    {
        public int Element { get; }

        public ElementSelector(int element)
        {
            Element = element;
        }

        public JToken Run(JToken value)
        {
            return value switch
            {
                JArray array => array[Element],
                JObject jObject => jObject.Property(Element.ToString()),
                _ => throw new QsiException(QsiError.NotSupportedFeature, $"MemberAccess by indexer to {value.Type}")
            };
        }
    }
}
