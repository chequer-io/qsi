using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Analyzers.Context;
using Qsi.MongoDB.Internal.Nodes;

namespace Qsi.MongoDB.Analyzers
{
    public abstract partial class MongoDBAnalyzerBase
    {
        private void InitializeDatabaseHandlers()
        {
            DatabaseMemberHandlers ??= new Dictionary<string, Action<JsObjectInfo, INode[], IAnalyzerContext>>
            {
                { "getCollection", GetCollectionHandler },
                { "getSiblingDB", GetSiblingDBHandler },
                { "adminCommand", (o, a, c) => HandleFunction(o, a, c, "Object") },
            };
        }

        #region Database Handlers
        private void GetCollectionHandler(JsObjectInfo jsObjectInfo, INode[] arguments, IAnalyzerContext context)
        {
            if (arguments.Length == 0)
                throw new InvalidOperationException("Missing required argument");

            var argument = arguments[0] as LiteralNode;

            if (argument?.Value is string name)
            {
                jsObjectInfo.CollectionName = name;
                AppendParameterExpression(argument.Range, jsObjectInfo, context);
            }
            else
            {
                throw new InvalidOperationException("Collection name must be a string.");
            }
        }

        private void GetSiblingDBHandler(JsObjectInfo jsObjectInfo, INode[] arguments, IAnalyzerContext context)
        {
            if (arguments.Length == 0)
                throw new InvalidOperationException("Missing required argument");

            var argument = arguments[0] as LiteralNode;

            if (argument?.Value is string name)
            {
                jsObjectInfo.DatabaseName = name;
                AppendParameterExpression(argument.Range, jsObjectInfo, context);
            }
            else
            {
                throw new InvalidOperationException("Database name must be a string.");
            }
        }

        public IEnumerable<string> GetParameters(IEnumerable<INode> arguments, IAnalyzerContext context)
        {
            return arguments.Cast<BaseNode>().Select(a => context.Script.Script[a.Range]);
        }

        private void HandleFunction(JsObjectInfo jsObjectInfo, IEnumerable<INode> arguments, IAnalyzerContext context, string returnType, bool clearCollectionInfo = true)
        {
            jsObjectInfo.ReturnType = returnType;
            jsObjectInfo.AppendParameters(GetParameters(arguments, context));

            if (clearCollectionInfo)
            {
                jsObjectInfo.DatabaseName = null;
                jsObjectInfo.CollectionName = null;
            }
        }
        #endregion
    }
}
