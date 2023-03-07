using System;
using NUnit.Framework;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql;

namespace Qsi.Tests.PostgreSql;

public class PgDeparseTest
{
    [TestCase("SELECT 1", TestName = "SELECT - Simple Literal")]
    [TestCase("SELECT 1.1, 'Text', true, x'binText1', b'binText2'", TestName = "SELECT - Literals")]
    [TestCase(@"WITH cteTable(averageVal) as (SELECT avg(Salary) FROM Employee)
                    SELECT EmployeeID,Name, Salary 
                    FROM Employee, temporaryTable 
                    WHERE Employee.Salary > cteTable.averageVal",
        TestName = "SELECT - With (CTE)")]
    [TestCase("SELECT 1, 2, 3", TestName = "SELECT - Simple Multiple Literal")]
    [TestCase("SELECT", TestName = "SELECT - Keyword Only")]
    [TestCase("SELECT COUNT(*) FROM actor", TestName = "SELECT - Count Function")]
    [TestCase("SELECT a.* FROM actor a", TestName = "SELECT - Table specific all column")]
    [TestCase("SELECT ROW()", TestName = "SELECT - Explicit Row Expression (Empty)")]
    [TestCase("SELECT !TRUE", TestName = "SELECT - Unary Expression (A_Expr)")]
    [TestCase("SELECT NOT TRUE", TestName = "SELECT - Unary Expression (BoolExpr)")]
    [TestCase("SELECT * FROM actor LIMIT 1", TestName = "SELECT - Limit / Offset #1")]
    [TestCase("SELECT * FROM actor LIMIT 10 OFFSET 15", TestName = "SELECT - Limit / Offset #2")]
    [TestCase("SELECT * FROM actor FETCH NEXT 15 ROWS ONLY", TestName = "SELECT - Limit / Offset #3")]
    // TODO: With group by
    // [TestCase("SELECT * FROM actor FETCH NEXT 15 ROWS WITH TIES", TestName = "SELECT - Limit / Offset #4")]
    [TestCase("SELECT COALESCE(NULL, 1, 2)", TestName = "SELECT - Function (Coalesce)")]
    [TestCase(@"SELECT 	
	                GROUPING(BRAND) AS GROUPING_BRAND,
	                GROUPING(SEGMENT) AS GROUPING_SEGMENT,
	                BRAND,
	                SEGMENT,
	                SUM(QUANTITY)
                FROM sales s 
                GROUP BY
                GROUPING SETS (
	                (BRAND, SEGMENT),
	                (BRAND),
	                (SEGMENT),
	                ()
                )
                ORDER BY BRAND, SEGMENT", TestName = "SELECT - Function (Grouping)")]
    [TestCase("SELECT * FROM actor ORDER BY actor_id ASC, first_name DESC", TestName = "SELECT - Sort By")]
    [TestCase("SELECT GREATEST(1,2,3), LEAST(1,2,3)", TestName = "SELECT - Function(Min, Max)")]
    [TestCase("SELECT TEST_FUNC(TEST_PARAM1 := 1, TEST_PARAM2 := 2, TEST_PARAM3 := 3)", TestName = "SELECT - Function Named Paramters")]
    [TestCase("SELECT actor.*, actor2.* FROM actor, actor2", TestName = "SELECT - All Column with path")]
    [TestCase("SELECT ROW(1, \"text\")", TestName = "SELECT - Explicit Row Expression")]
    [TestCase("SELECT * FROM a JOIN b ON a.test = b.test", TestName = "SELECT - Join")]
    [TestCase("SELECT * FROM a FULL JOIN b ON a.test = b.test", TestName = "SELECT - Full Join")]
    [TestCase("SELECT * FROM a LEFT JOIN b ON a.test = b.test", TestName = "SELECT - Left Join")]
    [TestCase("SELECT * FROM a RIGHT JOIN b ON a.test = b.test", TestName = "SELECT - Right Join")]
    [TestCase("SELECT * FROM table1, table2, table3", TestName = "SELECT - Join (Comma)")]
    [TestCase("SELECT * FROM table1 INNER JOIN table2 USING (c1, c2) AS a", TestName = "SELECT - Join (Using)")]
    [TestCase("SELECT * FROM (table1 JOIN table2 ON table1.c1 = table2.c1) AS joined_table (joined_col1, joined_col2)", TestName = "SELECT - Join (Alias)")]
    [TestCase("SELECT * FROM table1 AS aliased_table (col1, col2)", TestName = "SELECT - Aliased Relation")]
    [TestCase("SELECT actor_id AS c1, first_name FROM table1", TestName = "SELECT - Aliased Column")]
    [TestCase(@"SELECT (SELECT nspname FROM pg_namespace WHERE oid = relnamespace)::text as column_name 
                FROM pg_class WHERE oid = test::regclass", TestName = "SELECT - Cast")]
    [TestCase("SELECT (SELECT * FROM ACTOR)", TestName = "SELECT - Subquery Expression")]
    [TestCase(@"SELECT 'session_stats' AS chart_name, pg_catalog.row_to_json(t) AS chart_data
                FROM
                    (SELECT
                        (SELECT count(*) FROM pg_catalog.pg_stat_activity) AS ""Total"",
                        (SELECT count(*) FROM pg_catalog.pg_stat_activity WHERE state = 'active')  AS ""Active"",
                        (SELECT count(*) FROM pg_catalog.pg_stat_activity WHERE state = 'idle')  AS ""Idle""
                    ) t
                UNION ALL
                    SELECT 'tps_stats' AS chart_name, pg_catalog.row_to_json(t) AS chart_data FROM
                    (SELECT
                        (SELECT sum(xact_commit) + sum(xact_rollback) FROM pg_catalog.pg_stat_database) AS ""Transactions"",
                        (SELECT sum(xact_commit) FROM pg_catalog.pg_stat_database) AS ""Commits"",
                        (SELECT sum(xact_rollback) FROM pg_catalog.pg_stat_database) AS ""Rollbacks""
                    ) t
                UNION ALL
                    SELECT 'ti_stats' AS chart_name, pg_catalog.row_to_json(t) AS chart_data FROM
                    (SELECT
                        (SELECT sum(tup_inserted) FROM pg_catalog.pg_stat_database) AS ""Inserts"",
                        (SELECT sum(tup_updated) FROM pg_catalog.pg_stat_database) AS ""Updates"",
                        (SELECT sum(tup_deleted) FROM pg_catalog.pg_stat_database) AS ""Deletes""
                    ) t
                UNION ALL
                    SELECT 'to_stats' AS chart_name, pg_catalog.row_to_json(t) AS chart_data FROM
                    (SELECT
                        (SELECT sum(tup_fetched) FROM pg_catalog.pg_stat_database) AS ""Fetched"",
                        (SELECT sum(tup_returned) FROM pg_catalog.pg_stat_database) AS ""Returned""
                    ) t
                UNION ALL
                    SELECT 'bio_stats' AS chart_name, pg_catalog.row_to_json(t) AS chart_data FROM
                    (SELECT
                        (SELECT sum(blks_read) FROM pg_catalog.pg_stat_database) AS ""Reads"",
                        (SELECT sum(blks_hit) FROM pg_catalog.pg_stat_database) AS ""Hits""
                    ) t",
        TestName = "SELECT - Union")]
    [TestCase(@"SELECT a, CASE a WHEN 1 THEN 'one' 
                                 WHEN 2 THEN 'two' ELSE 'other' END FROM test",
        TestName = "SELECT - Case Expression (with Expression)")]
    [TestCase(@"SELECT CASE WHEN score > 90 THEN 'A' 
                            WHEN score > 70 THEN 'B' 
                            WHEN score > 50 THEN 'C' 
                            WHEN score > 30 THEN 'D' 
                            ELSE 'F'
                       END AS code_source FROM mine",
        TestName = "SELECT - Case Expression")]
    [TestCase("SELECT * FROM (VALUES (1, 'one'), (2, 'two'), (3, 'three')) AS \"t-table\" (num, letter)",
        TestName = "SELECT - Subquery")]
    [TestCase("SELECT * FROM users WHERE users.email = 'test@example.com' AND users.deleted_at IS NULL",
        TestName = "SELECT - Bool Expression")]
    [TestCase("SELECT (array[1,2,3,4,5,6])[1:3]", TestName = "SELECT - Indexer 1")]
    [TestCase("SELECT my_array[1];", TestName = "SELECT - Indexer 2")]
    [TestCase(@"SELECT CURRENT_DATE, 
                       CURRENT_TIME, CURRENT_TIME(1),
                       CURRENT_TIMESTAMP, CURRENT_TIMESTAMP(1),
                       LOCALTIME, LOCALTIME(1),
                       LOCALTIMESTAMP, LOCALTIMESTAMP(1),
                       CURRENT_ROLE, CURRENT_USER, SESSION_USER, USER,
                       CURRENT_CATALOG, CURRENT_SCHEMA;", TestName = "SELECT - SQLValue functions")]
    [TestCase("SELECT * FROM actor WHERE a IN ($0, $0, $0)", TestName = "SELECT - Binding Parameter (Question Mark)")]
    [TestCase("SELECT * FROM actor WHERE a IN ($1, $2, $3)", TestName = "SELECT - Binding Parameter")]
    [TestCase("SELECT * FROM actor WHERE enabled IS TRUE", TestName = "SELECT - Bool Test #1")]
    [TestCase("SELECT * FROM actor WHERE enabled IS NOT UNKNOWN", TestName = "SELECT - Bool Test #2")]
    [TestCase("SELECT * FROM CURRENT_DATE", TestName = "SELECT - Table Function #1")]
    [TestCase("SELECT * FROM pg_typeof(1)", TestName = "SELECT - Table Function #2")]
    [TestCase("SELECT * FROM ROWS FROM (pg_typeof(1))", TestName = "SELECT - Table Function #3")]
    [TestCase("SELECT * FROM ROWS FROM (pg_typeof(1)) AS t (c1)", TestName = "SELECT - Table Function #4")]
    [TestCase("SELECT DISTINCT ON (pronargs) oid, pronargs FROM pg_proc WHERE proname ='age'", TestName = "SELECT - Disinict On #1")]
    [TestCase(@"SELECT xmltable.* FROM hoteldata, 
                                       XMLTABLE ('/hotels/hotel/rooms/room' PASSING hotels COLUMNS 
                                           id FOR ORDINALITY, 
                                           hotel_name text PATH '../../name' NOT NULL,
                                           room_id int PATH '@id' NOT NULL,
                                           capacity int,
                                           comment text PATH 'comment' DEFAULT 'A regular room'
                                       );", TestName = "SELECT - Xmltable")]
    // INSERT
    [TestCase("INSERT INTO actor VALUES (1,2), (3,4)", TestName = "INSERT - Simple #1")]
    [TestCase("INSERT INTO actor (c1, c2) VALUES (1,2), (3,4)", TestName = "INSERT - Simple #2")]
    [TestCase("INSERT INTO info.actor (c1, c2) VALUES (1,2), (3,4)", TestName = "INSERT - Simple #3")]
    [TestCase("INSERT INTO actor_backup (SELECT * FROM actor)", TestName = "INSERT - Subquery")]
    [TestCase("INSERT INTO distributors AS d (did, dname) VALUES (8, 'Anvil Distribution')", TestName = "INSERT - Aliased")]
    [TestCase("INSERT INTO test_table SELECT * FROM actor", TestName = "INSERT - Subquery (no parenthesis)")]
    [TestCase("INSERT INTO test_table (SELECT * FROM actor)", TestName = "INSERT - Subquery (parenthesis)")]
    // DELETE
    [TestCase("DELETE FROM actor WHERE actor_id BETWEEN 15 AND 30", TestName = "DELETE - Simple #1")]
    [TestCase("DELETE FROM actor", TestName = "DELETE - Simple #2")]
    // UPDATE
    [TestCase("UPDATE actor SET actor_id = 1", TestName = "UPDATE - Simple #1")]
    [TestCase("UPDATE actor SET actor_id = 1, actor_name = 'Actor 1'", TestName = "UPDATE - Simple #2")]
    [TestCase("UPDATE actor SET actor_id = 1 WHERE actor_id = 999", TestName = "UPDATE - Simple #3")]
    [TestCase("UPDATE test SET (pk, id, name) = (SELECT pk, id, name FROM test1 WHERE pk = 1)", TestName = "UPDATE - Subquery #1")]
    [TestCase("UPDATE test SET count = test.count + a.count FROM (SELECT * FROM test1) AS a WHERE test.pk = a.pk", TestName = "UPDATE - Subquery #2")]
    [TestCase("UPDATE PRODUCT2 A SET NET_PRICE = A.PRICE - (A.PRICE * B.DISCOUNT) FROM PRODUCT_SEGMENT B WHERE A.SEGMENT_ID = B.ID;", TestName = "UPDATE - Join #1")]
    [TestCase(@"WITH t1 AS ( SELECT Product, Max(LastEditDate) AS MaxDate FROM t GROUP BY Product )
                UPDATE t
                SET Number=1
                FROM t1
                WHERE t.Product = t1.Product AND t.LastEditDate = t1.MaxDate;", TestName = "UPDATE - CTE #1")]
    // SET
    [TestCase("SET search_path TO myschema, public", TestName = "SET - search_path")]
    [TestCase("SET test_var TO 1", TestName = "SET - Simple #1")]
    public void Deparse(string query)
    {
        var res = Parser.Parse(query);
        var parser = new PostgreSqlParser();
        var result = parser.Parse(new QsiScript(query, QsiScriptType.Select));
        var deparser = new PostgreSqlDeparser();

        var expected = res.Stmts[0].Stmt.ToString();
        var actual = deparser.ConvertToPgNode(result).ToString();

        var expectedScript = Parser.Deparse(res.Stmts[0].Stmt);
        var actualScript = deparser.Deparse(result, default!);

        Console.WriteLine($"Original : {query}");
        Console.WriteLine($"Expected : {expectedScript}");
        Console.WriteLine($"Actual   : {actualScript}");

        Assert.AreEqual(expectedScript, actualScript);
    }
}
