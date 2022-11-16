using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PrimarSql.Data;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tests.Models;
using Qsi.Tests.Utilities;
using Qsi.Tests.Vendor.PostgreSQL.Driver;
using VerifyNUnit;

namespace Qsi.Tests.PrimarSql;

[TestFixture("EndPoint=127.0.0.1:8000")]
public partial class PrimarSqlTest : VendorTestBase
{
    public PrimarSqlTest(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection OpenConnection(string connectionString)
    {
        return new PrimarSqlConnection(new PrimarSqlConnectionStringBuilder(connectionString));
    }

    protected override void PrepareConnection(DbConnection connection)
    {
        var sqls = new[]
        {
            "DROP TABLE IF EXISTS tbl_hash",
            "DROP TABLE IF EXISTS tbl_hash_sort",
            "CREATE TABLE tbl_hash (hash INT HASH KEY)",
            "CREATE TABLE tbl_hash_sort (hash INT HASH KEY, sort INT SORT KEY)",
            "INSERT INTO tbl_hash VALUES {'hash':1,'a':'a'},{'hash':2,'b':'b'},{'hash':3,'c':'c'}",
            "INSERT INTO tbl_hash_sort VALUES {'hash':1,'sort':10,'a':'a'},{'hash':2,'sort':20,'b':'b'},{'hash':3,'sort':30,'c':'c'}"
        };

        foreach (var sql in sqls)
        {
            using var command = connection.CreateCommand();

            command.CommandText = sql;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(sql, e);
            }
        }
    }

    protected override IQsiLanguageService CreateLanguageService(DbConnection connection)
    {
        return new PrimarSqlLanguageService(connection);
    }

    //
    // TESTS
    //

    [TestCaseSource(nameof(Select_TestDatas))]
    public async Task Test_SELECT(string sql)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);

        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);

        QsiTableStructureView[] views = QsiTableStructureView.From(((QsiTableResult)result[0]).Table);

        await Verifier.Verify(views).UseDirectory("verified");
    }

    [TestCaseSource(nameof(Execute_TestDatas))]
    public async Task Test_Execute(string sql, string[] expectedSqls, int expectedResultCount)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Unknown), null);
        CollectionAssert.AreEqual(expectedSqls, ScriptHistories.Select(x => x.Script));
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(expectedResultCount, result.Length);
    }

    [TestCaseSource(nameof(Print_TestDatas))]
    public async Task Test_Print(string sql)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Unknown), null);
        var print = DebugUtility.Print(result.OfType<QsiDataManipulationResult>());

        await Verifier.Verify(print).UseDirectory("verified");
    }

    [TestCaseSource(nameof(Throw_TestDatas))]
    public string Test_Throws(string sql)
    {
        return Assert.ThrowsAsync<QsiException>(async () =>
            await Engine.Execute(new QsiScript(sql, QsiScriptType.Unknown), null))?.Message;
    }
}
