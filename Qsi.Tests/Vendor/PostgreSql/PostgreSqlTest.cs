using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tests.Models;
using Qsi.Tests.PostgreSql.Driver;
using Qsi.Tests.Utilities;
using Qsi.Tests.Vendor.PostgreSql.Utilities;
using VerifyNUnit;

namespace Qsi.Tests.PostgreSql;

[TestFixture("server=localhost;port=5432;user id=postgres;password=querypie1!;database=dvdrental", Category = "PostgreSql")]
public partial class PostgreSqlTest : VendorTestBase
{
    private const string _baseResourcePath = "PostgreSql.dvdrental";
    private const string _defaultDatabase = "postgres";

    public PostgreSqlTest(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection OpenConnection(string connectionString)
    {
        // Setup database first.
        SetupDatabase(connectionString);

        var connection = new NpgsqlConnection(connectionString);
        var wrapper = new NpgsqlConnectionWrapper(connection);

        return wrapper;
    }

    private static void SetupDatabase(string baseConnectionString)
    {
        using var connection = CreateTemporaryConnection(baseConnectionString);
        connection.Open();

        new NpgsqlCommand("DROP DATABASE IF EXISTS dvdrental", connection).ExecuteNonQuery();
        new NpgsqlCommand("CREATE DATABASE dvdrental", connection).ExecuteNonQuery();
    }

    private static NpgsqlConnection CreateTemporaryConnection(string baseConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Database = _defaultDatabase
        };

        builder.Password ??= "querypie1!";

        var connectionString = builder.ToString();

        return new NpgsqlConnection(connectionString);
    }

    protected override void PrepareConnection(DbConnection connection)
    {
        // Schema
        var wrapper = connection as NpgsqlConnectionWrapper;
        var npgsqlConnection = wrapper._connection;

        PrepareQuery(npgsqlConnection, $"{_baseResourcePath}.restore.sql");

        // Data
        PrepareData(npgsqlConnection);

        // After copy
        PrepareQuery(npgsqlConnection, $"{_baseResourcePath}.after-copy.sql");
    }

    private static void PrepareQuery(NpgsqlConnection connection, string resourcepath)
    {
        var query = ResourceUtility.GetResourceContent(resourcepath);

        var script = new NpgsqlCommand(query, connection);
        script.ExecuteNonQuery();
    }

    private void PrepareData(NpgsqlConnection connection)
    {
        var query = ResourceUtility.GetResourceContent($"{_baseResourcePath}.copy.sql");

        IEnumerable<string> copyStatements = query.Split('\n')
            .Where(stmt => stmt.Contains("COPY"));

        IEnumerator<string> statementEnumerator = copyStatements.GetEnumerator();

        while (true)
        {
            if (!statementEnumerator.MoveNext())
                break;

            var copyCommand = statementEnumerator.Current;

            if (!statementEnumerator.MoveNext())
                throw new Exception("COPY statement count must be even.");

            var copyTarget = statementEnumerator.Current;

            var target = copyTarget
                .Split("FROM")
                .Last() // " '~/Downloads/dvdrental/3065.dat';"
                .Trim() // "'~/Downloads/dvdrental/3065.dat';"
                .TrimEnd(';') // "'~/Downloads/dvdrental/3065.dat'"
                .Trim('\'') // "~/Downloads/dvdrental/3065.dat"
                .Split('/') // [ ~, Downloads, dvdrental, 3065.dat ]  
                .Last(); // "3065.dat"

            Copy(connection, copyCommand, $"{_baseResourcePath}.{target}");
        }
    }

    private static void Copy(NpgsqlConnection connection, string command, string resourcePath)
    {
        using var writer = connection.BeginTextImport(command);

        var resourceText = ResourceUtility.GetResourceContent(resourcePath);
        writer.Write(resourceText);
    }

    protected override IQsiLanguageService CreateLanguageService(DbConnection connection)
    {
        return new PostgreSqlLanguageService(connection);
    }

    [Timeout(10000)]
    [TestCaseSource(nameof(_pgTestCaseDatas))]
    [TestCaseSource(nameof(_dataGripTestDatas))]
    public async Task Test_QsiEngine_Syntax(string query)
    {
        Console.WriteLine(query);

        try
        {
            await Engine.Execute(new QsiScript(query, QsiScriptType.Select), null);
        }
        catch (QsiException e)
            when (e.Error is not (QsiError.Syntax or QsiError.SyntaxError))
        {
            Assert.Fail($"Syntax error: {e.Message}");
        }

        Assert.Pass();
    }

    [TestCaseSource(nameof(PostgresSpecificTestDatas))]
    [TestCaseSource(nameof(LiteralTestDatas))]
    [TestCaseSource(nameof(FunctionTestDatas))]
    [TestCaseSource(nameof(SystemColumnTestDatas))]
    public async Task Test_QsiEngine(string query)
    {
        Console.WriteLine(query);

        await Engine.Execute(new QsiScript(query, QsiScriptType.Select), null);

        Assert.Pass();
    }

    [TestCaseSource(nameof(BasicSelectTestDatas))]
    public async Task Test_SELECT_Basic(string query)
    {
        IQsiAnalysisResult[] results = await Engine.Execute(new QsiScript(query, QsiScriptType.Select), null);

        CollectionAssert.IsNotEmpty(results);
        Assert.AreEqual(1, results.Length);

        QsiTableStructureView[] views = QsiTableStructureView.From(((QsiTableResult)results[0]).Table);

        await Verifier.Verify(views).UseDirectory("verified");
    }

    [TestCaseSource(nameof(ColumnNameSelectTestDatas))]
    [TestCaseSource(nameof(SystemTableFunctionTestDatas))]
    public async Task<string[]> Test_SELECT_Column_Name(string query)
    {
        IQsiAnalysisResult[] results = await Engine.Execute(new QsiScript(query, QsiScriptType.Select), null);

        CollectionAssert.IsNotEmpty(results);
        Assert.AreEqual(1, results.Length);
        Assert.IsInstanceOf<QsiTableResult>(results[0]);

        return ((QsiTableResult)results[0]).Table.Columns
            .Select(c => c.Name?.ToString())
            .ToArray();
    }

    [TestCaseSource(nameof(InsertTestDatas))]
    [TestCaseSource(nameof(UpdateTestDatas))]
    public async Task Test_DML(string query, string[] expectedQueries, int expectedResultCount)
    {
        var script = new QsiScript(query, QsiScriptType.Select);

        IQsiAnalysisResult[] results = await Engine.Execute(script, null);

        CollectionAssert.IsNotEmpty(results);
        CollectionAssert.AreEqual(expectedQueries, ScriptHistories.Select(x => x.Script));

        Assert.AreEqual(results.Length, expectedResultCount);
    }
}
