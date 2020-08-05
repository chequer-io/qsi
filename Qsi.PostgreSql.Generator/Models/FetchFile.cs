using Newtonsoft.Json;

namespace Qsi.PostgreSql.Generator.Models
{
    internal sealed class FetchFile
    {
        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("fetch")]
        public FetchData[] Fetches { get; set; }
    }
}
