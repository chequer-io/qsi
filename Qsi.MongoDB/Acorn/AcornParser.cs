using System;
using System.Collections.Generic;
using System.Linq;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Qsi.Data;
using Qsi.MongoDB.Internal;
using Qsi.MongoDB.Internal.Nodes;
using Qsi.MongoDB.Internal.Serialization;
using Qsi.MongoDB.Resources;
using Qsi.Parsing;

namespace Qsi.MongoDB.Acorn
{
    public static class AcornParser
    {
        private static readonly JavascriptContext _javascriptContext;
        private static readonly JsonSerializerSettings _serializerSettings;

        private static readonly string[] _selectFunctions = { "findOne", "findOneAndUpdate", "findOneAndReplace", "findOneAndDelete", "findAndModify", "count", "countDocuments" };
        private static readonly string[] _deleteFunctions = { "remove", "deleteOne", "deleteMany" };
        private static readonly string[] _insertFunctions = { "insert", "insertOne", "insertMany", "bulkWrite" };
        private static readonly string[] _updateFunctions = { "update", "updateOne", "updateMany", "replaceOne", "updateRole", "updateUser" };
        private static readonly string[] _grantFunctions = { "grantRolesToUser", "grantRolesToRole", "grantPrivilegesToRole" };
        private static readonly string[] _revokeFunctions = { "revokeRolesFromUser", "revokeRolesFromRole", "revokePrivilegesFromRole" };
        private static readonly string[] _dropInCollectionFunctions = { "drop", "dropIndex", "dropIndexes" };

        private static readonly string[] _dropInDatabaseFunctions =
        {
            "dropAllUsersFromDatabase", "dropUser",
            "dropRole", "dropAllUsers", "dropAllRolesFromDatabase", "dropAllRoles",
            "drop", "dropDatabase", "dropIndexes"
        };

        private static readonly string[] _createInCollectionFunctions = { "createIndex", "createIndexes" };
        private static readonly string[] _createInDatabaseFunctions = { "createUser", "createRole", "createCollection", "createView" };
        private static readonly string[] _selectWithCursorFunctions = { "find", "aggregate" };

        private static readonly string[] _cursorFunctions =
        {
            "skip", "limit", "map", "allowDiskUse",
            "addOption", "allowPartialResults", "batchSize",
            "collation", "comment", "hint", "maxAwaitTimeMS",
            "min", "noCursorTimeout", "oplogReplay", "readPref",
            "returnKey", "tailable", "showRecordId", "readConcern"
        };

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
                    ScriptType = GetScriptType(n),
                    Range = n.Range,
                    Start = n.Loc.Start,
                    End = n.Loc.End
                });
        }

        private static QsiScriptType GetScriptType(INode node)
        {
            var context = new ParseContext();

            GetScriptTypeInternal(node, context);

            return context.Success ? context.ScriptType : QsiScriptType.Unknown;
        }

        private static void GetScriptTypeInternal(INode node, ParseContext context)
        {
            if (context.IsDone)
                context.Success = false;

            if (!context.Success)
                return;

            switch (node)
            {
                case CallExpressionNode callExpression:
                    if (callExpression.Callee is not MemberExpressionNode mExpression)
                    {
                        context.Success = false;
                        return;
                    }

                    GetScriptTypeInternal(mExpression.Object, context);

                    if (mExpression.Property is IdentifierNode identNode)
                    {
                        var funcName = identNode.Name;

                        if (context.IsCursor)
                        {
                            if (!_cursorFunctions.Contains(funcName))
                            {
                                context.Success = false;
                            }
                        }
                        else if (context.IsDatabase)
                        {
                            if (!context.IsCollection && !VisitDatabaseFunctions(funcName, context) ||
                                context.IsCollection && !VisitCollectionFunctions(funcName, context))
                            {
                                context.Success = false;
                            }
                        }
                    }
                    else
                    {
                        GetScriptTypeInternal(mExpression.Property, context);
                    }

                    break;

                case ExpressionStatementNode expressionStatement:
                    GetScriptTypeInternal(expressionStatement.Expression, context);
                    break;

                case MemberExpressionNode memberExpression:
                    GetScriptTypeInternal(memberExpression.Object, context);
                    GetScriptTypeInternal(memberExpression.Property, context);

                    break;

                case IdentifierNode identifier:
                    if (context.IsDatabase && context.IsCollection)
                        return;

                    if (identifier.Name == "db" && !context.IsDatabase)
                    {
                        context.ScriptType = QsiScriptType.Show;
                        context.IsDatabase = true;
                    }
                    else if (context.IsDatabase && !context.IsCollection)
                    {
                        context.IsCollection = true;
                    }

                    break;
            }
        }

        private static bool VisitDatabaseFunctions(string name, ParseContext context)
        {
            // db.getCollection("coll").~
            if (name == "getCollection")
            {
                context.IsCollection = true;
            }
            // db.getSiblingDB("db").~
            else if (name == "getSiblingDB")
            {
            }
            else if (_grantFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Grant;
                context.IsDone = true;
            }
            else if (_revokeFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Revoke;
                context.IsDone = true;
            }
            else if (_createInDatabaseFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Create;
                context.IsDone = true;
            }
            else if (_dropInDatabaseFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Drop;
                context.IsDone = true;
            }
            else
            {
                return false;
            }

            return true;
        }

        private static bool VisitCollectionFunctions(string name, ParseContext context)
        {
            if (_selectFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Select;
                context.IsDone = true;
            }
            else if (_deleteFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Delete;
                context.IsDone = true;
            }
            else if (_insertFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Insert;
                context.IsDone = true;
            }
            else if (_updateFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Update;
                context.IsDone = true;
            }
            else if (_selectWithCursorFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Select;
                context.IsCursor = true;
            }
            else if (_createInCollectionFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Create;
                context.IsDone = true;
            }
            else if (_dropInCollectionFunctions.Contains(name))
            {
                context.ScriptType = QsiScriptType.Drop;
                context.IsDone = true;
            }
            else
            {
                switch (name)
                {
                    case "explain":
                        context.ScriptType = QsiScriptType.Explain;
                        context.IsDone = true;
                        break;

                    case "renameCollection":
                        context.ScriptType = QsiScriptType.Rename;
                        context.IsDone = true;
                        break;

                    default:
                        return false;
                }
            }

            return true;
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
            code = NormalizeCode(code);

            return _javascriptContext.Evaluate($"JSON.stringify(acorn.parse('{code}', {{locations: true}}))");
        }

        internal static string ParseLoose(string code)
        {
            code = NormalizeCode(code);

            return _javascriptContext.Evaluate($"JSON.stringify(acorn.loose.LooseParser.parse('{code}', {{locations: true}}))");
        }

        private static string NormalizeCode(string code)
        {
            code = code?.Replace("'", "\\'") ?? "";
            code = code.Replace("\n", "\\n");
            code = code.Replace("\r", "\\r");

            return code;
        }

        private class ParseContext
        {
            public bool IsDatabase { get; set; }

            public bool IsCollection { get; set; }

            public bool IsCursor { get; set; }

            public bool IsDone { get; set; }

            public QsiScriptType ScriptType { get; set; }

            public bool Success { get; set; } = true;
        }
    }
}
