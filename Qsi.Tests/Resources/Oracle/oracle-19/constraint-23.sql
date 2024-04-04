-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/constraint.html
CREATE TABLE employees_obj
   ( e_name   VARCHAR2(100),
     e_number NUMBER,
     e_dept   REF department_typ REFERENCES departments_obj_t);