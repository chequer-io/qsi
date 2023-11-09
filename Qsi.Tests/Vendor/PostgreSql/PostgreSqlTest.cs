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

[TestFixture("server=localhost;port=54322;user id=postgres;password=1234;database=dvdrental", Category = "PostgreSql")]
public partial class PostgreSqlTest : VendorTestBase
{
    private const string _baseResourcePath = "PostgreSql.dvdrental";
    private const string _defaultDatabase = "postgres";

    private static readonly HashSet<QsiError> _acceptableQsiErrors = new()
    {
        QsiError.NotSupportedTree, // 지원하지 않는 경우
        QsiError.NotSupportedScript,
        QsiError.NotSupportedFeature,
        QsiError.UnableResolveColumn, // 결정할 수 없는 경우
        QsiError.UnableResolveDefinition,
        QsiError.UnableResolveFunction,
        QsiError.UnableResolveTable,
        QsiError.UnknownColumn, // 알 수 없는 경우
        QsiError.UnknownTable,
        QsiError.UnknownVariable,
        QsiError.UnknownView,
        QsiError.UnknownColumnIn,
        QsiError.UnknownTableIn,
        QsiError.UnknownViewIn
    };

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

        builder.Password ??= "1234";

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

        new NpgsqlCommand("SET search_path = \"public\"", npgsqlConnection).ExecuteNonQuery();
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

    /// <summary>
    /// Qsi Engine에서, 예외가 발생하지 않고 동작하는지 체크합니다.
    /// </summary>
    /// <remarks>이 테스트는 실제 분석 결과를 체크하지 않으며, 오직 예외 발생 여부만 확인합니다.</remarks>
    /// <param name="query">테스트용 쿼리입니다. 이 쿼리는 문법적으로 오류가 없어야 합니다.</param>
    [Timeout(10000)]
    [TestCaseSource(nameof(BasicSelectTestDatas))]
    // [TestCaseSource(nameof(_pgTestCaseDatas))]
    // [TestCaseSource(nameof(GetTestCaseDatas), new object[] { "Resources/PostgreSql/queries.12.json" })]
    // [TestCaseSource(nameof(GetTestCaseDatas), new object[] { "Resources/PostgreSql/queries.13.json" })]
    // [TestCaseSource(nameof(GetTestCaseDatas), new object[] { "Resources/PostgreSql/queries.14.json" })]
    [TestCaseSource(nameof(GetTestCaseDatas), new object[] { "Resources/PostgreSql/queries.15.json" })]
    [TestCaseSource(nameof(_dataGripTestDatas))]
    [TestCaseSource(nameof(PostgresSpecificTestDatas))]
    [TestCaseSource(nameof(LiteralTestDatas))]
    [TestCaseSource(nameof(FunctionTestDatas))]
    [TestCaseSource(nameof(SystemColumnTestDatas))]
    public async Task Test_QsiEngine(string query)
    {
        try
        {
            await Engine.Execute(new QsiScript(query, QsiScriptType.Select), null);
        }
        catch (QsiException e)
            when (_acceptableQsiErrors.Contains(e.Error))
        {
            Assert.Pass($"Exception ({e.Message}) is valid.");
        }
        catch (Exception e)
        {
            Console.WriteLine();
            Console.WriteLine(query);

            Assert.Fail($"{e.GetType()} : {e.Message}");
        }

        Assert.Pass();
    }

    /// <summary>
    /// SELECT 문에 대한 기본적인 테스트를 수행합니다.
    /// </summary>
    /// <param name="query">SELECT 문 쿼리입니다.</param>
    // [TestCaseSource(nameof(BasicSelectTestDatas))]
    public async Task Test_SELECT_Basic(string query)
    {
        IQsiAnalysisResult[] results = await Engine.Execute(new QsiScript(query, QsiScriptType.Select), null);

        CollectionAssert.IsNotEmpty(results);
        Assert.AreEqual(1, results.Length);

        QsiTableStructureView[] views = QsiTableStructureView.From(((QsiTableResult)results[0]).Table);

        await Verifier.Verify(views).UseDirectory("verified");
    }

    /// <summary>
    /// SELECT 문의 결과 중, 컬럼이 올바른지에 대한 테스트를 수행합니다.
    /// </summary>
    /// <param name="query">SELECT 문 쿼리입니다.</param>
    /// <returns>컬럼 이름 목록입니다.</returns>
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
            .Select(name =>
            {
                if (name.Length < 2)
                    return name.ToLower();

                if (name.StartsWith('"') && name.EndsWith('"'))
                    return name;

                return name.ToLower();
            })
            .ToArray();
    }

    /// <summary>
    /// INSERT, UPDATE, DELETE 문 등 DML에 대한 테스트룰 수행합니다.
    /// </summary>
    /// <param name="query">DML 문입니다.</param>
    /// <param name="expectedQueries">DML 문을 분석할 때 추가로 수행이 예상되는 쿼리의 목록입니다.</param>
    /// <param name="expectedResultCount">Qsi 분석 결과 인스턴스의 예상 갯수입니다.</param>
    [TestCaseSource(nameof(InsertTestDatas))]
    [TestCaseSource(nameof(UpdateTestDatas))]
    [TestCaseSource(nameof(DeleteTestDatas))]
    public async Task Test_DML(string query, string[] expectedQueries, int expectedResultCount)
    {
        var script = new QsiScript(query, QsiScriptType.Select);

        try
        {
            IQsiAnalysisResult[] results = await Engine.Execute(script, null);

            CollectionAssert.IsNotEmpty(results);
            CollectionAssert.AreEqual(expectedQueries, ScriptHistories.Select(x => x.Script));

            Assert.AreEqual(results.Length, expectedResultCount);
        }
        catch (QsiException e) when (_acceptableQsiErrors.Contains(e.Error))
        {
            Assert.Pass($"Exception ({e.Message}) is valid.");
        }
        catch (Exception e)
        {
            Console.WriteLine();
            Console.WriteLine(query);

            Assert.Fail($"{e.GetType()} : {e.Message}");
        }

        Assert.Pass();
    }

    /// <summary>
    /// Parameterized query에 대하여 테스트를 수행합니다.
    /// </summary>
    /// <param name="query">Parameterized된 쿼리입니다.</param>
    /// <param name="arguments">쿼리에 들어갈 인자 목록입니다.</param>
    [TestCaseSource(nameof(ParameterTestDatas))]
    public async Task Test_Parameters(string query, object[] arguments)
    {
        QsiParameter[] qsiParameters = arguments
            .Select(p => new QsiParameter(QsiParameterType.Index, null, p))
            .ToArray();

        IQsiAnalysisResult[] results = await Engine.Execute(new QsiScript(query, QsiScriptType.Select), qsiParameters);

        Console.WriteLine(DebugUtility.Print(results.OfType<QsiDataManipulationResult>()));
    }

    /// <summary>
    /// 버전에 따라 지원 여부가 달라지는 함수와 연산자에 대하여, 파싱이 가능한지에 대해 테스트를 수행합니다.
    /// </summary>
    /// <param name="query">테스트할 쿼리입니다.</param>
    /// <param name="from">이 버전부터 쿼리를 지원합니다.</param>
    /// <param name="to">이 버전까지 쿼리를 지원합니다.</param>
    [TestCaseSource(nameof(_versionDependentTestCaseDatas))]
    [TestCaseSource(nameof(_versionDependentSystemTestCaseDatas))]
    public async Task Test_Version_Dependent_Syntax(string query, int? from, int? to)
    {
        var script = new QsiScript(query, QsiScriptType.Select);

        try
        {
            await Engine.Execute(script, null);
        }
        catch (QsiException e)
            when (_acceptableQsiErrors.Contains(e.Error))
        {
            Assert.Pass($"Exception ({e.Message}) is valid.");
        }
        catch (Exception e)
        {
            Console.WriteLine();
            Console.WriteLine($"This query is available {(from is null ? "" : $"{from}")}~{(to is null ? "" : $"{to}")}.");
            Console.WriteLine(query);

            Assert.Fail($"{e.GetType()} : {e.Message}");
        }

        Assert.Pass();
    }

    /// <summary>
    /// 버전에 따라 지원 여부가 달라지는 함수와 연산자에 대하여, 현재 데이터베이스 버전에서 테스트할 수 있는 쿼리에 대해서만 테스트를 수행합니다.
    /// </summary>
    /// <param name="query">테스트할 쿼리입니다.</param>
    /// <param name="from">이 버전부터 쿼리를 지원합니다.</param>
    /// <param name="to">이 버전까지 쿼리를 지원합니다.</param>
    [TestCaseSource(nameof(_versionDependentTestCaseDatas))]
    [TestCaseSource(nameof(_versionDependentSystemTestCaseDatas))]
    public async Task Test_Version_Dependent_Executable_Only(string query, int? from, int? to)
    {
        var wrapper = Connection as NpgsqlConnectionWrapper;
        var npgsqlConnection = wrapper._connection;

        var command = new NpgsqlCommand("show server_version", npgsqlConnection);
        var reader = command.ExecuteReader();

        await reader.ReadAsync();

        var currentVersionString = reader.GetString(0).Split().First();

        await reader.DisposeAsync();
        await command.DisposeAsync();

        var currentVersion = new Version(currentVersionString);

        if ((from is not null && currentVersion.Major < from)
            || to is not null && currentVersion.Major > to)
        {
            Console.WriteLine();
            Console.WriteLine($"This query is available {(from is null ? "" : $"{from}")}~{(to is null ? "" : $"{to}")}.");
            Console.WriteLine($"Your database version is: {currentVersion}");

            Assert.Pass("Current database version does not support this query.");
        }

        var script = new QsiScript(query, QsiScriptType.Select);

        IQsiAnalysisResult[] results = await Engine.Execute(script, null);

        CollectionAssert.IsNotEmpty(results);

        Assert.Pass();
    }
}
