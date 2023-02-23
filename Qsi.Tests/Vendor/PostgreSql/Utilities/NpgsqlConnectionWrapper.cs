using System.Data;
using System.Data.Common;
using Npgsql; 

namespace Qsi.Tests.Vendor.PostgreSql.Utilities;

internal class NpgsqlConnectionWrapper : DbConnection
{
    internal readonly NpgsqlConnection _connection;

    public NpgsqlConnectionWrapper(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return _connection.BeginTransaction(isolationLevel);
    }

    public override void ChangeDatabase(string databaseName)
    {
        _connection.ChangeDatabase(databaseName);
    }

    public override void Close()
    {
        _connection.Close();
    }

    public override void Open()
    {
        _connection.Open();
    }

    public override string ConnectionString
    {
        get => _connection.ConnectionString;
        set => _connection.ConnectionString = value;
    }

    public override string Database => _connection.Database;

    public override ConnectionState State => _connection.State;

    public override string DataSource => _connection.DataSource;

    public override string ServerVersion => _connection.ServerVersion;

    protected override DbCommand CreateDbCommand()
    {
        var command = _connection.CreateCommand();
        command.AllResultTypesAreUnknown = true;

        return command;
    }
}
