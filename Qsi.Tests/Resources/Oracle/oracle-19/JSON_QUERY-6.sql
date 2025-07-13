-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/JSON_QUERY.html
SELECT JSON_QUERY('[0,1,2,3,4,5,6,7,8]', '$[0, 3 to 5, 7]' WITH WRAPPER) AS value
  FROM DUAL;