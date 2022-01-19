using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tests.Driver;
using Qsi.Tests.Utilities;

namespace Qsi.Tests;

[TestFixture("Host=localhost;Port=5432;Username=postgres;Password=postgres;Pooling=False;Database=postgres", Category = "PostgreSql")]
public partial class PostgreSqlTest : VendorTestBase
{
    public PostgreSqlTest(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection OpenConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }

    protected override void PrepareConnection(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = ResourceUtility.GetResourceContent("pg-setup.sql");
        command.ExecuteNonQuery();
    }

    protected override IQsiLanguageService CreateLanguageService(DbConnection connection)
    {
        return new PostgreSqlLanguageService(connection);
    }

    [TestCase("SELECT * FROM qsi_unit_tests.J1_TBL", ExpectedResult = "postgres.qsi_unit_tests.j1_tbl")]
    [TestCase("SELECT * FROM qsi_unit_tests.J2_TBL", ExpectedResult = "postgres.qsi_unit_tests.j2_tbl")]
    [TestCase("SELECT * FROM qsi_unit_tests.J1_TBL a CROSS JOIN qsi_unit_tests.J2_TBL c", ExpectedResult = "(a { postgres.qsi_unit_tests.j1_tbl }, c { postgres.qsi_unit_tests.j2_tbl })")]
    [TestCase("SELECT * FROM qsi_unit_tests.J1_TBL UNION SELECT *, 3 FROM qsi_unit_tests.J2_TBL", ExpectedResult = "postgres.qsi_unit_tests.j1_tbl + postgres.qsi_unit_tests.j2_tbl")]
    [TestCase("SELECT * FROM (SELECT i FROM qsi_unit_tests.J1_TBL) a", ExpectedResult = "a { postgres.qsi_unit_tests.j1_tbl }")]
    [TestCase("SELECT * FROM (SELECT k FROM qsi_unit_tests.J1_TBL, qsi_unit_tests.J2_TBL) ac", ExpectedResult = "ac { (postgres.qsi_unit_tests.j1_tbl, postgres.qsi_unit_tests.j2_tbl) }")]
    public async Task<string> Test_SELECT(string sql)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);
        return QsiTableStructureHelper.GetPseudoName(((QsiTableResult)result[0]).Table);
    }
}
