using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Qsi.PostgreSql.Generator.Models
{
    internal sealed class GenerateConfig
    {
        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("fetch")]
        public FetchFile[] Fetches { get; set; }

        [JsonProperty("compile")]
        public CompileConfig CompileConfig { get; set; }

        [JsonProperty("typeMap")]
        public ImmutableDictionary<string, string> TypeMap { get; set; }

        // support wildcard
        [JsonProperty("targets")]
        public string[] Targets { get; set; }
    }
}
