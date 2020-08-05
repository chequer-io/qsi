using Newtonsoft.Json;

namespace Qsi.PostgreSql.Internal
{
    internal sealed class PgParseResult
    {
        [JsonProperty("parse_tree")]
        public IPgTree[] Tree { get; set; }

        [JsonProperty("stderr_buffer")]
        public string StandardError { get; set; }

        [JsonProperty("error")]
        public PgParseError Error { get; set; }
    }
}
