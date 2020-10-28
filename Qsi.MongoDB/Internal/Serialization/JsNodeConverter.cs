using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Qsi.MongoDB.Internal.Nodes;
using Newtonsoft.Json;

namespace Qsi.MongoDB.Internal.Serialization
{
    public class JsNodeConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (objectType.IsSZArray)
                return ReadNodeArray(reader, objectType, serializer);

            return ReadNode(reader, serializer);
        }

        private object ReadNodeArray(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var list = new List<INode>();

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

        private INode ReadNode(JsonReader reader, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.StartObject)
                reader.Read();

            if (reader.TokenType != JsonToken.PropertyName)
                throw new SerializationException($"'{JsonToken.PropertyName}' token expected.");

            var nodeName = (string) reader.Value;

            if (nodeName != "type")
                throw new SerializationException("'Could not find 'type' key.");

            reader.Read();
            var typeName = (string) reader.Value;
            reader.Read();

            if (string.IsNullOrWhiteSpace(nodeName))
                throw new SerializationException("Acorn");

            if (!JsNodeTypes.TryGetNodeType(typeName, out var nodeType))
                throw new SerializationException($"Not supported node '{nodeName}'");

            var node = Activator.CreateInstance(nodeType);
            serializer.Populate(new JsJsonReader(reader), node!);

            return (INode)node;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(INode).IsAssignableFrom(objectType) ||
                   objectType.IsSZArray && typeof(INode).IsAssignableFrom(objectType.GetElementType());
        }
    }
}