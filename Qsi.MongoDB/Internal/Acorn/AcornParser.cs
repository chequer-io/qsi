using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Qsi.MongoDB.Internal.Nodes;
using Qsi.MongoDB.Internal.Serialization;
using Qsi.MongoDB.Resources;

namespace Qsi.MongoDB.Internal.Acorn
{
    public static class AcornParser
    {
        private static readonly JavascriptContext _javascriptContext;
        private static readonly JsonSerializerSettings _serializerSettings;

        static AcornParser()
        {
            _javascriptContext = new JavascriptContext();
            Initialize();

            _serializerSettings = new JsonSerializerSettings
            {
                Converters =
                {
                    new JsNodeConverter()
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        private static void Initialize()
        {
            _javascriptContext.InitializeEngine();
            _javascriptContext.Execute(ResourceManager.GetResourceContent("acorn.min.js"));
            _javascriptContext.Execute(ResourceManager.GetResourceContent("acorn-loose.min.js"));
        }

        public static string Execute(string code)
        {
            return _javascriptContext.Evaluate(code);
        }

        private static string NormalizeCode(string code)
        {
            code = code?.Replace("'", "\\'") ?? "";
            code = code.Replace("\n", "\\n");
            code = code.Replace("\r", "\\r");

            return code;
        }

        internal static string ParseStrict(string code)
        {
            code = NormalizeCode(code);

            return _javascriptContext.Evaluate($"JSON.stringify(acorn.parse('{code}'))");
        }

        internal static string ParseLoose(string code)
        {
            code = NormalizeCode(code);

            return _javascriptContext.Evaluate($"JSON.stringify(acorn.loose.LooseParser.parse('{code}'))");
        }

        private static Range ConvertRange(JObject statement)
        {
            return new Range(
                statement["start"]?.ToObject<int>() ?? -1,
                statement["end"]?.ToObject<int>() ?? -1
            );
        }

        public static INode GetAstNode(string code)
        {
            string result = ParseStrict(code);
            return JsonConvert.DeserializeObject<ProgramNode>(result, _serializerSettings);
        }

        public static IEnumerable<Range> Parse(string code)
        {
            string result;

            try
            {
                result = ParseStrict(code);
            }
            catch (Exception)
            {
                result = ParseLoose(code);
            }

            return JObject.Parse(result)["body"]?
                .Values<JObject>()
                .Select(ConvertRange);
        }
    }
}
