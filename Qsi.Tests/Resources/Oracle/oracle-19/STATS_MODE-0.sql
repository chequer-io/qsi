-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/STATS_MODE.html
SELECT x FROM (SELECT x, COUNT(x) AS cnt1
   FROM t GROUP BY x)
   WHERE cnt1 =
      (SELECT MAX(cnt2) FROM (SELECT COUNT(x) AS cnt2 FROM t GROUP BY x));