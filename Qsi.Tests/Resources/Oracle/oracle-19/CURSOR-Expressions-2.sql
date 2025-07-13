-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/CURSOR-Expressions.html
SELECT e1.last_name FROM employees e1
   WHERE f(
   CURSOR(SELECT e2.hire_date FROM employees e2
   WHERE e1.employee_id = e2.manager_id),
   e1.hire_date) = 1
   ORDER BY last_name;