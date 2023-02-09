using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tests.PostgreSql.Driver;

namespace Qsi.Tests.PostgreSql;

[TestFixture("server=localhost;port=5432;user id=mason;password=;database=postgres", Category = "PostgreSql")]
public partial class PostgreSqlTest : VendorTestBase
{
    [Timeout(10000)]
    // [TestCaseSource(nameof(TestCaseDatas))]
    [TestCaseSource(nameof(_dataGripTestDatas))]
    public async Task Test(string query)
    {
        Console.WriteLine(query);
        
        try
        {
            IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(query, QsiScriptType.Select), null);

            var res = result[0];

            if (res is QsiTableResult tableResult)
            {
                Console.WriteLine(tableResult.Table.Columns.Count);
                Console.WriteLine(string.Join(' ', tableResult.Table.Columns.Select(c => c.Name?.ToString() ?? "ANOM")));
            }
        }
        catch (QsiException e)
            when (e.Error is not (QsiError.Syntax or QsiError.SyntaxError))
        {
            throw;
        }

        // var command = new NpgsqlCommand(query, ((NpgsqlConnectionWrapper)Connection)._connection);
        //
        // await using var reader = await command.ExecuteReaderAsync();
        //
        // Console.WriteLine(string.Join('\t', GetColumns()));
        //
        // while (reader.Read())
        // {
        //     Console.WriteLine(string.Join('\t', reader));
        // }
        //
        // IEnumerable<string> GetColumns()
        // {
        //     for (int i = 0; i < reader.FieldCount; i++)
        //     {
        //         yield return reader.GetName(i);
        //     }
        // }
    }

    public PostgreSqlTest(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection OpenConnection(string connectionString)
    {
        var connection = new NpgsqlConnection(connectionString);
        var wrapper = new NpgsqlConnectionWrapper(connection);

        return wrapper;
    }

    protected override void PrepareConnection(DbConnection connection)
    {
        // TBD
    }

    protected override IQsiLanguageService CreateLanguageService(DbConnection connection)
    {
        return new PostgreSqlLanguageService(connection);
    }

    protected class NpgsqlConnectionWrapper : DbConnection
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
}
