-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/NEW_TIME.html
ALTER SESSION SET NLS_DATE_FORMAT = 'DD-MON-YYYY HH24:MI:SS';

SELECT NEW_TIME(TO_DATE('11-10-09 01:23:45', 'MM-DD-YY HH24:MI:SS'), 'AST', 'PST')
         "New Date and Time"
  FROM DUAL;