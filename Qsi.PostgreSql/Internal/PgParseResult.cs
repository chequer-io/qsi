using Newtonsoft.Json;

namespace Qsi.PostgreSql.Internal
{
    internal sealed class PgParseResult<T> where T : IPgNode
    {
        [JsonProperty("parse_tree")]
        public T[] Tree { get; set; }

        [JsonProperty("stderr_buffer")]
        public string StandardError { get; set; }

        [JsonProperty("error")]
        public PgParseError Error { get; set; }
    }
}
