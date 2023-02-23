using NUnit.Framework;

namespace Qsi.Tests.PostgreSql;

public partial class PostgreSqlTest
{
    /// <summary>
    /// Basic test data for SELECT.
    /// </summary>
    /// <param name="query (param #0)">Query string.</param>
    private static readonly TestCaseData[] BasicSelectTestDatas =
    {
        new("SELECT * FROM actor"),
        new("SELECT * FROM city"),
        new("SELECT * FROM film"),

        new("SELECT * FROM actor a"),
        new("SELECT a.* FROM actor a"),

        new("SELECT actor_id(a) FROM actor a"),

        new("SELECT count(a) FROM actor a"),
        new("SELECT count(a.*) FROM actor a"),
        new("SELECT count(*) FROM actor a"),

        new("TABLE actor"),
        new("TABLE city"),
        new("TABLE film"),

        new("SELECT * FROM actor a JOIN city c USING (last_update)"),
        new("SELECT * FROM customer cu JOIN address a ON cu.address_id = a.address_id JOIN city ON a.city_id = city.city_id JOIN country ON city.country_id = country.country_id"),

        new("SELECT * FROM actor a, actor_info i WHERE a.actor_id = i.actor_id AND length(i.film_info) < 345;"),
        new("SELECT * FROM actor a INNER JOIN actor_info i ON (a.actor_id = i.actor_id) WHERE length(i.film_info) < 345;"),
        new("SELECT * FROM actor a NATURAL JOIN actor_info i WHERE length(i.film_info) < 345;"),

        new("SELECT * FROM actor a, LATERAL (SELECT * FROM city WHERE city.id = a.actor_id) abc"),

        new("SELECT * FROM actor a ORDER BY a.*"),
        new("SELECT country_id, max(last_update), count(*) FROM city GROUP BY country_id ORDER BY 3 DESC;"),

        new("SELECT * FROM film_list UNION SELECT * FROM nicer_but_slower_film_list"),
        new("(SELECT 1) UNION (SELECT 2)"),

        new("SELECT * FROM (SELECT actor_id FROM actor) a;"),
        new("SELECT * FROM (SELECT actor_id FROM actor, city) ac;"),

        new("SELECT (SELECT actor_id) FROM actor;"),
        new("SELECT (SELECT actor_id FROM city LIMIT 1) FROM actor;"),
        new("SELECT (SELECT a.actor_id FROM actor LIMIT 1) from actor a;"),

        new("SELECT (SELECT actor.actor_id FROM city AS a LIMIT 1) FROM actor; -- This works"),
        new("SELECT (SELECT actor_id), (SELECT city_id) FROM actor, city; -- This works"),
        new("SELECT (SELECT actor_id FROM city limit 1) FROM actor; -- This works"),
        new("SELECT (SELECT actor_id FROM actor limit 1) FROM actor_info; -- This works"),

        new("SELECT * FROM (VALUES (1, 1), (2, 2), (3, 3)) AS tbl;"),

        new("WITH RECURSIVE CTE AS (SELECT 1 N UNION ALL SELECT N + 1 FROM CTE WHERE N < 10) SELECT * FROM CTE"),
    };

    private static readonly TestCaseData[] FunctionTestDatas =
    {
        new("SELECT current_catalog"),
        new("SELECT current_database()"),
        new("SELECT current_query()"),
        new("SELECT current_schema()"),
        new("SELECT current_schema"),
        new("SELECT current_role"),
        new("SELECT has_database_privilege('postgres', 'dvdrental', 'create');"),
        new("SELECT pg_table_is_visible('actor'::regclass::oid);"),
        new("SELECT pg_typeof(1);"),
        new("SELECT generate_series(1, 5);"),
        new("SELECT coalesce(special_features[4], special_features[3], '(none at index 3 and 4)') from film;"),
        new("SELECT nullif(special_features[3:], '{}'::text[]) from film;"),
        new("SELECT greatest(ARRAY[1, 2, 3]);"),
        new("SELECT least(ARRAY[1, 2, 3]);"),
    };

    private static readonly TestCaseData[] SystemTableFunctionTestDatas =
    {
        new("SELECT * FROM pg_get_keywords()") { ExpectedResult = new[] { "word", "catcode", "catdesc" } },
        new("SELECT c.checkpoint_lsn, c.timeline_id FROM pg_control_checkpoint as c") { ExpectedResult = new[] { "checkpoint_lsn", "timeline_id" } },
        new("SELECT * FROM current_catalog") { ExpectedResult = new[] { "current_catalog" } },
        new("SELECT * FROM current_catalog WITH ORDINALITY") { ExpectedResult = new[] { "current_catalog", "ordinality" } },
        new("SELECT * FROM current_dialog WITH ORDINALITY as x ( t1, t2 )") { ExpectedResult = new[] { "t1", "t2" } },
        new("SELECT * FROM ROWS FROM ( current_catalog, current_user, pg_get_keywords() )") { ExpectedResult = new[] { "current_catalog", "current_user", "word", "catcode", "catdesc" } },
        new("SELECT current_database.* FROM ROWS FROM ( current_database(), current_user )") { ExpectedResult = new[] { "current_database, current_user" } },

        new("SELECT * FROM unnest(ARRAY [1, 2, 3], ARRAY [4, 5])") { ExpectedResult = new[] { "unnest.unnest", "unnest.unnest" } },
        new("SELECT * FROM unnest(ARRAY[(SELECT actor_id FROM actor LIMIT 1),2], ARRAY['foo','bar','baz']);") { ExpectedResult = new[] { "unnest.unnest", "unnest.unnest" } },

        new("SELECT (pg_get_keywords()).*") { ExpectedResult = new[] { "word", "catcode", "catdesc" } },
    };

    /// <summary>
    /// Test data for testing system columns.
    /// </summary>
    /// <param name="query (param #0)">Query string.</param>
    private static readonly TestCaseData[] SystemColumnTestDatas =
    {
        new("SELECT CTID, TABLEOID, XMIN, XMAX, CMIN, CMAX from actor"),
        new("SELECT abc.TABLEOID from actor abc;"),
        new("SELECT (SELECT a.CTID) as ctid from actor a")
    };

    /// <summary>
    /// Test data for SELECT, especially for column name.
    /// </summary>
    /// <param name="string query (param #0)">Query string.</param>
    /// <param name="string[] columnNames (param #1)">Expected column names.</param>
    private static readonly TestCaseData[] ColumnNameSelectTestDatas =
    {
        // Basic test queries
        new("SELECT * FROM actor") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT * FROM actor a") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT a.* FROM actor a") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT a.actor_id AS \"hey~\" FROM actor a") { ExpectedResult = new[] { "hey~" } },
        new("SELECT * FROM actor JOIN film_actor USING (actor_id) LIMIT 0") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update", "film_id", "last_update" } },
        new("SELECT a.* FROM actor a JOIN film_actor f USING (actor_id) LIMIT 0") { ExpectedResult = new[] { "actor_id", "first_name", "last_name", "last_update" } },
        new("SELECT f.* FROM actor a JOIN film_actor f USING (actor_id) LIMIT 0") { ExpectedResult = new[] { "actor_id", "film_id", "last_update" } },
        new("SELECT * FROM actor JOIN film_actor USING (actor_id, last_update) LIMIT 0") { ExpectedResult = new[] { "actor_id", "last_update", "first_name", "last_name", "film_id" } },
        new("SELECT * FROM (SELECT actor_id FROM actor) a;") { ExpectedResult = new[] { "actor_id" } },
        new("SELECT * FROM (SELECT actor_id FROM actor, city) ac;") { ExpectedResult = new[] { "actor_id" } },

        // System functions
        new("SELECT * FROM pg_get_keywords()") { ExpectedResult = new[] { "word", "catcode", "catdesc" } },
        new("SELECT checkpoint_lsn, redo_lsn, timeline_id, checkpoint_time FROM pg_control_checkpoint()") { ExpectedResult = new[] { "checkpoint_lsn", "redo_lsn", "timeline_id", "checkpoint_time" } },
        new("SELECT * FROM pg_prepared_statement") { ExpectedResult = new[] { "name", "statement", "prepare_time", "parameter_Types", "from_sql" } },

        // System views
        new("SELECT * FROM pg_available_extensions") { ExpectedResult = new[] { "name", "default_version", "installed_version", "comment" } },

        // new("(SELECT 1) UNION (SELECT 2)") { ExpectedResult = new[] { "1" } },

        // Currently, subquery in a column is not supported.
        // new("SELECT (SELECT actor_id) FROM actor;") { ExpectedResult = new [] { "actor_id" } },
        // new("SELECT (SELECT actor_id FROM city LIMIT 1) FROM actor;") { ExpectedResult = new [] { "actor_id" } },
        // new("SELECT (SELECT a.actor_id FROM actor LIMIT 1) from actor a;") { ExpectedResult = new [] { "actor_id" } },

        // new("SELECT (SELECT actor.actor_id FROM city AS a LIMIT 1) FROM actor; -- This works") { ExpectedResult = new [] { "actor_id" } },
        // new("SELECT (SELECT actor_id), (SELECT city_id) FROM actor, city; -- This works") { ExpectedResult = new [] { "actor_id", "city_id" } },
        // new("SELECT (SELECT actor_id from city limit 1) from actor; -- This works") { ExpectedResult = new [] { "actor_id" } },
        // new("SELECT (SELECT actor_id from actor limit 1) from actor_info; -- This works") { ExpectedResult = new [] { "actor_id" } },
    };

    private static readonly TestCaseData[] PostgresSpecificTestDatas =
    {
        new("SELECT a FROM actor a"),
        new("SELECT a.count FROM actor a"),

        new("SELECT release_year, rental_duration, GROUPING(release_year, rental_duration), avg(rental_rate) FROM film GROUP BY ROLLUP(release_year, rental_duration);"),
        new("SELECT rating, rental_duration, avg(rental_rate) FROM film GROUP BY DISTINCT ROLLUP (rating, rental_duration), ROLLUP ((rating));"),
    };

    private static readonly TestCaseData[] LiteralTestDatas =
    {
        // Numeric
        new("SELECT 1, .2, 0.3, 0.4E+5;"),
        new("SELECT 1 + 2, 3 +/*cmt*/ 4;"),
        new("SELECT 42, 3.5 ,4. ,.001 ,5e2 ,1.925e-3;"),

        // String
        new("SELECT 'Test';"),
        new("SELECT '❤️'; -- In PG, this just works"),

        // C-stype Escape
        new("SELECT E'Hello,\tWorld!';"),
        new("SELECT E'\x12\x34\x56\x78';"),

        // Dollar Escape
        new("SELECT $$This is \"amazin'\"!$$;"),
        new("SELECT $function$ BEGIN RETURN ($1 ~ $q$[\t\r\n\v\\]$q$); END; $function$;"),

        // National Character
        new("SELECT N'National';"), // This works, but behave as a normal character type. Column name should be 'bpchar', since it auto-casts value into a character type.
        new("SELECT pg_typeof(N'NATIONAL');"), // This should be a character type

        // Internal
        new("SELECT '가'::\"char\";"), // Result: \\352
        new("SELECT '가'::char;"),
        new("SELECT 'hello, world'::name;"),

        // Bit String
        new("SELECT B'0101';"),
        new("SELECT X'1FF';"),

        // Monetary
        new("SELECT '12.34'::money;"),

        // Binary
        new("SELECT '\\x12AB'::bytea;"),
        new("SELECT 'abc \\153\\154\\215 \\052\\251\\124'::bytea;"),
        new("SELECT 'Hello, world!'::bytea;"), // Column name is bytea (text)

        // Type Casting
        new("SELECT double precision '4';"),
        new("SELECT '4'::double precision;"),
        new("SELECT CAST('4' AS double precision);"),
        new("SELECT float8(4);"),

        new("SELECT timestamp '2023-02-15 03:38:24.074721';"),
        new("SELECT '2023-02-15 03:38:24.074721'::timestamp;"),
        new("SELECT CAST('2023-02-15 03:38:24.074721' AS timestamp);"),
        new("SELECT \"timestamp\"('2023-02-15 03:38:24.074721');"),

        new("SELECT '가'::bpchar;"),

        new("SELECT 'now'::timestamp - '2023-02-15 03:38:24.074721'::timestamp;"),
        new("SELECT '0 years 0 mons 0 days 3 hours 48 mins 38.617811 secs'::interval;"),
        new("SELECT pg_typeof(now() at time zone 'zulu');"),

        new("SELECT now()::timestamptz;"),

        // SQL String Operators
        new("SELECT 'FOO' || 'BAR';"),
        new("SELECT 'FOO' || 123::money;"),
        new("SELECT 'alphabet' ^@ 'alph';"),

        new("SELECT 'thomas' ~ 't.*ma';"),
        new("SELECT 'thomas' ~* 'T.*ma';"),
        new("SELECT 'thomas' !~ 't.*max';"),
        new("SELECT 'thomas' !~* 'T.*ma';"),

        // SQL String Predicates
        new("SELECT U&'\0061\0308bc' IS NFD NORMALIZED;"),

        // SQL String Functions
        new("SELECT overlay('foo bar' PLACING 'baz' FROM 1);"),
        new("SELECT overlay('foo_bar' PLACING 'baz' FROM 3 FOR 0 );"),

        new("SELECT position ( 'yee' IN 'yaayeeyee' );"),

        new("SELECT substring('Thomas' from 2 for 3);"),
        new("SELECT substring('Thomas' from 3);"),
        new("SELECT substring('Thomas' for 2);"),
        new("SELECT substring('Thomas' from '...$');"),

        new("SELECT substring('Thomas' similar '%#\"o_a#\"_' escape '#');"), // since SQL:2003
        new("SELECT substring('Thomas' FROM '%#\"o_a#\"_' FOR '#');"), // only in SQL:1999 and should be considered obsolete

        new("SELECT trim(both 'xyz' from 'yxTomxx');"),
        new("SELECT trim(both from 'yxTomxx', 'xyz');"), // non-standard format
        new("SELECT trim(leading 'xyz' from 'yxTomxx');"),

        new("SELECT yeet as result FROM regexp_matches('foobarbequebaz', 'ba.', 'g') as yeet;"),
        new("SELECT regexp_split_to_table('hello world', '\\s+') || ' wut';"),
        new("SELECT string_to_table('xx~^~yy~^~zz', '~^~', 'yy') as nice;"),

        // SQL Binary Functions
        new("SELECT overlay(B'01010101010101010' placing B'11111' from 2 for 3);"),
        new("SELECT position(B'010' in B'000001101011');"),
        new("SELECT substring(B'110010111111' from 3 for 2);"),

        new("SELECT 44::bit(10);"),
        new("SELECT 44::bit(3);"),
        new("SELECT cast(-44 as bit(12));"),

        new("SELECT '1110'::bit(4)::integer;"),

        // SQL Datetime Functions
        new("SELECT to_char(timestamp '2002-04-20 17:31:12.66', 'HH12:MI:SS');"),
        new("SELECT extract(hour from timestamp '2001-02-16 20:38:40');"),
        new("SELECT extract(month from interval '2 years 3 months');"),

        new("SELECT (DATE '2001-02-16', DATE '2001-12-21') OVERLAPS (DATE '2001-10-30', DATE '2002-10-30');"),
        new("SELECT timestamp with time zone '2005-04-02 12:00:00-07' + interval '1 day';"),
        new("SELECT timestamp with time zone '2005-04-02 12:00:00-07' + interval '24 hours';"),

        new("SELECT date_part('day', TIMESTAMP '2001-02-16 20:38:40');"),

        new("SELECT timestamp '2001-02-16 20:38:40' at time zone 'America/Denver';"),
        new("SELECT timestamp with time zone '2001-02-16 20:38:40-05' at time zone 'America/Denver';"),
        new("SELECT time with time zone '05:34:17-05' at time zone 'UTC';"),

        // Geometric Operators
        new("SELECT path '((0,0),(1,0),(1,1))' * point '(3.0,0)';"),
        new("SELECT path '((0,0),(1,0),(1,1))' * point(cosd(45), sind(45));"),
        new("SELECT path '((0,0),(1,0),(1,1))' / point '(2.0,0)';"),
        new("SELECT path '((0,0),(1,0),(1,1))' / point(cosd(45), sind(45));"),
        new("SELECT @-@ path '[(0,0),(1,0),(1,1)]';"),
        new("SELECT @@ box '(2,2),(0,0)';"),
        new("SELECT # path '((1,0),(0,1),(-1,0))';"),
        new("SELECT lseg '[(0,0),(1,1)]' # lseg '[(1,0),(0,1)]';"),
        new("SELECT box '(2,2),(-1,-1)' # box '(1,1),(-2,-2)';"),
        new("SELECT point '(0,0)' ## lseg '[(2,0),(0,2)]';"),
        new("SELECT circle '<(0,0),1>' <-> circle '<(5,0),1>';"),
        new("SELECT circle '<(0,0),2>' @> point '(1,1)';"),
        new("SELECT point '(1,1)' <@ circle '<(0,0),2>';"),
        new("SELECT box '(1,1),(0,0)' &< box '(2,2),(0,0)';"),
        new("SELECT box '(3,3),(0,0)' &> box '(2,2),(0,0)';"),
        new("SELECT box '(3,3),(0,0)' <<| box '(5,5),(3,4)';"),
        new("SELECT box '(5,5),(3,4)' |>> box '(3,3),(0,0)';"),
        new("SELECT box '(1,1),(0,0)' &<| box '(2,2),(0,0)';"),
        new("SELECT box '(3,3),(0,0)' |&> box '(2,2),(0,0)';"),
        new("SELECT box '((1,1),(0,0))' <^ box '((2,2),(1,1))';"),
        new("SELECT box '((2,2),(1,1))' >^ box '((1,1),(0,0))';"),
        new("SELECT lseg '[(-1,0),(1,0)]' ?# box '(2,2),(-2,-2)';"),
        new("SELECT ?- lseg '[(-1,0),(1,0)]';"),
        new("SELECT point '(1,0)' ?- point '(0,0)';"),
        new("SELECT lseg '[(0,0),(0,1)]' ?-| lseg '[(0,0),(1,0)]';"),
        new("SELECT lseg '[(-1,0),(1,0)]' ?|| lseg '[(-1,2),(1,2)]';"),
        new("SELECT polygon '((0,0),(1,1))' ~= polygon '((1,1),(0,0))';"),

        // Network Addresss Operators
        new("SELECT inet '192.168.1.5' << inet '192.168.1/24';"),
        new("SELECT inet '192.168.1/24' <<= inet '192.168.1/24';"),
        new("SELECT inet '192.168.1/24' >> inet '192.168.1.5';"),
        new("SELECT inet '192.168.1/24' >>= inet '192.168.1/24';"),
        new("SELECT trunc(macaddr8 '12:34:56:78:90:ab:cd:ef');"),

        // Text Search Functions
        new("SELECT ts_delete('fat:2,4 cat:3 rat:5A'::tsvector, ARRAY['fat','rat']);"),
        new("SELECT ts_filter('fat:2,4 cat:3b,7c rat:5A'::tsvector, '{a,b}');"),
        new("SELECT tsvector_to_array('fat:2,4 cat:3 rat:5A'::tsvector);"),
        new("SELECT * FROM unnest('cat:3 fat:2,4 rat:5A'::tsvector);"),
        new("SELECT ts_token_type('default');"),
        new("SELECT ts_stat('SELECT fulltext FROM film');"),

        new("SELECT xmlelement(name foo, xmlattributes('xyz' as bar));"),
        new("SELECT xmlelement(name foo, xmlattributes(current_date as bar), 'cont', 'ent');"),

        new("SELECT xmlelement(name foo, xmlattributes('xyz' as bar), xmlelement(name abc), xmlcomment('test'), xmlelement(name xyz));"),

        new("SELECT xmlforest('abc' AS foo, 123 AS bar);"),
        new("SELECT xmlforest(table_name, column_name) FROM information_schema.columns WHERE table_schema = 'pg_catalog';"),

        new("SELECT xmlpi(name php, 'echo \"hello world\";');"),

        new("SELECT xmlroot(xmlparse(document '<?xml version=\"1.1\"?><content>abc</content>'), version '1.0', standalone yes);"),

        new("SELECT xmlparse(document '<?xml version=\"1.1\"?><content>abc</content>') IS DOCUMENT;"),

        new("SELECT xmlexists ( 'content' PASSING xmlparse(document '<?xml version=\"1.1\"?><content>abc</content>'));"),
        new("SELECT xmlexists('//town[text() = ''Toronto'']' PASSING BY VALUE '<towns><town>Toronto</town><town>Ottawa</town></towns>'); "), // The BY REF and BY VALUE clauses are accepted in PostgreSQL, but are ignored. (Only BY VALUE is supported.)

        new("SELECT xpath('/my:a/text()', '<my:a xmlns:my=\"http://example.com\">test</my:a>', ARRAY[ARRAY['my', 'http://example.com']]);"),

        new("WITH xmldata(data) AS (VALUES ('<example xmlns=\"http://example.com/myns\" xmlns:B=\"http://example.com/b\"> <item foo=\"1\" B:bar=\"2\"/> <item foo=\"3\" B:bar=\"4\"/> <item foo=\"4\" B:bar=\"5\"/> </example>'::xml)) SELECT xmltable.* FROM XMLTABLE(XMLNAMESPACES('http://example.com/myns' AS x, 'http://example.com/b' AS \"B\"), '/x:example/x:item' PASSING (SELECT data FROM xmldata) COLUMNS foo int PATH '@foo', bar int PATH '@B:bar');"),

        // JSON Operators
        new("SELECT '[{\"a\":\"foo\"},{\"b\":\"bar\"},{\"c\":\"baz\"}]'::json -> 2;"),
        new("SELECT '{\"a\": {\"b\":\"foo\"}}'::json -> 'a';"),
        new("SELECT '[1,2,3]'::json ->> 2;"),
        new("SELECT '{\"a\":1,\"b\":2}'::json ->> 'b';"),
        new("SELECT '{\"a\": {\"b\": [\"foo\",\"bar\"]}}'::json #> '{a,b,1}';"),
        new("SELECT '{\"a\": {\"b\": [\"foo\",\"bar\"]}}'::json #>> '{a,b,1}';"),

        // jsonb Operators
        new("SELECT '{\"a\":1, \"b\":2}'::jsonb @> '{\"b\":2}'::jsonb;"),
        new("SELECT '{\"b\":2}'::jsonb <@ '{\"a\":1, \"b\":2}'::jsonb;"),
        new("SELECT '{\"a\":1, \"b\":2}'::jsonb ? 'b';"),
        new("SELECT '{\"a\":1, \"b\":2, \"c\":3}'::jsonb ?| array['b', 'd'];"),
        new("SELECT '[\"a\", \"b\", \"c\"]'::jsonb ?& array['a', 'b'];"),
        new("SELECT '{\"a\": \"b\"}'::jsonb || '42'::jsonb;"),
        new("SELECT '[1, 2]'::jsonb || jsonb_build_array('[3, 4]'::jsonb);"),
        new("SELECT '{\"a\": \"b\", \"c\": \"d\"}'::jsonb - 'a';"),
        new("SELECT '{\"a\": \"b\", \"c\": \"d\"}'::jsonb - '{a,c}'::text[];"),
        new("SELECT '[\"a\", {\"b\":1}]'::jsonb #- '{1,b}';"),
        new("SELECT '{\"a\":[1,2,3,4,5]}'::jsonb @? '$.a[*] ? (@ > 2)';"),
        new("SELECT '{\"a\":[1,2,3,4,5]}'::jsonb @@ '$.a[*] > 2';"),

        // JSON Functions
        new("SELECT * FROM json_array_elements('[1,true, [2,false]]');"),
        new("SELECT json_each('{\"a\":\"foo\", \"b\":\"bar\"}');"),
        new("SELECT * FROM json_each('{\"a\":\"foo\", \"b\":\"bar\"}');"),
        new("SELECT * FROM json_each_text('{\"a\":\"foo\", \"b\":\"bar\"}');"),

        new("SELECT json_extract_path('{\"f2\":{\"f3\":1},\"f4\":{\"f5\":99,\"f6\":\"foo\"}}', 'f4', 'f6');"),
        new("SELECT * FROM json_to_recordset('[{\"a\":1,\"b\":\"foo\"}, {\"a\":\"2\",\"c\":\"bar\"}]') as x(a int, b text, c text, d text);"),
        new("SELECT jsonb_path_exists('{\"a\":[1,2,3,4,5]}', '$.a[*] ? (@ >= $min && @ <= $max)', '{\"min\":2, \"max\":4}');"),
        new("SELECT * FROM jsonb_path_query('{\"a\":[1,2,3,4,5]}', '$.a[*] ? (@ >= $min && @ <= $max)', '{\"min\":2, \"max\":4}');"),
        new("SELECT jsonb_path_exists_tz('[\"2015-08-01 12:00:00-05\"]', '$[*] ? (@.datetime() < \"2015-08-02\".datetime())');"),

        // JSON Path Functions
        new("SELECT jsonb_path_query('[\"2015-8-1\", \"2015-08-12\"]', '$[*] ? (@.datetime() < \"2015-08-2\".datetime())') ;"),
        new("SELECT jsonb_path_query_array('{\"x\": \"20\", \"y\": 32}', '$.keyvalue()');"),
        new("SELECT jsonb_path_query_array('[\"abc\", \"abd\", \"aBdC\", \"abdacb\", \"babc\"]', '$[*] ? (@ like_regex \"^ab.*c\" flag \"i\")');"),
        new("SELECT jsonb_path_query('{\"x\": [1, 2], \"y\": [2, 4]}', 'strict $.* ? (exists (@ ? (@[*] > 2)))');"),

        // Sequence Manipulation Functions
        new("SELECT lastval();"),

        // Conditional Expressions
        new("SELECT actor_id, CASE actor_id % 3 WHEN 0 THEN 'three' WHEN 1 THEN 'one' ELSE 'two' END FROM actor;"),

        // Array Operators & Functions
        new("SELECT ARRAY[1,4,3] @> ARRAY[3,1,3];"),
        new("SELECT ARRAY[2,2,7] <@ ARRAY[1,7,4,2,6];"),
        new("SELECT ARRAY[1,4,3] && ARRAY[2,1];"),

        // Range and Multirange Operators
        new("SELECT '{[1.1,2.2)}'::nummultirange -|- '{[2.2,3.3)}'::nummultirange;"),

        // Subscripts
        new("SELECT special_features[1] FROM film;"),
        new("SELECT special_features[:2] FROM film;"),
        new("SELECT special_features[2:3] FROM film;"),
    };
}
