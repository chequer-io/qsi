-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/con_id_to_dbid.html
SELECT CON_ID, NAME, DBID FROM V$CONTAINERS;