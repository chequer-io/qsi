using System;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Context;
using Qsi.Data;
using Qsi.MongoDB.Internal.Nodes;
using Qsi.MongoDB.Lookup;
using Qsi.MongoDB.Lookup.Data;
using Qsi.Tree;

namespace Qsi.MongoDB.Analyzers
{
    public class MongoDBExpressionAnalyzer : MongoDBAnalyzerBase
    {
        private readonly MongoDBVariableStack _variableStack;

        public MongoDBExpressionAnalyzer(MongoDBVariableStack variableStack, QsiEngine engine) : base(engine)
        {
            _variableStack = variableStack;
        }

        public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
        {
            if (tree is QsiMongoTreeNode node)
                return node.Node is ExpressionStatementNode;

            return false;
        }

        protected override async ValueTask<IQsiAnalysisResult> OnExecute(IAnalyzerContext context)
        {
            if (!(context.Tree is QsiMongoTreeNode mongoNode))
                throw new InvalidOperationException();

            var node = mongoNode.Node as ExpressionStatementNode
                       ?? throw new InvalidOperationException("Node is not ExpressionStatementNode");

            _repositoryProvider = (MongoDBRepositoryProviderBase)context.Engine.RepositoryProvider;

            var jsObjectInfo = new JsObjectInfo();

            ResolveNode(node.Expression, jsObjectInfo, context);

            return null; // new QsiTableAnalysisResult(table);
        }

        private void ResolveNode(INode node, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            switch (node)
            {
                case IExpressionNode expressionNode:
                    ResolveExpressionNode(expressionNode, jsObjectInfo, context);
                    return;
            }

            ThrowNotSupportedNode(node);
        }

        private void ResolveExpressionNode(IExpressionNode node, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            switch (node)
            {
                case CallExpressionNode callExpressionNode:
                    ResolveCallExpressionNode(callExpressionNode, jsObjectInfo, context);
                    break;

                case IdentifierNode identifierNode:
                    ResolveIdentifierNode(identifierNode, jsObjectInfo, context);
                    break;

                case MemberExpressionNode memberExpressionNode:
                    ResolveMemberExpressionNode(memberExpressionNode, jsObjectInfo, context);
                    break;
            }
        }

        private void ResolveCallExpressionNode(CallExpressionNode callExpressionNode, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            ResolveNode(callExpressionNode.Callee, jsObjectInfo, context);
            HandleFunction(callExpressionNode.Arguments, jsObjectInfo, context);
        }

        private void HandleFunction(INode[] arguments, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            if (jsObjectInfo.IsDatabase && DatabaseMemberHandlers.ContainsKey(jsObjectInfo.LastExpression))
                DatabaseMemberHandlers[jsObjectInfo.LastExpression](jsObjectInfo, arguments, context);
        }

        private void ResolveIdentifierNode(IdentifierNode identifierNode, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            ProcessObject(identifierNode.Name, jsObjectInfo, context);
        }

        private void ResolveMemberExpressionNode(MemberExpressionNode memberExpressionNode, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            ResolveNode(memberExpressionNode.Object, jsObjectInfo, context);
            ResolveExpressionNode(memberExpressionNode.Property, jsObjectInfo, context);
        }

        private void ProcessObject(string @object, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            var expr = GetExpressionText(jsObjectInfo, @object);

            if (ProcessPredefinedMember(@object, jsObjectInfo, context))
            {
                jsObjectInfo.AppendExpression(@object);
            }
            else
            {
                var lookupResult = _repositoryProvider.LookupObject(expr);

                if (lookupResult == null)
                    throw new InvalidOperationException($"'{expr}' is not defined");

                ValidateObject(lookupResult, jsObjectInfo, context);
            }
        }

        // Database, Collection, Cursor의 경우 미리 정의된 함수로 체크
        private bool ProcessPredefinedMember(string @object, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            if (jsObjectInfo.IsDatabase)
            {
                // if object name is not known function
                if (!DatabaseMemberHandlers.ContainsKey(@object))
                {
                    jsObjectInfo.CollectionName = @object;
                }

                return true;
            }

            if (jsObjectInfo.IsCollection)
            {
                return true;
            }

            if (jsObjectInfo.IsView)
            {
                return true;
            }

            return false;
        }

        private void ValidateObject(MongoDBLookupResult mongoDbLookupResult, JsObjectInfo jsObjectInfo, IAnalyzerContext context)
        {
            switch (mongoDbLookupResult.Type)
            {
                case MongoDBLookupType.Database:
                    var databaseData = mongoDbLookupResult.Value as DatabaseLookupData
                                       ?? throw new InvalidOperationException("Data type not matched.");

                    if (jsObjectInfo.IsDatabase)
                    {
                        // TODO: db.getSilbingDB("") cannot proceed.
                        throw new InvalidOperationException("Already object is database.");
                    }

                    jsObjectInfo.DatabaseName = databaseData.DatabaseName;
                    jsObjectInfo.ObjectType = JsObjectType.Database;
                    jsObjectInfo.AppendExpression(mongoDbLookupResult.ObjectName);
                    break;

                case MongoDBLookupType.Collection:
                    var collectionData = mongoDbLookupResult.Value as CollectionLookupData
                                         ?? throw new InvalidOperationException("Data type not matched.");

                    jsObjectInfo.DatabaseName ??= collectionData.DatabaseName;
                    jsObjectInfo.CollectionName = collectionData.CollectionName;
                    jsObjectInfo.ObjectType = JsObjectType.Collection;

                    jsObjectInfo.ClearExpression();
                    jsObjectInfo.AppendExpression(mongoDbLookupResult.ObjectName);
                    break;
            }
        }

        private string GetExpressionText(JsObjectInfo jsObjectInfo, string expr)
        {
            string exprText = jsObjectInfo.Expression;

            return string.IsNullOrEmpty(exprText) ? expr : $"{exprText}.{expr}";
        }

        private void ThrowNotSupportedNode(INode node)
        {
            throw new NotSupportedException($"Not supported {node?.GetType().Name ?? "UnknownNode"}.");
        }

        // private MongoDBLookupResult LookupObject(INode node)
        // {
        //     switch (node)
        //     {
        //         case IdentifierNode identifierNode:
        //         {
        //             RepositoryProvider.LookupObject(identifierNode.Name);
        //             break;
        //         }
        //     }
        //
        //     return null;
        // }
    }
}
