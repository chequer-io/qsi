using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Engines.Explain
{
    internal sealed class ExplainActionAnalyzer : IQsiAnalyzer
    {
        public readonly QsiActionAnalyzer _analyzer;

        public ExplainActionAnalyzer(QsiActionAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public bool CanExecute(QsiScript script, IQsiTreeNode tree)
        {
            return _analyzer.CanExecute(script, tree);
        }

        public async ValueTask<IQsiAnalysisResult[]> Execute(
            QsiScript script,
            QsiParameter[] parameters,
            IQsiTreeNode tree,
            QsiAnalyzerOptions analyzerOptions,
            ExecuteOptions executeOptions,
            CancellationToken cancellationToken = default)
        {
            IQsiAnalysisResult[] results = await _analyzer.Execute(script, parameters, tree, analyzerOptions, executeOptions, cancellationToken);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];

                if (result is QsiDataManipulationResult dmResult)
                {
                    results[i] = ConvertToExplain(dmResult);
                }
            }

            return results;
        }

        private QsiExplainDataManipulationResult ConvertToExplain(QsiDataManipulationResult result)
        {
            var ordinalMap = result.AffectedColumns
                .Select(ac => result.Table.Columns.IndexOf(ac))
                .ToArray();

            var operations = new QsiDataValueOperation[result.Table.Columns.Count];

            for (int i = 0; i < ordinalMap.Length; i++)
            {
                ref var operation = ref operations[i];

                if (result.InsertRows?.Count > 0)
                    operation |= QsiDataValueOperation.Insert;

                if (result.DeleteRows?.Count > 0)
                    operation |= QsiDataValueOperation.Delete;

                if (result.UpdateAfterRows?.Count > 0)
                    operation |= QsiDataValueOperation.Update;

                if (result.DeleteRows?.Count > 0)
                    operation |= QsiDataValueOperation.Delete;
            }

            var explainResult = new QsiExplainDataManipulationResult(
                result.Table,
                result.AffectedColumns,
                operations
            );

            explainResult.SensitiveDataCollection.AddRange(result.SensitiveDataCollection);

            return explainResult;
        }
    }
}
