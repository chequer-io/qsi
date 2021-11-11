using System;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Services;

namespace Qsi.Engines.Explain
{
    internal sealed class ExplainRepositoryProvider : IQsiRepositoryProvider
    {
        private readonly QsiEngine _engine;
        private readonly IQsiRepositoryProvider _repositoryProvider;

        public ExplainRepositoryProvider(QsiEngine engine, IQsiRepositoryProvider repositoryProvider)
        {
            _engine = engine;
            _repositoryProvider = repositoryProvider ?? throw new ArgumentNullException(nameof(repositoryProvider));
        }

        public QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            return _repositoryProvider.ResolveQualifiedIdentifier(identifier);
        }

        public QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            return _repositoryProvider.LookupTable(identifier);
        }

        public QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            return _repositoryProvider.LookupDefinition(identifier, type);
        }

        public QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            return _repositoryProvider.LookupVariable(identifier);
        }

        public QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            return _repositoryProvider.LookupObject(identifier, type);
        }

        public async Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken)
        {
            IQsiAnalysisResult[] results = await _engine.Explain(script, cancellationToken);

            if (results.Length != 1 || results[0] is not QsiTableResult tableResult)
                throw new QsiException(QsiError.InvalidNestedExplain, script.Script);

            var dataTable = new QsiDataTable(tableResult.Table, _engine.CacheProviderFactory);
            var dataRow = new QsiDataRow(dataTable.Rows.ColumnCount);

            for (int i = 0; i < dataRow.Length; i++)
                dataRow.Items[i] = QsiDataValue.Explain;

            dataTable.Rows.Add(dataRow);

            return dataTable;
        }
    }
}
