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

    private static readonly TestCaseData[] Test_InferredName_TestDatas =
    {
        new("SELECT DISTINCT first_name FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT `first_name` FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT (first_name) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT`first_name` FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT(first_name) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT(`first_name`) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT (`first_name`) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((first_name)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((`first_name`)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((`actor`.`first_name`)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT ((`actor`.`first_name`)) AS A FROM actor LIMIT 1;", new[] { "A" }),
        new("SELECT DISTINCT DISTINCT DISTINCT ((`actor`.`first_name`)) FROM actor LIMIT 1;", new[] { "first_name" }),
        new("SELECT DISTINCT (actor_id) FROM actor WHERE first_name = ANY(SELECT DISTINCT first_name FROM actor);", new[] { "actor_id" }),
        new("SELECT actor_id IN (1, 2, 3) FROM actor LIMIT 1", new[] { "actor_id IN (1, 2, 3)" }),
        new("SELECT `actor_id` IN (1, 2, 3) FROM actor LIMIT 1", new[] { "`actor_id` IN (1, 2, 3)" }),
        new("SELECT (`actor_id`) IN (1, 2, 3) FROM actor LIMIT 1", new[] { "(`actor_id`) IN (1, 2, 3)" }),
        new("SELECT actor_id IN (`actor_id`) FROM actor LIMIT 1", new[] { "actor_id IN (`actor_id`)" }),
        new("SELECT (actor_id IN (`actor_id`)) FROM actor LIMIT 1", new[] { "(actor_id IN (`actor_id`))" }),


        new("SELECT actor_id, first_name FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        new("SELECT `actor_id`, `first_name` FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        new("SELECT (actor_id), (first_name) FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        new("SELECT (`actor_id`), (`first_name`) FROM actor LIMIT 1;", new[] { "actor_id", "first_name" }),
        new("SELECT (`actor`.`actor_id`), (`actor`.first_name) FROM actor LIMIT 1", new[] { "actor_id", "first_name" }),

        new("SELECT SUM(DISTINCT first_name) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT first_name)" }),
        new("SELECT SUM(DISTINCT `first_name`) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT `first_name`)" }),
        new("SELECT SUM(DISTINCT (first_name)) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT (first_name))" }),
        new("SELECT SUM(DISTINCT`first_name`) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT`first_name`)" }),
        new("SELECT SUM(DISTINCT(first_name)) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT(first_name))" }),
        new("SELECT SUM(DISTINCT (`first_name`)) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT (`first_name`))" }),
        new("SELECT SUM(DISTINCT ((first_name))) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT ((first_name)))" }),
        new("SELECT SUM(DISTINCT ((`first_name`))) FROM actor LIMIT 1;", new[] { "SUM(DISTINCT ((`first_name`)))" }),
        new("SELECT SUM(actor_id) FROM actor WHERE first_name = ANY(select DISTINCT first_name FROM actor);", new[] { "SUM(actor_id)" }),

        new("SELECT @actor_id:=@actor_ID+1 FROM actor;", new[] { "@actor_id:=@actor_ID+1" }),
        new("SELECT ABS(LAG(rental_id, 1, rental_id) OVER (ORDER BY rental_id) - rental_id) FROM payment LIMIT 1;",
            new[] { "ABS(LAG(rental_id, 1, rental_id) OVER (ORDER BY rental_id) - rental_id)" }),

        new("SELECT ('A' | 'B');", new[] { "('A' | 'B')" }),
        new("SELECT ('A' || 'B');", new[] { "('A' || 'B')" }),
        new("SELECT ('A' xor 'B');", new[] { "('A' xor 'B')" }),
        new("SELECT (('A') | ('B'));", new[] { "(('A') | ('B'))" }),
        new("SELECT actor_id IN (('A') | ('B')) FROM actor LIMIT 1;", new[] { "actor_id IN (('A') | ('B'))" }),
        new("SELECT 1 + 2;", new[] { "1 + 2" }),
        new("SELECT (('1 + 2'));", new[] { "1 + 2" }),
        new("SELECT 1 + (2);", new[] { "1 + (2)" }),
        new("SELECT 1 + (2) IN (2);", new[] { "1 + (2) IN (2)" }),
        new("SELECT 1 + (2) IN (2, 3);", new[] { "1 + (2) IN (2, 3)" }),
        new("SELECT 1 + (2) IN (2, (3));", new[] { "1 + (2) IN (2, (3))" }),
        new("SELECT (2 * 3) | (1 & 7);", new[] { "(2 * 3) | (1 & 7)" }),
        new("SELECT (('A' | 'B') | (1 + 7));", new[] { "(('A' | 'B') | (1 + 7))" }),


        // PredicateExprIn:         IN_SYMBOL (subquery | OPEN_PAR_SYMBOL exprList CLOSE_PAR_SYMBOL)
        new("SELECT actor_id IN (SELECT actor_id from actor) FROM actor LIMIT 1;", new[] { "actor_id IN (SELECT actor_id from actor)" }),
        new("SELECT actor_id IN (1, 2, 3) FROM actor LIMIT 1;", new[] { "actor_id IN (1, 2, 3)" }),

        // PredicateExprBetween:    BETWEEN_SYMBOL bitExpr AND_SYMBOL predicate
        new("SELECT actor_id BETWEEN 1 AND 3 FROM actor LIMIT 1;", new[] { "actor_id BETWEEN 1 AND 3" }),
        new("SELECT actor_id BETWEEN (1) AND (3) FROM actor LIMIT 1;", new[] { "actor_id BETWEEN (1) AND (3)" }),
        new("SELECT actor_id BETWEEN (SELECT actor_id FROM actor LIMIT 1) AND (3) FROM actor LIMIT 1;", new[] { "actor_id BETWEEN (SELECT actor_id FROM actor LIMIT 1) AND (3)" }),

        // PredicateExprLike:       LIKE_SYMBOL simpleExpr (ESCAPE_SYMBOL simpleExpr)?
        new("SELECT first_name LIKE '%A%' FROM actor LIMIT 1;", new[] { "first_name LIKE '%A%'" }),
        new("SELECT first_name LIKE ('%A%' OR '%B%') FROM actor LIMIT 1;", new[] { "first_name LIKE ('%A%' OR '%B%')" }),

        // BinaryExpr
        new("SELECT 1 IN (2);", new[] { "1 IN (2)" }),

        // BitExpr(INTERVAL): <bitExpr> operator INTERVAL <expr> <interval>
        new("SELECT NOW();", new[] { "NOW()" }),
        new("SELECT DATE_ADD(NOW(), INTERVAL 1 YEAR);", new[] { "DATE_ADD(NOW(), INTERVAL 1 YEAR)" }),
        new("SELECT DATE_ADD(NOW(), INTERVAL 1 YEAR);", new[] { "DATE_ADD(NOW(), INTERVAL 1 YEAR)" }),
        new("SELECT DATE_ADD(NOW(), INTERVAL 1 YEAR_MONTH);", new[] { "DATE_ADD(NOW(), INTERVAL 1 YEAR_MONTH)" }),
        new("SELECT NOW() + INTERVAL 1 YEAR_MONTH;", new[] { "NOW() + INTERVAL 1 YEAR_MONTH" }),
        new("SELECT (NOW()) + INTERVAL 1 YEAR_MONTH;", new[] { "(NOW()) + INTERVAL 1 YEAR_MONTH" }),

        // SimpleExprList:          ROW_SYMBOL? OPEN_PAR_SYMBOL exprList CLOSE_PAR_SYMBOL
        new("SELECT (1) FROM actor LIMIT 1;", new[] { "1" }),
        new("SELECT (1), (2) FROM actor LIMIT 1;", new[] { "1", "2" }),
        new("SELECT (1), (2) FROM actor LIMIT 1;", new[] { "1", "2" }),

        // SimpleExprMatch:         MATCH_SYMBOL identListArg AGAINST_SYMBOL OPEN_PAR_SYMBOL bitExpr fulltextOptions? CLOSE_PAR_SYMBOL
        // NOTE: Table must descript FULLTEXT indexing.
        // FULLTEXT SEARCH do not affect to search result columns naming.
        // new("SELECT * FROM actor WHERE MATCH(last_name) AGAINST('GUINESS');", new[] { "" }),

        new("SELECT * FROM actor LIMIT 1;", new[] { "actor_id", "first_name", "last_name", "last_update" }),
        new("SELECT * FROM actor a INNER JOIN actor_info ai USING (actor_id) LIMIT 1;", new[] { "actor_id", "first_name", "last_name", "last_update", "first_name", "last_name", "film_info" }),
        new("SELECT actor_id FROM actor a INNER JOIN actor_info ai USING (actor_id) LIMIT 1;", new[] { "actor_id" }),
        new("SELECT actor_id, a.first_name, ai.film_info FROM actor a INNER JOIN actor_info ai USING (actor_id) LIMIT 1;", new[] { "actor_id", "first_name", "film_info" }),
        new("SELECT f.title, fl.title FROM film AS f JOIN film_list AS fl LIMIT 1;", new[] { "title", "title" }),
        new("SELECT (f.title), (fl.title) FROM film AS f JOIN film_list AS fl LIMIT 1;", new[] { "title", "title" }),
        new("SELECT DISTINCT (f.title), (fl.title) FROM film AS f JOIN film_list AS fl LIMIT 1;", new[] { "title", "title" }),
    };

    private static readonly TestCaseData[] Test_LeadLagInfo_TestDatas = 
    {
        // QCP-1303 : https://chequer.atlassian.net/browse/QP-5465?focusedCommentId=48962
        new("SELECT LAG(first_name, 4294967295) OVER(ORDER BY actor_id) FROM actor;"),
        new("SELECT LAG(first_name, IF(TRUE, 2, 1)) OVER(ORDER BY actor_id) FROM actor;"),
        new("SELECT LAG(first_name, IF(TRUE, 2, 1) + IF(FALSE, 2, 1)) OVER(ORDER BY actor_id) FROM actor;"),
        
        new("SELECT LEAD(first_name, 4294967295) OVER(ORDER BY actor_id) FROM actor;"),
        new("SELECT LEAD(first_name, IF(TRUE, 2, 1)) OVER(ORDER BY actor_id) FROM actor;"),
        new("SELECT LEAD(first_name, IF(TRUE, 2, 1) + IF(FALSE, 2, 1)) OVER(ORDER BY actor_id) FROM actor;"),
        
        new("SELECT NTILE(4294967295) OVER(ORDER BY actor_id) FROM actor;"),
        new("SELECT NTILE(IF(TRUE, 2, 1)) OVER(ORDER BY actor_id) FROM actor;"),
        new("SELECT NTILE(IF(TRUE, 2, 1) + IF(FALSE, 2, 1)) OVER(ORDER BY actor_id) FROM actor;")
    };
}
