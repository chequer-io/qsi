using Newtonsoft.Json;

namespace Qsi.PostgreSql.Internal
{
    internal sealed class PgParseError
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("funcname")]
        public string FunctionName { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("lineno")]
        public string FileLineNumber { get; set; }

        [JsonProperty("cursorpos")]
        public string CursorPosition { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }
    }
}
