-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/FLASHBACK-TABLE.html
SELECT object_name, droptime FROM user_recyclebin 
   WHERE original_name = 'PRINT_MEDIA';