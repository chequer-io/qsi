-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/SELECT.html
INSERT INTO (SELECT department_id, department_name, location_id
   FROM departments WHERE location_id < 2000)
   VALUES (9999, 'Entertainment', 2500);