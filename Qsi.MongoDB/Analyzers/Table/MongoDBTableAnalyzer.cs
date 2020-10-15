using System;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.MongoDB.Analyzers.Table
{
    public class MongoDBTableAnalyzer : QsiAnalyzerBase
    {
        public MongoDBTableAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
        {
            return true;
        }

        protected override ValueTask<IQsiAnalysisResult> OnExecute(ExecutionContext context)
        {
            if (!(context.Tree is QsiTreeMongoNode mongoNode))
                throw new InvalidOperationException();

            var node = mongoNode.Node;
            
            // variable declaration
            
        }
    }
}
