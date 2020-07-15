using Newtonsoft.Json;
using Qsi.PostgreSql.Resources;

namespace Qsi.PostgreSql.Internal
{
    internal class PgQuery10 : PgQueryBase<IPgTree>
    {
        protected override string GetParserScript()
        {
            return ResourceManager.GetResourceContent("pg_query_10.js");
        }

        protected override string GetParseScript(string input)
        {
            return $"JSON.stringify(PgQuery.parse({JsonConvert.SerializeObject(input)}))";
        }
    }
}
