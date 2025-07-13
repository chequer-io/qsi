-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/FEATURE_VALUE.html
SELECT *
  FROM (SELECT cust_id, FEATURE_VALUE(nmf_sh_sample, 3 USING *) match_quality
          FROM nmf_sh_sample_apply_prepared
          ORDER BY match_quality DESC)
  WHERE ROWNUM < 11;