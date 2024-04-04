-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/storage_clause.html
CREATE TABLE divisions 
    (div_no     NUMBER(2), 
     div_name   VARCHAR2(14), 
     location   VARCHAR2(13) ) 
     STORAGE  ( INITIAL 8M MAXSIZE 1G );