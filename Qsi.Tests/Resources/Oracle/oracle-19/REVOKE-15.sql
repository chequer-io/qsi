-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/REVOKE.html
REVOKE REFERENCES 
    ON hr.employees 
    FROM oe 
    CASCADE CONSTRAINTS;