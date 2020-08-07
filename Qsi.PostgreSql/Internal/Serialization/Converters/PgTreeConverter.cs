using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Qsi.PostgreSql.Internal.Serialization.Converters
{
    internal sealed class PgTreeConverter : JsonConverter
    {
        private Type _skipNextType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Wrapper : Start Object
            VerifyNextExpected(reader, JsonToken.PropertyName);

            var nodeName = (string)reader.Value;

            if (string.IsNullOrWhiteSpace(nodeName))
                throw new SerializationException("PostgreSql");

            if (!PgNodeContract.TryGetNodeType(nodeName, out var nodeType))
            {
                if (string.IsNullOrWhiteSpace(nodeName))
                    throw new SerializationException($"Not supported node '{nodeName}'");
            }

            // Value : Start Object
            VerifyNextExpected(reader, JsonToken.StartObject);

            if (!nodeType.IsInterface)
                _skipNextType = nodeType;

            var node = serializer.Deserialize(reader, nodeType);
            _skipNextType = null;

            // Wrapper : End Object
            VerifyNext(reader, JsonToken.EndObject);

            return node;
        }

        private void VerifyNext(JsonReader reader, JsonToken current)
        {
            if (reader.TokenType != current)
                throw new SerializationException($"'{current}' token expected.");

            reader.Read();
        }

        private void VerifyNextExpected(JsonReader reader, JsonToken expected)
        {
            if (!reader.Read() || reader.TokenType != expected)
                throw new SerializationException($"'{expected}' token expected.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            if (_skipNextType == objectType)
            {
                _skipNextType = null;
                return false;
            }

            return typeof(IPgNode).IsAssignableFrom(objectType);
        }
    }
}
