-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/XMLAGG.html
SELECT XMLELEMENT("Department",
      XMLAGG(XMLELEMENT("Employee", e.job_id||' '||e.last_name)))
   AS "Dept_list"
   FROM employees e
   GROUP BY e.department_id;