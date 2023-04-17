using System;
using System.Collections.Generic;
using System.Linq;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Qsi.MongoDB.Internal.Nodes;
using Qsi.MongoDB.Internal.Serialization;
using Qsi.MongoDB.Resources;
using Qsi.Parsing;

namespace Qsi.MongoDB.Acorn
{
    internal static class AcornParser
    {
        private static readonly JsonSerializerSettings _serializerSettings;

        private static readonly JsValue _parse;
        private static readonly JsValue _parseLoose;

        static AcornParser()
        {
            var engine = new Engine()
                .Execute(ResourceManager.GetResourceContent("acorn.min.js"))
                .Execute(ResourceManager.GetResourceContent("acorn-loose.min.js"))
                .Execute("function acorn_parse(code) { return JSON.stringify(acorn.parse(code, {locations: true})) }")
                .Execute("function acorn_parse_loose(code) { return JSON.stringify(acorn.loose.LooseParser.parse(code, {locations: true})) }");

            _parse = engine.GetValue("acorn_parse");
            _parseLoose = engine.GetValue("acorn_parse_loose");

            _serializerSettings = new JsonSerializerSettings
            {
                Converters =
                {
                    new JsNodeConverter()
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public static INode GetAstNode(string code)
        {
            try
            {
                string result = ParseStrict(code);
                Console.WriteLine(result);
                return JsonConvert.DeserializeObject<ProgramNode>(result, _serializerSettings);
            }
            catch (JavaScriptException e)
            {
                int lineNumber = 0;
                int columnNumber = 0;

                try
                {
                    var loc = e.Error.AsObject().Get("loc").AsObject();

                    lineNumber = (int)loc.Get("line").AsNumber();
                    columnNumber = (int)loc.Get("column").AsNumber();
                }
                catch
                {
                    // ignored
                }

                throw new QsiSyntaxErrorException(lineNumber, columnNumber, e.Message);
            }
        }

        public static IEnumerable<MongoDBStatement> Parse(string code)
        {
            string result;

            try
            {
                result = ParseStrict(code);
            }
            catch
            {
                result = ParseLoose(code);
            }

            return FlattenNode(JsonConvert.DeserializeObject<ProgramNode>(result, _serializerSettings))
                .Cast<BaseNode>()
                .Select(n => new MongoDBStatement
                {
                    Range = n.Range,
                    Start = n.Loc.Start,
                    End = n.Loc.End
                });
        }

        private static IEnumerable<INode> FlattenNode(INode rootNode)
        {
            var queue = new Queue<INode>();

            foreach (var child in rootNode.Children)
            {
                queue.Enqueue(child);

                while (queue.TryDequeue(out var node))
                {
                    if (node is BlockStatementNode scope)
                    {
                        foreach (var childNode in scope.Children)
                        {
                            queue.Enqueue(childNode);
                        }

                        continue;
                    }

                    yield return node;
                }
            }
        }

        internal static string ParseStrict(string code)
        {
            return _parse.Invoke(code).ToString();
        }

        internal static string ParseLoose(string code)
        {
            return _parseLoose.Invoke(code).ToString();
        }
    }
}
