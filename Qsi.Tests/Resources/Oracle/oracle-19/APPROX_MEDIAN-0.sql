-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/APPROX_MEDIAN.html
SELECT department_id "Department",
       APPROX_MEDIAN(salary DETERMINISTIC) "Median Salary"
  FROM employees
  GROUP BY department_id
  ORDER BY department_id;