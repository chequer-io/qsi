-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/BITAND.html
SELECT BITAND(
    BIN_TO_NUM(1,1,0),
    BIN_TO_NUM(0,1,1)) "Binary"
  FROM DUAL;