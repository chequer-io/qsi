-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/NLSSORT.html
SELECT *
  FROM test
  WHERE name > 'Gaberd'
  ORDER BY name;

no rows selected

SELECT *
  FROM test
  WHERE NLSSORT(name, 'NLS_SORT = XDanish') > 
        NLSSORT('Gaberd', 'NLS_SORT = XDanish')
  ORDER BY name;