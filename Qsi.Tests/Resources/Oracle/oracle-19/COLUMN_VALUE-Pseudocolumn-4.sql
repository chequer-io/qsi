-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/COLUMN_VALUE-Pseudocolumn.html
CREATE TABLE my_customers (
    cust_id       NUMBER,
    name          VARCHAR2(25),
    phone_numbers phone_list,
    credit_limit  NUMBER)
  NESTED TABLE phone_numbers STORE AS outer_ntab
  (NESTED TABLE COLUMN_VALUE STORE AS inner_ntab);