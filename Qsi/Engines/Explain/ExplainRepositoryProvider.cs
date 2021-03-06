﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Data;
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

        public async Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken)
        {
            IQsiAnalysisResult[] results = await _engine.Explain(script, cancellationToken);

            if (results.Length != 1 || results[0] is not QsiTableResult tableResult)
                throw new QsiException(QsiError.InvalidNestedExplain, script.Script);

            var dataTable = new QsiDataTable(tableResult.Table);
            var dataRow = dataTable.Rows.NewRow();

            for (int i = 0; i < dataRow.Length; i++)
                dataRow.Items[i] = QsiDataValue.Explain;

            return dataTable;
        }
    }
}
