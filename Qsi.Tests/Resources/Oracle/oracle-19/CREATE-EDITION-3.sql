-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/CREATE-EDITION.html
ALTER SESSION SET EDITION = TEST_ED;
Session altered.

CREATE OR REPLACE EDITIONING VIEW e_view AS
  SELECT last_name, first_name, email, salary FROM employees;

View created.