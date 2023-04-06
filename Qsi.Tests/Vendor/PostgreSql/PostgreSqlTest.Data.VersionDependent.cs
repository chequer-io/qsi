using NUnit.Framework;

namespace Qsi.Tests.PostgreSql;

public partial class PostgreSqlTest
{
    private static readonly TestCaseData[] _versionDependentSystemTestCaseDatas =
    {
        // Session Information Functions
        new("select pg_jit_available();", 11),
        
        // `aclitem` Operators & Functions
        new("select 'calvin=r*w/hobbes'::aclitem = 'calvin=r*w*/hobbes'::aclitem", 12),
        new("select '{calvin=r*w/hobbes,hobbes=r*w*/postgres}'::aclitem[] @> 'calvin=r*/hobbes'::aclitem", 12),
        new("select '{calvin=r*w/hobbes,hobbes=r*w*/postgres}'::aclitem[] ~ 'calvin=r*/hobbes'::aclitem", 12),
        new("select acldefault('r', r.oid) from pg_roles r where r.rolname = CURRENT_ROLE;", 12),
        new("select aclexplode('{postgres=arwdDxt/postgres}'::aclitem[]);", 12),
        new("select makeaclitem(r.oid, r.oid, 'INSERT', true) from pg_roles r where r.rolname = CURRENT_ROLE;", 12),
        
        // System Catalog Information Functions
        new("select pg_char_to_encoding('UTF8');", 15),
        new("select pg_encoding_to_char(6)", 15),
        new("select pg_get_catalog_foreign_keys()", 14),
        new("select pg_settings_get_flags('EXPLAIN')", 15),
        new("select to_regcollation('pg_catalog.\"POSIX\"');", 13),
        new("select pg_indexam_has_property(a.oid, 'can_include') from pg_am a;", 11),
        
        // Transaction ID and Snapshot Information Functions
        new("select pg_current_xact_id();", 13),
        new("select pg_xact_status('1'::xid8);", 13),
        new("select pg_current_snapshot();", 13),
        new("select pg_snapshot_xip('1220:1220:'::pg_snapshot);", 13),
        new("select pg_snapshot_xmax('1220:1220:'::pg_snapshot);", 13),
        new("select pg_snapshot_xmin('1220:1220:'::pg_snapshot);", 13),
        new("select pg_visible_in_snapshot('1'::xid8, '1220:1220:'::pg_snapshot);", 13),
        
        // Committed Transaction Information Functions
        new("select pg_xact_commit_timestamp_origin('1'::xid);", 14),
    };
}
