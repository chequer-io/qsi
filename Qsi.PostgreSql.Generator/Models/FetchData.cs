using Newtonsoft.Json;

namespace Qsi.PostgreSql.Generator.Models
{
    internal sealed class FetchData
    {
        [JsonProperty("find")]
        public string Find { get; set; }

        [JsonProperty("replace")]
        public string Replace { get; set; }
    }
}
