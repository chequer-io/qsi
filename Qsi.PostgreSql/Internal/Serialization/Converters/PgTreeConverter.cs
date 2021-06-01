using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Qsi.PostgreSql.Internal.PG10.Types;

namespace Qsi.PostgreSql.Internal.Serialization.Converters
{
    internal sealed class PgTreeConverter : JsonConverter
    {
        private Type _skipNextType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.IsSZArray)
                return ReadNodeArray(reader, objectType, serializer);

            return ReadNode(reader, serializer);
        }

        private object ReadNodeArray(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var list = new List<object>();

            if (reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    list.Add(ReadNode(reader, serializer));
                }
            }
            else
            {
                list.Add(ReadNode(reader, serializer));
            }

            var elementType = objectType.GetElementType();
            var result = Array.CreateInstance(elementType!, list.Count);

            for (int i = 0; i < result.Length; i++)
                result.SetValue(list[i], i);

            return result;
        }

        private object ReadNode(JsonReader reader, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Wrapper : Start Object
            bool wrapped = reader.TokenType == JsonToken.StartObject;

            if (wrapped)
                reader.Read();

            if (reader.TokenType != JsonToken.PropertyName)
                throw new SerializationException($"'{JsonToken.PropertyName}' token expected.");

            var nodeName = (string)reader.Value;

            if (string.IsNullOrWhiteSpace(nodeName))
                throw new SerializationException("PostgreSql");

            if (!PgNodeContract.TryGetNodeType(nodeName, out var nodeType))
                throw new SerializationException($"Not supported node '{nodeName}'");

            // Value : Start Object
            VerifyNextExpected(reader, JsonToken.StartObject);

            if (!nodeType.IsInterface)
                _skipNextType = nodeType;

            var node = serializer.Deserialize(reader, nodeType);
            _skipNextType = null;

            // Wrapper : End Object
            if (wrapped)
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

            var result =
                typeof(IPgNode).IsAssignableFrom(objectType) ||
                objectType.IsSZArray && typeof(IPgNode).IsAssignableFrom(objectType.GetElementType());

            if (objectType == typeof(IPg10Node[][]))
            {
            }

            return result;
        }
    }
}
