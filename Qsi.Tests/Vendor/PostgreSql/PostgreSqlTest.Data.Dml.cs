using System;
using NUnit.Framework;

namespace Qsi.Tests.PostgreSql;

public partial class PostgreSqlTest
{
    private static readonly TestCaseData[] InsertTestDatas =
    {
        new("INSERT INTO actor VALUES (1, 2, 3, 4), (5, 6, 7, 8)", Array.Empty<string>(), 1),
        new("INSERT INTO actor (actor_id) VALUES (1)", Array.Empty<string>(), 1),
        new("INSERT INTO actor (actor_id) VALUES (default)", Array.Empty<string>(), 1),
        new("INSERT INTO actor DEFAULT VALUES", Array.Empty<string>(), 1),
        new("INSERT INTO actor (actor_id, first_name, last_name) SELECT city_id, city FROM city", new[] { "SELECT city_id, city FROM city" }, 1),
        new("INSERT INTO actor SELECT * FROM actor LIMIT 0", new[] { "SELECT * FROM actor LIMIT 0" }, 1),
        new("INSERT INTO actor SELECT * FROM actor LIMIT 1", new[] { "SELECT * FROM actor LIMIT 1" }, 1),
        new("INSERT INTO actor SELECT * FROM actor LIMIT 2", new[] { "SELECT * FROM actor LIMIT 2" }, 1),

        // OVERRIDING
        new("INSERT INTO actor VALUES (1, 2, 3, 4) OVERRIDING SYSTEM VALUE", Array.Empty<string>(), 1),
        new("INSERT INTO actor VALUES (1, 2, 3, 4) OVERRIDING USER VALUE", Array.Empty<string>(), 1),

        // WITH Clause
        new("WITH cte AS (SELECT * FROM actor) INSERT INTO actor SELECT * FROM cte", new[] { "SELECT * FROM actor" }, 1),
        
        // ON CONFLICT Clause
        new("INSERT INTO city VALUES (1, 2, 3, now()) ON CONFLICT DO NOTHING", Array.Empty<string>(), 1),
        new("INSERT INTO city VALUES (1, 2, 3, now()) ON CONFLICT (city_id) DO NOTHING", Array.Empty<string>(), 1),
        new("INSERT INTO city VALUES (1, 2, 3, now()) ON CONFLICT (city_id) WHERE city_id > 10 DO NOTHING", Array.Empty<string>(), 1),
        new("INSERT INTO city VALUES (1, 2, 3, now()) ON CONFLICT ON CONSTRAINT city_pkey DO NOTHING", Array.Empty<string>(), 1),

        new("INSERT INTO actor VALUES (1, 2, 3, now()) ON CONFLICT (actor_id) DO UPDATE SET first_name = actor.first_name, last_name = excluded.last_name", Array.Empty<string>(), 1),
        new("INSERT INTO actor VALUES (1, 2, 3, now()) ON CONFLICT (actor_id) WHERE actor_id > 10 DO UPDATE SET first_name = actor.first_name, last_name = excluded.last_name", Array.Empty<string>(), 1),
        new("INSERT INTO actor VALUES (1, 2, 3, now()) ON CONFLICT (actor_id) DO UPDATE SET first_name = actor.first_name, last_name = excluded.last_name WHERE actor.actor_id > 10", Array.Empty<string>(), 1),
        new("INSERT INTO actor VALUES (1, 2, 3, now()) ON CONFLICT ON CONSTRAINT actor_pkey DO UPDATE SET first_name = actor.first_name, last_name = excluded.last_name", Array.Empty<string>(), 1),
        new("INSERT INTO actor VALUES (1, 2, 3, now()) ON CONFLICT ON CONSTRAINT actor_pkey DO UPDATE SET (first_name, last_name) = (actor.first_name, excluded.last_name)", Array.Empty<string>(), 1),
    };

    private static readonly TestCaseData[] UpdateTestDatas =
    {
        new("UPDATE actor SET actor_id = 1", new[] { "SELECT * FROM actor" }, 1),
        new("UPDATE actor SET actor_id = (SELECT city_id FROM city LIMIT 1)", new[] { "SELECT * FROM actor", "(SELECT city_id FROM city LIMIT 1)" }, 1),
        new("UPDATE actor SET actor_id = 1 WHERE false", new[] { "SELECT * FROM actor WHERE false" }, 1),
        new("UPDATE ONLY actor AS a SET actor_id = 1 WHERE a.actor_id > 10 AND false", new[] { "SELECT * FROM actor a WHERE a.actor_id > 10 AND false" }, 1),
        new("UPDATE actor AS a SET (actor_id, first_name) = (1, 'Foo') WHERE false", new[] { "SELECT * FROM actor a WHERE false" }, 1),
        new("UPDATE actor AS a SET (actor_id, first_name, last_name, last_update) = (SELECT * FROM actor) WHERE false", new[] { "SELECT * FROM actor", "SELECT * FROM actor a WHERE false" }, 1),
        
        // WITH Clause
        new("WITH cte AS (SELECT * FROM actor) UPDATE actor SET actor_id = cte.actor_id FROM cte WHERE false", new[] { "SELECT * FROM actor" }, 1),
        
        // FROM Clause
        new("UPDATE actor SET actor_id = city_id FROM city WHERE false", new[] { "SELECT * FROM actor WHERE false, SELECT * FROM city" }, 1),
        new("UPDATE actor SET actor_id = c.city_id FROM city c WHERE false", new[] { "SELECT * FROM actor WHERE false" }, 1),
        new("UPDATE actor SET actor_id = c.city_id FROM city c WHERE c.city_id = 2 AND false", new[] { "SELECT * FROM actor WHERE false" }, 1),
    };

    private static readonly TestCaseData[] DeleteTestDatas =
    {
        new("DELETE FROM actor", new[] { "SELECT * FROM actor" }, 1),
        new("DELETE FROM ONLY actor* AS a", new[] { "SELECT * FROM actor*" }, 1),
        new("DELETE FROM actor AS a WHERE actor_id = 1", new[] { "SELECT * FROM actor" }, 1),

        // WITH Clause
        new("WITH cte AS (SELECT * FROM city) DELETE FROM actor WHERE actor_id = city_id", new [] { "SELECT * FROM actor", "SELECT * FROM city" }, 1),
        
        // FROM Clause
        new("DELETE FROM actor AS a USING city WHERE actor_id = city_id", new[] { "SELECT * FROM actor", "SELECT * FROM city" }, 1),
    };

    private static readonly TestCaseData[] ParameterTestDatas =
    {
        new("INSERT INTO actor (actor_id, first_name, last_name) SELECT $1, $2 FROM city", new object[] { "city_id", "city" }),
        new("INSERT INTO actor VALUES (default, $1, $2)", new object[] { "Mason", "Oh" }),
        new("INSERT INTO city VALUES (default, $1, $2, now()) ON CONFLICT (city_id) WHERE city_id > $3 DO NOTHING", new object[] { "Mason", "Oh", 10 }),

        new("INSERT INTO actor VALUES (default, $1, $2, now()) ON CONFLICT (actor_id) DO UPDATE SET first_name = $3, last_name = $4", new object[] { "Mason", "Oh", "Manos", "Ho" }),
        new("INSERT INTO actor VALUES (default, $1, $2, now()) ON CONFLICT ON CONSTRAINT actor_pkey DO UPDATE SET first_name = $3, last_name = $4", new object[] { "Mason", "Oh", "actor_pkey", "Manos", "Ho" }),
        new("INSERT INTO actor VALUES (1, 2, 3, now()) ON CONFLICT ON CONSTRAINT actor_pkey DO UPDATE SET (first_name, last_name) = ($1, $2)", new object[] { "Mason", "Oh" }),
        
        new("WITH cte AS (SELECT 1, $1, $2, now()) INSERT INTO actor SELECT * FROM cte", new object[] { "Mason", "Oh" }),
        
        new("UPDATE actor SET actor_id = $1", new object[] { 1 }),
        new("UPDATE actor SET actor_id = (SELECT city_id FROM city LIMIT $1)", new object[] { 1 }),
        
        new("UPDATE actor AS a SET (actor_id, first_name, last_name, last_update) = (1, $1, $2, now()) WHERE false", new object[] { "Mason", "Oh" }),
        new("UPDATE actor SET actor_id = $1 FROM city c WHERE c.city_id = $2 AND false", new object[] { 1, "Mason" }),
        
        new("DELETE FROM actor AS a WHERE actor_id = $1", new object[] { 1 }),
    };
    
    private static readonly TestCaseData[] NotSupportedDmlTestDatas =
    {
        // DML with Returning Clause
    };
}
