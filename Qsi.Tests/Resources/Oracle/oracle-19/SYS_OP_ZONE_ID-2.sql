-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/SYS_OP_ZONE_ID.html
CREATE MATERIALIZED ZONEMAP sales_zmap
SCALE 8
AS
  SELECT SYS_OP_ZONE_ID(rowid,12), MIN(time_id), MAX(time_id)
  FROM sales
  GROUP BY SYS_OP_ZONE_ID(rowid,12);