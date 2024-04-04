-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/TO_LOB.html
CREATE TABLE new_table (col1, col2, ... lob_col CLOB);
INSERT INTO new_table (select o.col1, o.col2, ... TO_LOB(o.old_long_col)
   FROM old_table o;