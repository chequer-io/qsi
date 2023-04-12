using NUnit.Framework;

namespace Qsi.Tests.PostgreSql;

public partial class PostgreSqlTest
{
    private static readonly TestCaseData[] _versionDependentSystemTestCaseDatas =
    {
        // Session Information Functions
        new("select pg_jit_available();", 11, null),
        
        // `aclitem` Operators & Functions
        new("select 'calvin=r*w/hobbes'::aclitem = 'calvin=r*w*/hobbes'::aclitem", 12, null),
        new("select '{calvin=r*w/hobbes,hobbes=r*w*/postgres}'::aclitem[] @> 'calvin=r*/hobbes'::aclitem", 12, null),
        new("select '{calvin=r*w/hobbes,hobbes=r*w*/postgres}'::aclitem[] ~ 'calvin=r*/hobbes'::aclitem", 12, null),
        new("select acldefault('r', r.oid) from pg_roles r where r.rolname = CURRENT_ROLE;", 12, null),
        new("select aclexplode('{postgres=arwdDxt/postgres}'::aclitem[]);", 12, null),
        new("select makeaclitem(r.oid, r.oid, 'INSERT', true) from pg_roles r where r.rolname = CURRENT_ROLE;", 12, null),
        
        // System Catalog Information Functions
        new("select pg_char_to_encoding('UTF8');", 15, null),
        new("select pg_encoding_to_char(6)", 15, null),
        new("select pg_get_catalog_foreign_keys()", 14, null),
        new("select pg_settings_get_flags('EXPLAIN')", 15, null),
        new("select to_regcollation('pg_catalog.\"POSIX\"');", 13, null),
        new("select pg_indexam_has_property(a.oid, 'can_include') from pg_am a;", 11, null),
        
        // Transaction ID and Snapshot Information Functions
        new("select pg_current_xact_id();", 13, null),
        new("select pg_xact_status('1'::xid8);", 13, null),
        new("select pg_current_snapshot();", 13, null),
        new("select pg_snapshot_xip('1220:1220:'::pg_snapshot);", 13, null),
        new("select pg_snapshot_xmax('1220:1220:'::pg_snapshot);", 13, null),
        new("select pg_snapshot_xmin('1220:1220:'::pg_snapshot);", 13, null),
        new("select pg_visible_in_snapshot('1'::xid8, '1220:1220:'::pg_snapshot);", 13, null),
        
        // Committed Transaction Information Functions
        new("select pg_xact_commit_timestamp_origin('1'::xid);", 14, null),
    };

    private static readonly TestCaseData[] _versionDependentTestCaseDatas =
    {
        new("CREATE OR REPLACE FUNCTION increment(i integer) RETURNS integer AS $$\n        BEGIN\n            RETURN i + 1;\n        END;\n$$ LANGUAGE plpgsql WITH isStrict;", null, 10),
        new("ALTER FOREIGN TABLE distributors SET WITH OIDS;", null, 11),
        new("ALTER TABLE actor SET WITH OIDS;", null, 11),
        new("COPY country TO STDOUT (DELIMITER '|') WITH OIDS;", null, 11),
        new("CREATE TABLE films ( code char(5) CONSTRAINT firstkey PRIMARY KEY, title varchar(40) NOT NULL, did integer NOT NULL, date_prod date, kind varchar(10), len interval hour to minute) WITH OIDS;", null, 11),
        new("CREATE TABLE films_recent WITH OIDS AS SELECT * FROM films WHERE date_prod >= '2002-01-01';", null, 11),
        new("CREATE EXTENSION hstore SCHEMA addons FROM unpackaged;", null, 12),
        new("ALTER OPERATOR && (_int4, NONE) SET (RESTRICT = _int_contsel, JOIN = _int_contjoinsel);", null, 13),
        new("DROP OPERATOR && (_int4, NONE);", null, 13),
        new("VACUUM (INDEX_CLEANUP) something;", null, 13)
    };
}
