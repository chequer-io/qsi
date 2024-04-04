-- https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/CREATE-INDEX.html
CREATE TYPE rectangle AS OBJECT  
( length   NUMBER, 
  width    NUMBER, 
  MEMBER FUNCTION area RETURN NUMBER DETERMINISTIC 
); 
 
CREATE OR REPLACE TYPE BODY rectangle AS 
  MEMBER FUNCTION area RETURN NUMBER IS 
  BEGIN 
   RETURN (length*width); 
  END; 
END;