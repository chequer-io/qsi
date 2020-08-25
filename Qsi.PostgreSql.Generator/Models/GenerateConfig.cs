using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Qsi.PostgreSql.Generator.Models
{
    internal sealed class GenerateConfig
    {
        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("outputDirectory")]
        public string OutputDirectory { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("fetch")]
        public FetchFile[] Fetches { get; set; }

        [JsonProperty("compile")]
        public CompileConfig CompileConfig { get; set; }

        [JsonProperty("typeMap")]
        public Dictionary<string, string> TypeMap { get; set; }

        [JsonProperty("nodeTypes")]
        public HashSet<string> NodeTypes { get; set; }
    }
}
