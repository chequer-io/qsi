using System;
using NUnit.Framework;

namespace Qsi.Tests.Vendor.PostgreSQL;

public partial class PostgreSqlTest
{
    private static readonly TestCaseData[] Select_TestDatas =
    {
        new("SELECT 1"),
        new("SELECT * FROM actor"),
        new("SELECT * FROM city"),
        new("SELECT actor_id, first_name FROM actor"),
        new("SELECT actor.actor_id, actor.first_name FROM actor"),
        new("SELECT * FROM actor a JOIN city c ON a.actor_id = c.city_id"),
        new("SELECT * FROM actor UNION SELECT * FROM actor"),
        new("SELECT * FROM (SELECT * FROM actor) a"),
        new("SELECT * FROM (SELECT actor_id FROM actor) a"),
        new("SELECT * FROM (SELECT * FROM actor, film_actor) af"),
        new("SELECT * FROM (SELECT actor_id FROM actor, film_actor) af"),
        new("SELECT (SELECT actor_id) FROM actor"),
        new("SELECT (SELECT actor_id FROM film_actor LIMIT 1) FROM actor"),
        new("SELECT (SELECT a.actor_id FROM actor LIMIT 1) from actor a"),
        new("SELECT (SELECT actor.actor_id FROM film_actor AS actor LIMIT 1) FROM actor"),
        new("SELECT (SELECT actor_id, film_id) FROM actor, film_actor"),
        new("WITH RECURSIVE CTE AS (SELECT 1 N UNION ALL SELECT N + 1 FROM CTE WHERE N < 10) SELECT * FROM CTE"),
        new("SELECT (ROW (actor_id, first_name, last_name)).f2 from actor;"),
    };

    private static readonly TestCaseData[] Table_TestDatas =
    {
        new("TABLE actor"),
        new("TABLE city"),
    };

    private static readonly TestCaseData[] Select_ColumnName_TestDatas =
    {
        new("SELECT 1, .2, 0.3, 0.4E+5") { ExpectedResult = new[] { "?column?", "?column?", "?column?", "?column?" } },
        new("SELECT 1 + 2, 3 +/*cmt*/ 4") { ExpectedResult = new[] { "?column?", "?column?" } },
        new("SELECT 'Test'") { ExpectedResult = new[] { "?column?" } },
        // new("SELECT _utf8mb4 'Test'") { ExpectedResult = new[] { "Test" } },
        // new("SELECT _utf8mb4 'Test' collate utf8mb4_unicode_ci") { ExpectedResult = new[] { "_utf8mb4 'Test' collate utf8mb4_unicode_ci" } },
        new("SELECT N'National'") { ExpectedResult = new[] { "bpchar" } },
        new("SELECT X'0F', 0x0F") { ExpectedResult = new[] { "?column?", "x0F" } },
        new("SELECT B'0101', 0b0101") { ExpectedResult = new[] { "?column?", "b0101" } },
        new("SELECT NOW(), NOW(/*hi*/)") { ExpectedResult = new[] { "now", "now" } },
        new("SELECT actor_id FROM actor") {ExpectedResult = new[] {"actor_id"}},
        new("SELECT * FROM actor") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT * FROM actor a") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT a.* FROM actor a") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT a.actor_id AS \"hey~\" FROM actor a") { ExpectedResult = new[] { "\"hey~\"" } },
        new("SELECT * FROM actor JOIN film_actor USING (actor_id) LIMIT 0") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update", "film_id", "last_update" } },
        new("SELECT a.* FROM actor a JOIN film_actor f USING (actor_id) LIMIT 0") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT f.* FROM actor a JOIN film_actor f USING (actor_id) LIMIT 0") { ExpectedResult = new[] { "actor_id", "film_id", "last_update" } },
        new("SELECT * FROM actor JOIN film_actor USING (actor_id, last_update) LIMIT 0") { ExpectedResult = new[] { "actor_id", "last_update", "first_name", "last_name", "film_id" } },
        new("(SELECT 1) UNION (SELECT 2)") { ExpectedResult = new[] { "?column?" } },
    };

    private static readonly TestCaseData[] Insert_TestDatas =
    {
        new("INSERT INTO actor VALUES (1, 2, 3, 4), (5, 6, 7, 8)", Array.Empty<string>(), 1),
        new("INSERT INTO actor (actor_id) VALUES (1)", Array.Empty<string>(), 1),
        new("INSERT INTO actor (actor_id, last_name) SELECT city_id, city FROM city", new[] { "SELECT city_id, city FROM city" }, 1),
        new("INSERT INTO actor SELECT * FROM actor LIMIT 0", new[] { "SELECT * FROM actor LIMIT 0" }, 1),
        new("INSERT INTO actor SELECT * FROM actor LIMIT 1", new[] { "SELECT * FROM actor LIMIT 1" }, 1),
        new("INSERT INTO actor SELECT * FROM actor LIMIT 2", new[] { "SELECT * FROM actor LIMIT 2" }, 1),
        new("INSERT INTO actor SET actor_id = 1", Array.Empty<string>(), 1),
        new("INSERT INTO actor SET actor.actor_id = 1", Array.Empty<string>(), 1),
        new("INSERT INTO actor VALUES (1, 'foo', 'bar', now()) on conflict (actor_id) do UPDATE set last_update = now()", Array.Empty<string>(), 1),
    };

    private static readonly TestCaseData[] Delete_TestDatas =
    {
        new("DELETE FROM actor", new[] { "SELECT * FROM actor" }, 1),
        new("DELETE actor FROM actor", new[] { "SELECT actor.* FROM actor" }, 1),
        new("DELETE actor.* FROM actor WHERE 1=2", new[] { "SELECT actor.* FROM actor WHERE 1=2" }, 1),
        new("DELETE actor_id FROM actor", new[] { "SELECT actor_id FROM actor" }, 1),
        new("DELETE actor.actor_id FROM actor", new[] { "SELECT actor.actor_id FROM actor" }, 1),
        new("DELETE FROM actor AS a", new[] { "SELECT * FROM actor" }, 1),
        new("DELETE a FROM actor AS a", new[] { "SELECT a.* FROM actor AS a" }, 1),
        new("DELETE a.* FROM actor AS a", new[] { "SELECT a.* FROM actor AS a" }, 1),
        new("DELETE actor_id FROM actor AS a", new[] { "SELECT actor_id FROM actor AS a" }, 1),
        new("DELETE a.actor_id FROM actor AS a", new[] { "SELECT a.actor_id FROM actor AS a" }, 1),
        new("DELETE a, c FROM actor a JOIN city c ON false", new[] { "SELECT a.*, c.* FROM actor a JOIN city c ON false" }, 2),
        new("DELETE a, c, film FROM actor a JOIN city c JOIN film WHERE false", new[] { "SELECT a.*, c.*, film.* FROM actor a JOIN city c JOIN film WHERE false" }, 3),
        new("DELETE c FROM address a JOIN city c USING (city_id) WHERE false", new[] { "SELECT c.* FROM address a JOIN city c USING (city_id) WHERE false" }, 1),
    };

    private static readonly TestCaseData[] Update_TestDatas =
    {
        new("UPDATE actor SET actor_id = 1", new[] { "SELECT * FROM actor" }, 1),
        new("UPDATE actor SET actor.actor_id = 1", new[] { "SELECT * FROM actor" }, 1),
        new("UPDATE actor SET actor_id = (SELECT city_id FROM city LIMIT 1)", new[] { "SELECT * FROM actor", "(SELECT city_id FROM city LIMIT 1)" }, 1),
        new("UPDATE actor SET actor_id = 1 WHERE false", new[] { "SELECT * FROM actor WHERE false" }, 1),
        new("UPDATE actor AS a SET a.actor_id = 1 WHERE false", new[] { "SELECT * FROM actor AS a WHERE false" }, 1),
        new("UPDATE actor, city SET city_id = 2, actor_id = 1 WHERE false", new[] { "SELECT * FROM actor, city WHERE false" }, 2),
        new("UPDATE actor, city SET city.city_id = 2, actor.actor_id = 1 WHERE false", new[] { "SELECT * FROM actor, city WHERE false" }, 2),
        new("UPDATE actor a JOIN city c ON false JOIN film f ON false SET a.last_update = null, c.last_update = null, f.last_update = null", new[] { "SELECT * FROM actor a JOIN city c ON false JOIN film f ON false" }, 3),
        new("UPDATE address a JOIN city c USING (city_id) SET c.city = 1, a.address_id = 2 WHERE false", new[] { "SELECT * FROM address a JOIN city c USING (city_id) WHERE false" }, 2),
        new("UPDATE address a JOIN city c USING (city_id) SET c.last_update = 1, a.last_update = 2 WHERE false", new[] { "SELECT * FROM address a JOIN city c USING (city_id) WHERE false" }, 2),
    };

    private static readonly TestCaseData[] Throw_TestDatas =
    {
        new("SELECT * FROM x.y") { ExpectedResult = "QSI-0006: Unable to resolve table 'x.y'" },
        new("SELECT a.* FROM actor") { ExpectedResult = "QSI-0008: Unknown table 'a'" },
        new("INSERT INTO actor VALUES (1, 2, 3, 4), (5, 6, 7)") { ExpectedResult = "QSI-001B: Column count doesn't match value count at row 2" },
        new("INSERT INTO actor (aaaaaaaa) VALUES (1)") { ExpectedResult = "QSI-000C: Unknown column 'aaaaaaaa'" },
        new("UPDATE actor SET actor_id = 1, actor_id = 2") { ExpectedResult = "QSI-0001: 'Multiple set column' is not supported feature" },
        new("UPDATE actor SET bbb = 2") { ExpectedResult = "QSI-000C: Unknown column 'bbb'" },
        new("UPDATE actor_info SET film_info = 2") { ExpectedResult = "QSI-001A: Column 'film_info' is not updatable" },
        new("UPDATE address a JOIN city c USING (city_id) SET c.city_id = 1, a.address_id = 2 WHERE false") { ExpectedResult = "QSI-001A: Column 'c.city_id' is not updatable" },
        
        // TODO: Implement expected results.
        new("SELECT public.actor.actor_id, public.actor.first_name FROM public.actor"),
        new("SELECT postgres.public.actor.actor_id, postgres.public.actor.first_name FROM postgres.public.actor"),
    };

    private static readonly TestCaseData[] Print_BindParam_TestDatas =
    {
        new("INSERT INTO public.actor (first_name, last_name) VALUES ($1, $2)",
            new object[] { "MORRIS", "BABO" }),

        new("INSERT INTO public.actor SET first_name = $1, last_name = $2",
            new object[] { "MORRIS", "BABO" }),

        new("INSERT INTO public.actor SET actor.first_name = $1, actor.last_name = $2",
            new object[] { "MORRIS", "BABO" }),

        new("INSERT INTO public.actor SELECT $1, $2, $3, $4",
            new object[] { 1, "MORRIS", "BABO", null }),

        new("UPDATE public.actor SET first_name = $1, last_name = $2 WHERE actor_id = $3",
            new object[] { "MORRIS", "BABO", 1 }),

        new("UPDATE public.actor SET actor.first_name = $1, actor.last_name = $2 WHERE actor.actor_id = $3",
            new object[] { "MORRIS", "BABO", 1 }),

        new("UPDATE public.city, public.actor SET actor.first_name = $1, actor.last_name = $2, city.city_id = $3 WHERE actor_id = $4 LIMIT $5",
            new object[] { "MORRIS", "BABO", 2, 1, 1 }),

        new("UPDATE public.film SET title = $1, description = $2 WHERE film_id = $3",
            new object[] { "EVAN", "CHEQUER", 1 }),

        new("WITH CTE AS (SELECT $1 a, $2 b) UPDATE public.actor SET first_name = (SELECT a FROM CTE) + '!'",
            new object[] { "MORRIS", "BABO" }),

        new("DELETE FROM public.actor_info a WHERE actor_id = $1",
            new object[] { 1 }),

        new("DELETE FROM public.film WHERE film_id = $1",
            new object[] { 1 }),

        new("WITH CTE AS (SELECT $1) DELETE FROM film WHERE film_id = $2 AND (SELECT `1` FROM CTE) = $3",
            new object[] { 1, 1, 1 }),
    };

    private static readonly TestCaseData[] Print_TestDatas =
    {
        new("INSERT INTO public.actor (first_name, last_name) VALUES ('MORRIS', 'BABO')"),

        new("INSERT INTO public.actor SET first_name = 'MORRIS', last_name = 'BABO'"),

        new("INSERT INTO public.actor SET actor.first_name = 'MORRIS', actor.last_name = 'BABO'"),

        new("INSERT INTO public.actor SELECT 1, 'MORRIS', 'BABO', null"),

        new("INSERT INTO public.actor (actor_id, last_name) SELECT city_id, city FROM city LIMIT 2"),

        new("UPDATE public.actor SET first_name = 'MORRIS', last_name = 'BABO' WHERE actor_id = 1"),

        new("UPDATE public.actor SET actor.first_name = 'MORRIS', actor.last_name = 'BABO' WHERE actor.actor_id = 1"),

        new("UPDATE public.city, public.actor SET actor.first_name = 'MORRIS', actor.last_name = 'BABO', city.city_id = 2 WHERE actor_id = 1 LIMIT 1"),

        new("UPDATE public.film SET title = 'EVAN', category = 'CHEQUER' WHERE FID = 1"),

        new("WITH CTE AS (SELECT 'MORRIS' a, 'BABO' b) UPDATE public.actor SET first_name = (SELECT a FROM CTE) + '!'"),

        new("DELETE FROM public.actor_info a WHERE actor_id = 1"),

        new("DELETE FROM public.actor_info a, public.city b WHERE actor_id = city_id"),

        new("DELETE FROM public.film WHERE film_id = 1"),

        new("WITH CTE AS (SELECT 1) DELETE FROM public.film WHERE film_id = 1 AND (SELECT `1` FROM CTE) = 1")
    };
}
