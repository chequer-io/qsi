using System.Collections.Generic;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Data;
using Qsi.MongoDB.Analyzers;
using Qsi.Parsing;
using Qsi.Services;

namespace Qsi.MongoDB
{
    public abstract class MongoDBLanguageServiceBase : IQsiLanguageService
    {
        private readonly MongoDBVariableStack _variableStack;

        protected MongoDBLanguageServiceBase()
        {
            _variableStack = new MongoDBVariableStack();
        }
        
        public QsiAnalyzerOptions CreateAnalyzerOptions()
        {
            return new QsiAnalyzerOptions();
        }

        public IEnumerable<QsiAnalyzerBase> CreateAnalyzers(QsiEngine engine)
        {
            yield return new QsiActionAnalyzer(engine);
            yield return new MongoDBExpressionAnalyzer(_variableStack, engine);
        }

        public IQsiTreeParser CreateTreeParser()
        {
            return new MongoDBParser();
        }

        public IQsiTreeDeparser CreateTreeDeparser()
        {
            throw new System.NotImplementedException();
        }

        public IQsiScriptParser CreateScriptParser()
        {
            return new MongoDBScriptParser();
        }

        public abstract IQsiRepositoryProvider CreateRepositoryProvider();

        public bool MatchIdentifier(QsiIdentifier x, QsiIdentifier y)
        {
            throw new System.NotImplementedException();
        }
    }
}
