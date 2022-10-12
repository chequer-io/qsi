using System.Threading;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Tree;

namespace Qsi.Analyzers
{
    public interface IQsiAnalyzer
    {
        bool CanExecute(QsiScript script, IQsiTreeNode tree);

        ValueTask<IQsiAnalysisResult[]> Execute(
            QsiScript script,
            QsiParameter[] parameters,
            IQsiTreeNode tree,
            QsiAnalyzerOptions analyzerOptions,
            ExecuteOptions executeOptions,
            CancellationToken cancellationToken = default
        );
    }
}
