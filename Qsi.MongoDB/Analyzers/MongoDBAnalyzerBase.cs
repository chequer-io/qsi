using System;
using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Context;
using Qsi.MongoDB.Internal.Nodes;

namespace Qsi.MongoDB.Analyzers
{
    public abstract partial class MongoDBAnalyzerBase : QsiAnalyzerBase
    {
        protected MongoDBRepositoryProviderBase _repositoryProvider;

        protected Dictionary<string, Action<JsObjectInfo, INode[], IAnalyzerContext>> DatabaseMemberHandlers;

        protected MongoDBAnalyzerBase(QsiEngine engine) : base(engine)
        {
            InitializeDatabaseHandlers();
        }

        protected void AppendParameterExpression(Range range, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            jsObjectInfo.AppendParameter(context.Script.Script[range]);
        }
    }
}
