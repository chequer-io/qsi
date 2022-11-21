using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tests.Models;
using Qsi.Tests.Utilities;
using VerifyNUnit;

namespace Qsi.Tests.Vendor.PostgreSQL;

[TestFixture("server=pg.querypie.io;port=5432;user id=postgres;password=1234;", Category = "PostgreSql")]
// [TestFixture("server=localhost;port=5432;user id=postgres;password=password;", Category = "PostgreSql")]
public partial class PostgreSqlTest : VendorTestBase
{
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
        connection.ChangeDatabase("postgres");

        var pgConn = ((NpgsqlConnectionWrapper)connection)._connection;
        // var pgConn = (NpgsqlConnection)connection;

        new NpgsqlCommand("SET SCHEMA 'public'", pgConn).ExecuteNonQuery();

        var drop = ResourceUtility.GetResourceContent("postgres-sakila-drop-objects.sql");
        var dropCommand = new NpgsqlCommand(drop, pgConn);
        dropCommand.AllResultTypesAreUnknown = true;
        dropCommand.ExecuteNonQuery();

        var delete = ResourceUtility.GetResourceContent("postgres-sakila-delete-data.sql");
        var deleteCommand = new NpgsqlCommand(delete, pgConn);
        deleteCommand.AllResultTypesAreUnknown = true;
        deleteCommand.ExecuteNonQuery();

        var schema = ResourceUtility.GetResourceContent("postgres-sakila-schema.sql");
        var schemaCommand = new NpgsqlCommand(schema, pgConn);
        schemaCommand.AllResultTypesAreUnknown = true;
        schemaCommand.ExecuteNonQuery();

        var data = ResourceUtility.GetResourceContent("postgres-sakila-insert-data.sql");
        var dataCommand = new NpgsqlCommand(data, pgConn);
        dataCommand.AllResultTypesAreUnknown = true;
        dataCommand.ExecuteNonQuery();
    }

    protected override IQsiLanguageService CreateLanguageService(DbConnection connection)
    {
        return new Driver.PostgreSqlLanguageService(connection);
    }

    //
    // TESTS
    //

    [Timeout(10000)]
    [TestCaseSource(nameof(Select_TestDatas))]
    public async Task Test_SELECT(string sql)
    {
        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);

        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);

        var views = QsiTableStructureView.From(((QsiTableResult)result[0]).Table);

        await Verifier
            .Verify(new { Sql = sql, Result = views })
            .UseFileName($"{nameof(Test_SELECT)}_hash(sql)={StringUtility.CalculateHash(sql)}")
            .UseDirectory("verified");
    }

    [TestCaseSource(nameof(Table_TestDatas))]
    public async Task Test_TABLE(string sql)
    {
        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);

        var views = QsiTableStructureView.From(((QsiTableResult)result[0]).Table);

        await Verifier.Verify(views).UseDirectory("verified");
    }

    [TestCaseSource(nameof(Select_ColumnName_TestDatas))]
    public async Task<string[]> Test_SELECT_ColumnNames(string sql)
    {
        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);
        Assert.IsInstanceOf<QsiTableResult>(result[0]);

        // TODO: Remove debugging logs.
        var tableResult = (QsiTableResult)result[0];
        var table = tableResult.Table;
        Print(table);

        if (table.Columns.Count > 0)
        {
            Console.WriteLine("COLUMNS");

            Console.WriteLine(table.Columns
                .Select(c => c.Name == null ? "NONAME" : c.Name.Value)
                .Aggregate((a, b) => $"{a} {b}"));
        }
        else
        {
            Console.WriteLine("NO COLUMNS!");
        }

        // TODO: Remove null exception on x.Name when table does not have columns.
        return ((QsiTableResult)result[0]).Table.Columns
            .Select(x => x.Name.ToString())
            .ToArray();

        void Print(QsiTableStructure structure)
        {
            Console.WriteLine($"---- LOG FOR {structure} ----");

            Console.WriteLine($"TABLE_IDENTIFIER: {structure.Identifier}");
            Console.WriteLine($"TABLE_TYPE: {structure.Type}");

            if (structure.Columns?.Count > 0)
            {
                Console.WriteLine("COLUMNS: ");

                Console.WriteLine(structure.Columns
                    .Select(c =>
                    {
                        var name = c.IsAnonymous ? "ANONYMOUS" : c.Name.ToString();
                        var def = c.Default ?? "";
                        var binding = c.IsBinding;
                        var dynamic = c.IsDynamic;
                        var expr = c.IsExpression;
                        var visible = c.IsVisible;

                        var reference = c.ObjectReferences?.Count > 0 ? c.ObjectReferences
                            .Select(o => o.Identifier.ToString())
                            .Aggregate((a, b) => $"{a} {b}") : "";

                        return $"NAME: {name}, DEFAULT: {def}, B: {binding}, D: {dynamic}, E: {expr}, V: {visible}\n" +
                               $"OBJ_REF: {reference}";
                    })
                    .Aggregate((a, b) => $"{a}\n{b}"));
            }
            else
            {
                Console.WriteLine("NO_COLUMNS");
            }

            Console.WriteLine("---- END LOG ----");

            if (structure.References?.Count > 0)
            {
                Console.WriteLine(">>>> REFERENCES >>>>");

                foreach (var reference in structure.References)
                {
                    Print(reference);
                }
            }
        }
    }

    [TestCaseSource(nameof(Insert_TestDatas))]
    public async Task Test_INSERT(string sql, string[] expectedSqls, int expectedResultCount)
    {
        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.AreEqual(expectedSqls, ScriptHistories.Select(x => x.Script));
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(expectedResultCount, result.Length);
    }

    [TestCaseSource(nameof(Delete_TestDatas))]
    public async Task Test_DELETE(string sql, string[] expectedSqls, int expectedResultCount)
    {
        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.AreEqual(expectedSqls, ScriptHistories.Select(x => x.Script));
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(expectedResultCount, result.Length);
    }

    [TestCaseSource(nameof(Update_TestDatas))]
    public async Task Test_UPDATE(string sql, string[] expectedSqls, int expectedResultCount)
    {
        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.AreEqual(expectedSqls, ScriptHistories.Select(x => x.Script));
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(expectedResultCount, result.Length);
    }

    [TestCaseSource(nameof(Throw_TestDatas))]
    public string Test_Throws(string sql)
    {
        return Assert.ThrowsAsync<QsiException>(async () =>
            await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null))?.Message;
    }

    [TestCaseSource(nameof(Print_TestDatas))]
    public async Task Test_Print(string sql)
    {
        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);

        foreach (var scriptHistory in ScriptHistories)
        {
            Console.WriteLine(scriptHistory.Script);
        }

        var print = DebugUtility.Print(result.OfType<QsiDataManipulationResult>());
        Console.WriteLine(print);

        await Verifier.Verify(print).UseDirectory("verified");
    }

    [TestCaseSource(nameof(Print_BindParam_TestDatas))]
    public async Task Test_Print_BindParam(string sql, object[] parameters)
    {
        var qsiParameters = parameters
            .Select(x => new QsiParameter(QsiParameterType.Index, null, x))
            .ToArray();

        var result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), qsiParameters);

        foreach (var scriptHistory in ScriptHistories)
        {
            Console.WriteLine(scriptHistory.Script);
        }

        var print = DebugUtility.Print(result.OfType<QsiDataManipulationResult>());
        Console.WriteLine(print);

        await Verifier.Verify(print).UseDirectory("verified");
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
