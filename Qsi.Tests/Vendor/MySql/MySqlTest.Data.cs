using NUnit.Framework;

namespace Qsi.Tests.Vendor.MySql;

public partial class MySqlTest
{
    private static readonly TestCaseData[] Print_BindParam_TestDatas =
    {
        new("INSERT INTO actor (first_name, last_name) VALUES (?, ?)",
            new object[] { "MORRIS", "BABO" }),

        new("INSERT INTO actor SET first_name = ?, last_name = ?",
            new object[] { "MORRIS", "BABO" }),

        new("INSERT INTO actor SET actor.first_name = ?, actor.last_name = ?",
            new object[] { "MORRIS", "BABO" }),

        new("INSERT INTO actor SELECT ?, ?, ?, ?",
            new object[] { 1, "MORRIS", "BABO", null }),

        new("UPDATE actor SET first_name = ?, last_name = ? WHERE actor_id = ?",
            new object[] { "MORRIS", "BABO", 1 }),

        new("UPDATE actor SET actor.first_name = ?, actor.last_name = ? WHERE actor.actor_id = ?",
            new object[] { "MORRIS", "BABO", 1 }),

        new("UPDATE city, actor SET actor.first_name = ?, actor.last_name = ?, city.city_id = ? WHERE actor_id = ? LIMIT ?",
            new object[] { "MORRIS", "BABO", 2, 1, 1 }),

        new("UPDATE film_list SET title = ?, category = ? WHERE FID = ?",
            new object[] { "EVAN", "CHEQUER", 1 }),

        new("WITH CTE AS (SELECT ? a, ? b) UPDATE actor SET first_name = (SELECT a FROM CTE) + '!'",
            new object[] { "MORRIS", "BABO" }),

        new("DELETE first_name FROM actor_info a WHERE actor_id = ?",
            new object[] { 1 }),

        new("DELETE title, category FROM film_list WHERE FID = ?",
            new object[] { 1 }),

        new("WITH CTE AS (SELECT ?) DELETE title, category FROM film_list WHERE FID = ? AND (SELECT `1` FROM CTE) = ?",
            new object[] { 1, 1, 1 }),
    };

    private static readonly TestCaseData[] Print_TestDatas =
    {
        new("INSERT INTO actor (first_name, last_name) VALUES ('MORRIS', 'BABO')"),

        new("INSERT INTO actor SET first_name = 'MORRIS', last_name = 'BABO'"),

        new("INSERT INTO actor SET actor.first_name = 'MORRIS', actor.last_name = 'BABO'"),

        new("INSERT INTO actor SELECT 1, 'MORRIS', 'BABO', null"),

        new("INSERT INTO actor (actor_id, last_name) SELECT city_id, city FROM city LIMIT 2"),

        new("UPDATE actor SET first_name = 'MORRIS', last_name = 'BABO' WHERE actor_id = 1"),

        new("UPDATE actor SET actor.first_name = 'MORRIS', actor.last_name = 'BABO' WHERE actor.actor_id = 1"),

        new("UPDATE city, actor SET actor.first_name = 'MORRIS', actor.last_name = 'BABO', city.city_id = 2 WHERE actor_id = 1 LIMIT 1"),

        new("UPDATE film_list SET title = 'EVAN', category = 'CHEQUER' WHERE FID = 1"),

        new("WITH CTE AS (SELECT 'MORRIS' a, 'BABO' b) UPDATE actor SET first_name = (SELECT a FROM CTE) + '!'"),

        new("DELETE first_name FROM actor_info a WHERE actor_id = 1"),

        new("DELETE first_name, b FROM actor_info a, city b WHERE actor_id = city_id"),

        new("DELETE title, category FROM film_list WHERE FID = 1"),

        new("WITH CTE AS (SELECT 1) DELETE title, category FROM film_list WHERE FID = 1 AND (SELECT `1` FROM CTE) = 1")
    };

    private static readonly TestCaseData[] Test_InferredName_TestDatas = {
        
        // IQsiMultipleExpressionNode.Elements.Length == 1
        new("SELECT DISTINCT first_name FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT `first_name` FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT (first_name) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT`first_name` FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT(first_name) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT (`first_name`) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((first_name)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((`first_name`)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((`actor`.`first_name`)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((`actor`.`first_name`)) AS A FROM actor LIMIT 1;", new [] { "A" }),
        new("SELECT DISTINCT DISTINCT DISTINCT ((`actor`.`first_name`)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT (actor_id) FROM actor WHERE first_name = ANY(SELECT DISTINCT first_name FROM actor);", new[] { "actor_id" }),

        // IQsiMultipleExpressionNode.Elements.Length > 1
        new("SELECT actor_id IN (1, 2, 3) FROM actor LIMIT 1", new[] { "actor_id IN (1, 2, 3)" }),
        new("SELECT `actor_id` IN (1, 2, 3) FROM actor LIMIT 1", new[] { "`actor_id` IN (1, 2, 3)" }),
        new("SELECT (`actor_id`) IN (1, 2, 3) FROM actor LIMIT 1", new[] { "(`actor_id`) IN (1, 2, 3)" }),
        new("SELECT actor_id IN (`actor_id`) FROM actor LIMIT 1", new[] { "actor_id IN (`actor_id`)" }),
        
        // Not related but just test
        new("SELECT actor_id, first_name FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        new("SELECT `actor_id`, `first_name` FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        new("SELECT (actor_id), (first_name) FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        new("SELECT (`actor_id`), (`first_name`) FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        
        new("SELECT SUM(DISTINCT first_name) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT first_name)" }),
        new("SELECT SUM(DISTINCT `first_name`) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT `first_name`)" }),
        new("SELECT SUM(DISTINCT (first_name)) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT (first_name))" }),
        new("SELECT SUM(DISTINCT`first_name`) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT`first_name`)" }),
        new("SELECT SUM(DISTINCT(first_name)) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT(first_name))" }),
        new("SELECT SUM(DISTINCT (`first_name`)) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT (`first_name`))" }),
        new("SELECT SUM(DISTINCT ((first_name))) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT ((first_name)))" }),
        new("SELECT SUM(DISTINCT ((`first_name`))) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT ((`first_name`)))" }),
        new("SELECT SUM(actor_id) FROM actor WHERE first_name = ANY(select DISTINCT first_name FROM actor);", new[] { "SUM(actor_id)" }),
    };
}
