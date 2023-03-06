using System;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Data.Object.Function;
using Qsi.Engines;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (tableName)
            {
                case "actor":
                    var actor = CreateTable("postgres", "public", "actor");
                    AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");
                    return actor;

                case "actor_view":
                    var actorView = CreateTable("postgres", "public", "actor_view");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "actor_id", "first_name", "last_name", "last_update");
                    return actorView;

                case "actor_mat_view":
                    var actorMatView = CreateTable("postgres", "public", "actor_mat_view");
                    actorMatView.Type = QsiTableType.View;
                    AddColumns(actorMatView, "actor_id", "first_name", "last_name", "last_update");
                    return actorMatView;

                case "address":
                    var address = CreateTable("postgres", "public", "address");
                    AddColumns(address, "address_id", "address", "address2", "district", "city_id", "postal_code", "phone", "location", "last_update");
                    return address;

                case "city":
                    var city = CreateTable("postgres", "public", "city");
                    AddColumns(city, "city_id", "city", "country_id", "last_update", "test");
                    return city;

                case "test 1":
                    var test1 = CreateTable("postgres", "public", "\"test 1\"");
                    AddColumns(test1, "`c 1`", "`c 2`");
                    return test1;

                case "cs_memo":
                    var csMemo = CreateTable("postgres", "public", "cs_memo");
                    AddColumns(csMemo, "id", "memo");
                    return csMemo;

                case "pg_database":
                    var pgDatabase = CreateTable("postgres", "pg_catalog", "pg_database");

                    AddInvisibleColumns(
                        pgDatabase,
                        "tableoid",
                        "cmax",
                        "xmax",
                        "cmin",
                        "xmin",
                        "ctid"
                    );

                    AddColumns(
                        pgDatabase,
                        "oid",
                        "datname",
                        "datdba",
                        "encoding",
                        "datcollate",
                        "datctype",
                        "datistemplate",
                        "datallowconn",
                        "datconnlimit",
                        "datlastsysoid",
                        "datfrozenxid",
                        "datminmxid",
                        "dattablespace",
                        "datacl"
                    );

                    return pgDatabase;

                case "pg_authid":
                    var pgAuthid = CreateTable("postgres", "pg_catalog", "pg_authid");

                    AddColumns(
                        pgAuthid,
                        "oid",
                        "rolname",
                        "rolsuper",
                        "rolinherit",
                        "rolcreaterole",
                        "rolcreatedb",
                        "rolcanlogin",
                        "rolreplication",
                        "rolbypassrls",
                        "rolconnlimit",
                        "rolpassword",
                        "rolvaliduntil"
                    );

                    return pgAuthid;

                case "pg_stat_activity":

                    var pgStatActivity = CreateTable("postgres", "pg_catalog", "pg_stat_activity");
                    pgStatActivity.Type = QsiTableType.View;

                    AddColumns(pgStatActivity,
                        "datid",
                        "datname",
                        "pid",
                        "usesysid",
                        "usename",
                        "application_name",
                        "client_addr",
                        "client_hostname",
                        "client_port",
                        "backend_start",
                        "xact_start",
                        "query_start",
                        "state_change",
                        "wait_event_type",
                        "wait_event",
                        "state",
                        "backend_xid",
                        "backend_xmin",
                        "query",
                        "backend_type"
                    );

                    return pgStatActivity;

                case "pg_stat_database":

                    var pgStatDatabase = CreateTable("postgres", "pg_catalog", "pg_stat_database");
                    pgStatDatabase.Type = QsiTableType.View;

                    AddColumns(pgStatDatabase,
                        "datid",
                        "datname",
                        "numbackends",
                        "xact_commit",
                        "xact_rollback",
                        "blks_read",
                        "blks_hit",
                        "tup_returned",
                        "tup_fetched",
                        "tup_inserted",
                        "tup_updated",
                        "tup_deleted",
                        "conflicts",
                        "temp_files",
                        "temp_bytes",
                        "deadlocks",
                        "checksum_failures",
                        "checksum_last_failure",
                        "blk_read_time",
                        "blk_write_time",
                        "stats_reset"
                    );

                    return pgStatDatabase;

                case "pg_roles":

                    var pgRoles = CreateTable("postgres", "pg_catalog", "pg_roles");
                    pgRoles.Type = QsiTableType.View;

                    AddColumns(pgRoles,
                        "rolname",
                        "rolsuper",
                        "rolinherit",
                        "rolcreaterole",
                        "rolcreatedb",
                        "rolcanlogin",
                        "rolreplication",
                        "rolconnlimit",
                        "rolpassword",
                        "rolvaliduntil",
                        "rolbypassrls",
                        "rolconfig",
                        "oid"
                    );

                    return pgRoles;

                case "pg_db_role_setting":
                    var pgDbRoleSetting = CreateTable("postgres", "pg_catalog", "pg_db_role_setting");

                    AddColumns(pgDbRoleSetting, "setdatabase", "setrole", "setconfig");
                    return pgDbRoleSetting;

                case "pg_auth_members":
                    var pgAuthMembers = CreateTable("postgres", "pg_catalog", "pg_auth_members");

                    AddColumns(pgAuthMembers, "roleid", "member", "grantor", "admin_option");
                    return pgAuthMembers;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "actor_view":
                    return new QsiScript("CREATE OR REPLACE VIEW public.actor_view AS SELECT actor.actor_id, actor.first_name, actor.last_name, actor.last_update FROM actor;", QsiScriptType.Create);

                case "actor_mat_view":
                    return new QsiScript("CREATE MATERIALIZED VIEW public.actor_view AS SELECT actor.actor_id, actor.first_name, actor.last_name, actor.last_update FROM actor;", QsiScriptType.Create);

                case "pg_stat_activity":
                    return new QsiScript(@"CREATE OR REPLACE VIEW pg_catalog.pg_stat_activity AS  SELECT s.datid,
    d.datname,
    s.pid,
    s.usesysid,
    u.rolname AS usename,
    s.application_name,
    s.client_addr,
    s.client_hostname,
    s.client_port,
    s.backend_start,
    s.xact_start,
    s.query_start,
    s.state_change,
    s.wait_event_type,
    s.wait_event,
    s.state,
    s.backend_xid,
    s.backend_xmin,
    s.query,
    s.backend_type
   FROM ((pg_stat_get_activity(NULL::integer) s(datid, pid, usesysid, application_name, state, query, wait_event_type, wait_event, xact_start, query_start, backend_start, state_change, client_addr, client_hostname, client_port, backend_xid, backend_xmin, backend_type, ssl, sslversion, sslcipher, sslbits, sslcompression, ssl_client_dn, ssl_client_serial, ssl_issuer_dn, gss_auth, gss_princ, gss_enc)
     LEFT JOIN pg_database d ON ((s.datid = d.oid)))
     LEFT JOIN pg_authid u ON ((s.usesysid = u.oid)));", QsiScriptType.Create);

                case "pg_stat_database":
                    return new QsiScript(@"CREATE OR REPLACE VIEW pg_catalog.pg_stat_database AS  SELECT d.oid AS datid,
    d.datname,
        CASE
            WHEN (d.oid = (0)::oid) THEN 0
            ELSE pg_stat_get_db_numbackends(d.oid)
        END AS numbackends,
    pg_stat_get_db_xact_commit(d.oid) AS xact_commit,
    pg_stat_get_db_xact_rollback(d.oid) AS xact_rollback,
    (pg_stat_get_db_blocks_fetched(d.oid) - pg_stat_get_db_blocks_hit(d.oid)) AS blks_read,
    pg_stat_get_db_blocks_hit(d.oid) AS blks_hit,
    pg_stat_get_db_tuples_returned(d.oid) AS tup_returned,
    pg_stat_get_db_tuples_fetched(d.oid) AS tup_fetched,
    pg_stat_get_db_tuples_inserted(d.oid) AS tup_inserted,
    pg_stat_get_db_tuples_updated(d.oid) AS tup_updated,
    pg_stat_get_db_tuples_deleted(d.oid) AS tup_deleted,
    pg_stat_get_db_conflict_all(d.oid) AS conflicts,
    pg_stat_get_db_temp_files(d.oid) AS temp_files,
    pg_stat_get_db_temp_bytes(d.oid) AS temp_bytes,
    pg_stat_get_db_deadlocks(d.oid) AS deadlocks,
    pg_stat_get_db_checksum_failures(d.oid) AS checksum_failures,
    pg_stat_get_db_checksum_last_failure(d.oid) AS checksum_last_failure,
    pg_stat_get_db_blk_read_time(d.oid) AS blk_read_time,
    pg_stat_get_db_blk_write_time(d.oid) AS blk_write_time,
    pg_stat_get_db_stat_reset_time(d.oid) AS stats_reset
   FROM ( SELECT 0 AS oid,
            NULL::name AS datname
        UNION ALL
         SELECT pg_database.oid,
            pg_database.datname
           FROM pg_database) d;", QsiScriptType.Create);

                case "pg_roles":
                    return new QsiScript(@"CREATE OR REPLACE VIEW pg_catalog.pg_roles AS  SELECT pg_authid.rolname,
    pg_authid.rolsuper,
    pg_authid.rolinherit,
    pg_authid.rolcreaterole,
    pg_authid.rolcreatedb,
    pg_authid.rolcanlogin,
    pg_authid.rolreplication,
    pg_authid.rolconnlimit,
    '********'::text AS rolpassword,
    pg_authid.rolvaliduntil,
    pg_authid.rolbypassrls,
    s.setconfig AS rolconfig,
    pg_authid.oid
   FROM (pg_authid
     LEFT JOIN pg_db_role_setting s ON (((pg_authid.oid = s.setrole) AND (s.setdatabase = (0)::oid))));", QsiScriptType.Create);
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "pg_get_keywords" when type is QsiObjectType.Function:
                {
                    return new QsiFunctionObject(
                        new QsiQualifiedIdentifier(
                            new QsiIdentifier("pg_catalog", false),
                            new QsiIdentifier("pg_get_keywords", false)
                        ),
                        @"CREATE OR REPLACE FUNCTION pg_catalog.pg_get_keywords
                            (
                                OUT word text,
                                OUT catcode ""char"",
                                OUT barelabel boolean,
                                OUT catdesc text,
                                OUT baredesc text
                            )
                            RETURNS SETOF record
                            LANGUAGE internal
                            STABLE PARALLEL SAFE STRICT COST 10 ROWS 500
                            AS $function$pg_get_keywords$function$",
                        5);
                }
            }

            return null;
        }

        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier, ExecuteOptions executeOptions)
        {
            identifier = identifier.Level switch
            {
                1 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("postgres", false),
                    new QsiIdentifier("public", false),
                    identifier[0]
                ),
                2 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("postgres", false),
                    identifier[0],
                    identifier[1]
                ),
                _ => identifier
            };

            if (identifier.Level != 3)
                throw new InvalidOperationException();

            return identifier;
        }
    }
}
