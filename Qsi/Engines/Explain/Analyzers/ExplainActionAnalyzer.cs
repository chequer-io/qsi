using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Data;
using Qsi.Shared.Extensions;
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
            QsiAnalyzerOptions options,
            CancellationToken cancellationToken = default)
        {
            IQsiAnalysisResult[] results = await _analyzer.Execute(script, parameters, tree, options, cancellationToken);

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
                ProcessDataRows(i, result.InsertRows, QsiDataValueOperation.Insert, v => v != QsiDataValue.Explain);
                ProcessDataRows(i, result.DuplicateRows, QsiDataValueOperation.Duplicate, v => v != QsiDataValue.Explain);
                ProcessDataRows(i, result.UpdateAfterRows, QsiDataValueOperation.Update, v => v != QsiDataValue.Explain);
                ProcessDataRows(i, result.DeleteRows, QsiDataValueOperation.Delete, v => v == QsiDataValue.Explain);
            }

            void ProcessDataRows(
                int index,
                QsiDataRowCollection collection,
                QsiDataValueOperation operation,
                Predicate<QsiDataValue> predicate)
            {
                if (collection == null)
                    return;

                var ordinal = ordinalMap[index];

                foreach (var row in collection)
                {
                    if (predicate(row.Items[ordinal]))
                        operations[ordinal] |= operation;
                }
            }

            return new QsiExplainDataManipulationResult(
                result.Table,
                result.AffectedColumns,
                operations
            );
        }
    }
}
