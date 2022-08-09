using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Qsi.Analyzers;
using Qsi.Analyzers.Table;
using Qsi.Data;
using Qsi.Services;
using Qsi.Tests.Models;
using Qsi.Tests.Utilities;
using VerifyNUnit;

namespace Qsi.Tests.Vendor.MySql;

[TestFixture("server=localhost;port=3306;user id=root;password=root;pooling=False;allowuservariables=True", Category = "MySql")]
public partial class MySqlTest : VendorTestBase
{
    public MySqlTest(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection OpenConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }

    protected override void PrepareConnection(DbConnection connection)
    {
        new MySqlScript(
            (MySqlConnection)connection,
            ResourceUtility.GetResourceContent("mysql-sakila-schema.sql")
        ).Execute();

        new MySqlScript(
            (MySqlConnection)connection,
            ResourceUtility.GetResourceContent("mysql-sakila-data.sql")
        ).Execute();

        connection.ChangeDatabase("qsi_unit_tests");
    }

    protected override IQsiLanguageService CreateLanguageService(DbConnection connection)
    {
        return new Driver.MySqlLanguageService(connection);
    }

    [TestCase("SELECT * FROM actor")]
    [TestCase("SELECT * FROM city")]
    [TestCase("SELECT * FROM actor a JOIN city c")]
    [TestCase("SELECT * FROM actor UNION SELECT * FROM city")]
    [TestCase("SELECT * FROM (SELECT actor_id FROM actor) a")]
    [TestCase("SELECT * FROM (SELECT actor_id FROM actor, city) ac")]
    [TestCase("SELECT (SELECT actor_id) FROM actor")]
    [TestCase("SELECT (SELECT actor_id FROM city LIMIT 1) FROM actor")]
    [TestCase("SELECT (SELECT a.actor_id FROM actor LIMIT 1) from actor a")]
    [TestCase("SELECT (SELECT actor.actor_id FROM city AS actor LIMIT 1) FROM actor")]
    [TestCase("SELECT (SELECT actor_id, city_id) FROM actor, city")]
    [TestCase("WITH RECURSIVE CTE AS (SELECT 1 N UNION ALL SELECT N + 1 FROM CTE WHERE N < 10) SELECT * FROM CTE")]
    [TestCase("SELECT (SELECT actor_id from city) from actor")]
    [TestCase("SELECT (SELECT actor_id from actor) from actor_info")]
    public async Task Test_SELECT(string sql)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);

        QsiTableStructureView[] views = QsiTableStructureView.From(((QsiTableResult)result[0]).Table);

        await Verifier.Verify(views).UseDirectory("verified");
    }

    [TestCase("TABLE actor")]
    [TestCase("TABLE city")]
    public async Task Test_TABLE(string sql)
    {
        if (Connection.ServerVersion![0] == '5')
            Assert.Pass();

        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);

        QsiTableStructureView[] views = QsiTableStructureView.From(((QsiTableResult)result[0]).Table);

        await Verifier.Verify(views).UseDirectory("verified");
    }

    [TestCase("SELECT 1, .2, 0.3, 0.4E+5", ExpectedResult = new[] { "1", ".2", "0.3", "0.4E+5" })]
    [TestCase("SELECT 1 + 2, 3 +/*cmt*/ 4", ExpectedResult = new[] { "1 + 2", "3 +/*cmt*/ 4" })]
    [TestCase("SELECT 'Test'", ExpectedResult = new[] { "Test" })]
    [TestCase("SELECT _utf8mb4 'Test'", ExpectedResult = new[] { "Test" })]
    [TestCase("SELECT _utf8mb4 'Test' collate utf8mb4_unicode_ci", ExpectedResult = new[] { "_utf8mb4 'Test' collate utf8mb4_unicode_ci" })]
    [TestCase("SELECT N'National'", ExpectedResult = new[] { "National" })]
    [TestCase("SELECT X'0F', 0x0F", ExpectedResult = new[] { "X'0F'", "0x0F" })]
    [TestCase("SELECT B'0101', 0b0101", ExpectedResult = new[] { "B'0101'", "0b0101" })]
    [TestCase("SELECT NOW(), NOW(/*hi*/)", ExpectedResult = new[] { "NOW()", "NOW(/*hi*/)" })]
    [TestCase("SELECT * FROM actor", ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" })]
    [TestCase("SELECT * FROM actor a", ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" })]
    [TestCase("SELECT a.* FROM actor a", ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" })]
    [TestCase("SELECT a.actor_id AS `hey~` FROM actor a", ExpectedResult = new[] { "`hey~`" })]
    [TestCase("SELECT * FROM actor JOIN film_actor USING (actor_id) LIMIT 0", ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update", "film_id", "last_update" })]
    [TestCase("SELECT a.* FROM actor a JOIN film_actor f USING (actor_id) LIMIT 0", ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" })]
    [TestCase("SELECT f.* FROM actor a JOIN film_actor f USING (actor_id) LIMIT 0", ExpectedResult = new[] { "actor_id", "film_id", "last_update" })]
    [TestCase("SELECT * FROM actor JOIN film_actor USING (actor_id, last_update) LIMIT 0", ExpectedResult = new[] { "actor_id", "last_update", "first_name", "last_name", "film_id" })]
    [TestCase("(SELECT 1) UNION (SELECT 2)", ExpectedResult = new[] { "1" })]
    public async Task<string[]> Test_SELECT_ColumnNames(string sql)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(1, result.Length);
        Assert.IsInstanceOf<QsiTableResult>(result[0]);

        return ((QsiTableResult)result[0]).Table.Columns
            .Select(x => x.Name.ToString())
            .ToArray();
    }

    [TestCase("INSERT INTO actor VALUES (1, 2, 3, 4), (5, 6, 7, 8)", new string[0], 1)]
    [TestCase("INSERT INTO actor (actor_id) VALUES (1)", new string[0], 1)]
    [TestCase("INSERT INTO actor (actor_id, last_name) SELECT city_id, city FROM city", new[] { "SELECT city_id, city FROM city" }, 1)]
    [TestCase("INSERT INTO actor SELECT * FROM actor LIMIT 0", new[] { "SELECT * FROM actor LIMIT 0" }, 1)]
    [TestCase("INSERT INTO actor SELECT * FROM actor LIMIT 1", new[] { "SELECT * FROM actor LIMIT 1" }, 1)]
    [TestCase("INSERT INTO actor SELECT * FROM actor LIMIT 2", new[] { "SELECT * FROM actor LIMIT 2" }, 1)]
    [TestCase("INSERT INTO actor SET actor_id = 1", new string[0], 1)]
    [TestCase("INSERT INTO actor SET actor.actor_id = 1", new string[0], 1)]
    [TestCase("INSERT INTO actor VALUES (1, 2, 3, 4) ON DUPLICATE KEY UPDATE last_update = now()", new string[0], 1)]
    public async Task Test_INSERT(string sql, string[] expectedSqls, int expectedResultCount)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.AreEqual(expectedSqls, ScriptHistories.Select(x => x.Script));
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(result.Length, expectedResultCount);
    }

    [TestCase("DELETE FROM actor", new[] { "SELECT * FROM actor" }, 1)]
    [TestCase("DELETE actor FROM actor", new[] { "SELECT actor.* FROM actor" }, 1)]
    [TestCase("DELETE actor.* FROM actor WHERE 1=2", new[] { "SELECT actor.* FROM actor WHERE 1=2" }, 1)]
    [TestCase("DELETE actor_id FROM actor", new[] { "SELECT actor_id FROM actor" }, 1)]
    [TestCase("DELETE actor.actor_id FROM actor", new[] { "SELECT actor.actor_id FROM actor" }, 1)]
    [TestCase("DELETE FROM actor AS a", new[] { "SELECT * FROM actor" }, 1)]
    [TestCase("DELETE a FROM actor AS a", new[] { "SELECT a.* FROM actor AS a" }, 1)]
    [TestCase("DELETE a.* FROM actor AS a", new[] { "SELECT a.* FROM actor AS a" }, 1)]
    [TestCase("DELETE actor_id FROM actor AS a", new[] { "SELECT actor_id FROM actor AS a" }, 1)]
    [TestCase("DELETE a.actor_id FROM actor AS a", new[] { "SELECT a.actor_id FROM actor AS a" }, 1)]
    [TestCase("DELETE a, c FROM actor a JOIN city c ON false", new[] { "SELECT a.*, c.* FROM actor a JOIN city c ON false" }, 2)]
    [TestCase("DELETE a, c, film FROM actor a JOIN city c JOIN film WHERE false", new[] { "SELECT a.*, c.*, film.* FROM actor a JOIN city c JOIN film WHERE false" }, 3)]
    [TestCase("DELETE c FROM address a JOIN city c USING (city_id) WHERE false", new[] { "SELECT c.* FROM address a JOIN city c USING (city_id) WHERE false" }, 1)]
    public async Task Test_DELETE(string sql, string[] expectedSqls, int expectedResultCount)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.AreEqual(expectedSqls, ScriptHistories.Select(x => x.Script));
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(result.Length, expectedResultCount);
    }

    [TestCase("UPDATE actor SET actor_id = 1", new[] { "SELECT * FROM actor" }, 1)]
    [TestCase("UPDATE actor SET actor.actor_id = 1", new[] { "SELECT * FROM actor" }, 1)]
    [TestCase("UPDATE actor SET actor_id = (SELECT city_id FROM city LIMIT 1)", new[] { "SELECT * FROM actor", "(SELECT city_id FROM city LIMIT 1)" }, 1)]
    [TestCase("UPDATE actor SET actor_id = 1 WHERE false", new[] { "SELECT * FROM actor WHERE false" }, 1)]
    [TestCase("UPDATE actor AS a SET a.actor_id = 1 WHERE false", new[] { "SELECT * FROM actor AS a WHERE false" }, 1)]
    [TestCase("UPDATE actor, city SET city_id = 2, actor_id = 1 WHERE false", new[] { "SELECT * FROM actor, city WHERE false" }, 2)]
    [TestCase("UPDATE actor, city SET city.city_id = 2, actor.actor_id = 1 WHERE false", new[] { "SELECT * FROM actor, city WHERE false" }, 2)]
    [TestCase("UPDATE actor a JOIN city c ON false JOIN film f ON false SET a.last_update = null, c.last_update = null, f.last_update = null", new[] { "SELECT * FROM actor a JOIN city c ON false JOIN film f ON false" }, 3)]
    [TestCase("UPDATE address a JOIN city c USING (city_id) SET c.city = 1, a.address_id = 2 WHERE false", new[] { "SELECT * FROM address a JOIN city c USING (city_id) WHERE false" }, 2)]
    [TestCase("UPDATE address a JOIN city c USING (city_id) SET c.last_update = 1, a.last_update = 2 WHERE false", new[] { "SELECT * FROM address a JOIN city c USING (city_id) WHERE false" }, 2)]
    public async Task Test_UPDATE(string sql, string[] expectedSqls, int expectedResultCount)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        CollectionAssert.AreEqual(expectedSqls, ScriptHistories.Select(x => x.Script));
        CollectionAssert.IsNotEmpty(result);
        Assert.AreEqual(result.Length, expectedResultCount);
    }

    [TestCase("SELECT * FROM x.y", ExpectedResult = "QSI-0006: Unable to resolve table 'x.y'")]
    [TestCase("SELECT a.* FROM actor", ExpectedResult = "QSI-0008: Unknown table 'a'")]
    [TestCase("INSERT INTO actor VALUES (1, 2, 3, 4), (5, 6, 7)", ExpectedResult = "QSI-001B: Column count doesn't match value count at row 2")]
    [TestCase("INSERT INTO actor (aaaaaaaa) VALUES (1)", ExpectedResult = "QSI-000C: Unknown column 'aaaaaaaa'")]
    [TestCase("UPDATE actor SET actor_id = 1, actor_id = 2", ExpectedResult = "QSI-0001: 'Multiple set column' is not supported feature")]
    [TestCase("UPDATE actor SET bbb = 2", ExpectedResult = "QSI-000C: Unknown column 'bbb'")]
    [TestCase("UPDATE actor_info SET film_info = 2", ExpectedResult = "QSI-001A: Column 'film_info' is not updatable")]
    [TestCase("UPDATE address a JOIN city c USING (city_id) SET c.city_id = 1, a.address_id = 2 WHERE false", ExpectedResult = "QSI-001A: Column 'c.city_id' is not updatable")]
    public string Test_Throws(string sql)
    {
        return Assert.ThrowsAsync<QsiException>(async () => await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null))?.Message;
    }

    [TestCaseSource(nameof(Print_TestDatas))]
    public async Task Test_Print(string sql)
    {
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);

        foreach (var scriptHistory in ScriptHistories)
            Console.WriteLine(scriptHistory.Script);

        var print = DebugUtility.Print(result.OfType<QsiDataManipulationResult>());
        Console.WriteLine(print);

        await Verifier.Verify(print).UseDirectory("verified");
    }

    [TestCaseSource(nameof(Print_BindParam_TestDatas))]
    public async Task Test_Print_BindParam(string sql, object[] parameters)
    {
        QsiParameter[] qsiParameters = parameters
            .Select(x => new QsiParameter(QsiParameterType.Index, null, x))
            .ToArray();

        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), qsiParameters);

        foreach (var scriptHistory in ScriptHistories)
            Console.WriteLine(scriptHistory.Script);

        var print = DebugUtility.Print(result.OfType<QsiDataManipulationResult>());
        Console.WriteLine(print);

        await Verifier.Verify(print).UseDirectory("verified");
    }

    [TestCaseSource(nameof(Test_InferredName_TestDatas))]
    public async Task Test_InferredName(string sql, string[] expects)
    {
        // TODO: Use real connection's expects
        IQsiAnalysisResult[] result = await Engine.Execute(new QsiScript(sql, QsiScriptType.Select), null);
        var table = result.OfType<QsiTableResult>().Single().Table;
        IEnumerable<string> columnNames = table.Columns.Select(x => x.Name.Value);

        IEnumerable<string> scripts = ScriptHistories.Select(x => x.Script);

        Assert.AreEqual(expects, columnNames);
    }
}
