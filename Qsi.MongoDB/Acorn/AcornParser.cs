﻿using System;
using System.Collections.Generic;
using System.Linq;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Qsi.MongoDB.Internal;
using Qsi.MongoDB.Internal.Nodes;
using Qsi.MongoDB.Internal.Serialization;
using Qsi.MongoDB.Resources;
using Qsi.Parsing;

namespace Qsi.MongoDB.Acorn;

internal static class AcornParser
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

    public static INode GetAstNode(string code)
    {
        try
        {
            string result = ParseStrict(code);
            Console.WriteLine(result);
            return JsonConvert.DeserializeObject<ProgramNode>(result, _serializerSettings);
        }
        catch (JsCompilationException e)
        {
            throw new QsiSyntaxErrorException(e.LineNumber, e.ColumnNumber, e.Description);
        }
    }

    public static IEnumerable<MongoDBStatement> Parse(string code)
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

    public static string Execute(string code)
    {
        return _javascriptContext.Evaluate(code);
    }

    internal static string ParseStrict(string code)
    {
        _javascriptContext.SetVariable("code", code);
        return _javascriptContext.Evaluate($"JSON.stringify(acorn.parse(code, {{locations: true}}))");
    }

    internal static string ParseLoose(string code)
    {
        _javascriptContext.SetVariable("code", code);
        return _javascriptContext.Evaluate($"JSON.stringify(acorn.loose.LooseParser.parse(code, {{locations: true}}))");
    }
}