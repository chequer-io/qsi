-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/DENSE_RANK.html
SELECT department_id, last_name, salary,
       DENSE_RANK() OVER (PARTITION BY department_id ORDER BY salary) DENSE_RANK
  FROM employees WHERE department_id = 60
  ORDER BY DENSE_RANK, last_name;