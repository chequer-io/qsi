-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/Comments.html
SELECT /*+ INDEX_DESC(e emp_name_ix) */ *
  FROM employees e;