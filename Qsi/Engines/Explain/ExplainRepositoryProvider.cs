using System;
using System.Data;
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

        public async Task<IDataReader> GetDataReaderAsync(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken)
        {
            IQsiAnalysisResult[] results = await _engine.Explain(script, cancellationToken);

            if (results.Length != 1 || results[0] is not QsiTableResult tableResult)
                throw new QsiException(QsiError.InvalidNestedExplain, script.Script);

            return new ExplainDataReader(tableResult.Table.Columns.Count);
        }

        private sealed class ExplainDataReader : IDataReader
        {
            private const byte StateReady = 0;
            private const byte StateRead = 1;
            private const byte StateClosed = 2;

            public int Depth => 0;

            public bool IsClosed => _state == StateClosed;

            public int RecordsAffected => 0;

            public int FieldCount { get; }

            public object this[int i] => GetValue(i);

            public object this[string name] => throw new NotSupportedException();

            private byte _state;

            public ExplainDataReader(int fieldCount)
            {
                FieldCount = fieldCount;
            }

            public bool NextResult()
            {
                _state = StateClosed;
                return false;
            }

            public bool Read()
            {
                if (_state is StateReady)
                {
                    _state = StateRead;
                    return true;
                }

                _state = StateClosed;
                return false;
            }

            public void Dispose()
            {
                Close();
            }

            public void Close()
            {
                _state = StateClosed;
            }

            public DataTable GetSchemaTable()
            {
                throw new NotSupportedException();
            }

            public bool GetBoolean(int i)
            {
                throw new NotSupportedException();
            }

            public byte GetByte(int i)
            {
                throw new NotSupportedException();
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotSupportedException();
            }

            public char GetChar(int i)
            {
                throw new NotSupportedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotSupportedException();
            }

            public IDataReader GetData(int i)
            {
                throw new NotSupportedException();
            }

            public string GetDataTypeName(int i)
            {
                throw new NotSupportedException();
            }

            public DateTime GetDateTime(int i)
            {
                throw new NotSupportedException();
            }

            public decimal GetDecimal(int i)
            {
                throw new NotSupportedException();
            }

            public double GetDouble(int i)
            {
                throw new NotSupportedException();
            }

            public Type GetFieldType(int i)
            {
                throw new NotSupportedException();
            }

            public float GetFloat(int i)
            {
                throw new NotSupportedException();
            }

            public Guid GetGuid(int i)
            {
                throw new NotSupportedException();
            }

            public short GetInt16(int i)
            {
                throw new NotSupportedException();
            }

            public int GetInt32(int i)
            {
                throw new NotSupportedException();
            }

            public long GetInt64(int i)
            {
                throw new NotSupportedException();
            }

            public string GetName(int i)
            {
                throw new NotSupportedException();
            }

            public int GetOrdinal(string name)
            {
                throw new NotSupportedException();
            }

            public string GetString(int i)
            {
                throw new NotSupportedException();
            }

            public object GetValue(int i)
            {
                if (_state is not StateRead)
                    throw new InvalidOperationException();

                if (i < 0 || i >= FieldCount)
                    throw new IndexOutOfRangeException();

                return null;
            }

            public int GetValues(object[] values)
            {
                if (_state is not StateRead)
                    throw new InvalidOperationException();

                var read = Math.Min(FieldCount, values.Length);
                values.AsSpan(0, read).Fill(null);

                return read;
            }

            public bool IsDBNull(int i)
            {
                if (_state is not StateRead)
                    throw new InvalidOperationException();

                return true;
            }
        }
    }
}
