-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/CASE-Expressions.html
SELECT cust_last_name,
   CASE credit_limit WHEN 100 THEN 'Low'
   WHEN 5000 THEN 'High'
   ELSE 'Medium' END AS credit
   FROM customers
   ORDER BY cust_last_name, credit;