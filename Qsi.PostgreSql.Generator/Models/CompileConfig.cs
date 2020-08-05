using Newtonsoft.Json;

namespace Qsi.PostgreSql.Generator.Models
{
    internal sealed class CompileConfig
    {
        // support wildcard
        [JsonProperty("sources")]
        public string[] Sources { get; set; }

        // support wildcard
        [JsonProperty("sourceExcludes")]
        public string[] SourceExcludes { get; set; }

        [JsonProperty("includeFolders")]
        public string[] IncludeFolders { get; set; }
    }
}
