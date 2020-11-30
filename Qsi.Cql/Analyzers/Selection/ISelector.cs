using Newtonsoft.Json.Linq;

namespace Qsi.Cql.Analyzers.Selection
{
    internal interface ISelector
    {
        JToken Run(JToken value);
    }
}
