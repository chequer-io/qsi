using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Services;
using Qsi.Utilities;

namespace Qsi.Tests.Drivers;

public abstract class RepositoryProviderDriverBase : QsiRepositoryProviderBase
{
    private static readonly Dictionary<Type, QsiDataType> _dataTypeMapping =
        new()
        {
            [typeof(string)] = QsiDataType.String,
            [typeof(byte)] = QsiDataType.Numeric,
            [typeof(sbyte)] = QsiDataType.Numeric,
            [typeof(short)] = QsiDataType.Numeric,
            [typeof(ushort)] = QsiDataType.Numeric,
            [typeof(int)] = QsiDataType.Numeric,
            [typeof(uint)] = QsiDataType.Numeric,
            [typeof(long)] = QsiDataType.Numeric,
            [typeof(ulong)] = QsiDataType.Numeric,
            [typeof(float)] = QsiDataType.Decimal,
            [typeof(double)] = QsiDataType.Decimal,
            [typeof(decimal)] = QsiDataType.Decimal,
            [typeof(bool)] = QsiDataType.Boolean,
            [typeof(TimeSpan)] = QsiDataType.Time,
            [typeof(DateTime)] = QsiDataType.DateTime,
            [typeof(DateTimeOffset)] = QsiDataType.DateTimeOffset,
            [typeof(byte[])] = QsiDataType.Binary,
            [typeof(object)] = QsiDataType.Object
        };

    protected DbConnection Connection { get; }

    public List<QsiScript> ScriptHistories { get; } = new();

    protected RepositoryProviderDriverBase(DbConnection connection)
    {
        Connection = connection;
    }

    protected override async Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken)
    {
        var structure = new QsiTableStructure();
        using var reader = await GetDataReaderAsync(script, parameters, cancellationToken);

        for (int i = 0; i < reader.FieldCount; i++)
        {
            var column = structure.NewColumn();
            column.Name = new QsiIdentifier(reader.GetName(i), false);
        }

        var table = new QsiDataTable(structure);

        while (reader.Read())
        {
            var fieldCount = reader.FieldCount;
            var row = new QsiDataRow(fieldCount);

            for (var i = 0; i < fieldCount; i++)
            {
                var value = reader.GetValue(i);
                QsiDataType valueType;

                if (value is null or DBNull)
                {
                    valueType = QsiDataType.Null;
                    value = null;
                }
                else
                {
                    valueType = GetValueType(value.GetType());
                }

                row.Items[i] = new QsiDataValue(value, valueType);
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private QsiDataType GetValueType(Type type)
    {
        if (_dataTypeMapping.TryGetValue(type, out var valueType))
            return valueType;

        return QsiDataType.Unknown;
    }

    protected override Task<IDataReader> GetDataReaderAsync(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken)
    {
        ScriptHistories.Add(script);
        return GetDataReaderCoreAsync(script, parameters, cancellationToken);
    }

    protected async Task<IDataReader> GetDataReaderCoreAsync(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken)
    {
        var command = Connection.CreateCommand();
        command.CommandText = script.Script;

        if (!ListUtility.IsNullOrEmpty(parameters))
            throw new NotImplementedException();

        var dataReader = await command.ExecuteReaderAsync(cancellationToken);

        return new DataReaderDriver(dataReader, command);
    }

    private sealed class DataReaderDriver : IDataReader
    {
        private readonly IDataReader _reader;
        private readonly DbCommand _command;

        public DataReaderDriver(IDataReader reader, DbCommand command)
        {
            _reader = reader;
            _command = command;
        }

        public bool GetBoolean(int i)
        {
            return _reader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _reader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _reader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return _reader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return _reader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _reader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return _reader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return _reader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return _reader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return _reader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return _reader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _reader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _reader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _reader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return _reader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return _reader.GetString(i);
        }

        public object GetValue(int i)
        {
            return _reader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _reader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return _reader.IsDBNull(i);
        }

        public int FieldCount => _reader.FieldCount;

        public object this[int i] => _reader[i];

        public object this[string name] => _reader[name];

        public void Dispose()
        {
            _reader.Dispose();
            _command.Dispose();
        }

        public void Close()
        {
            _reader.Close();
        }

        public DataTable GetSchemaTable()
        {
            return _reader.GetSchemaTable();
        }

        public bool NextResult()
        {
            return _reader.NextResult();
        }

        public bool Read()
        {
            return _reader.Read();
        }

        public int Depth => _reader.Depth;

        public bool IsClosed => _reader.IsClosed;

        public int RecordsAffected => _reader.RecordsAffected;
    }
}
