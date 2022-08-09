using NUnit.Framework;

namespace Qsi.Tests.Oracle;

public partial class OracleScriptParserTest
{
    private static readonly TestCaseData[] Parse_TestDatas =
    {
        // SELECT
        new("SELECT 1 FROM DUAL;;") { ExpectedResult = new[] { "SELECT 1 FROM DUAL" } },
        // CREATE TABLE
        new("CREATE TABLE a(id NUMBER);") { ExpectedResult = new[] { "CREATE TABLE a(id NUMBER)" } },

        // BLOCK (BEGIN ~ END)
        new(@"
BEGIN
    DBMS_OUTPUT.put_line ('Hello World!');
END;

BEGIN
    DBMS_OUTPUT.put_line ('Hello World!');
END;

BEGIN
    DBMS_OUTPUT.put_line ('Hello World!');
END;")
        {
            ExpectedResult = new[]
            {
                @"BEGIN
    DBMS_OUTPUT.put_line ('Hello World!');
END;",
                @"BEGIN
    DBMS_OUTPUT.put_line ('Hello World!');
END;",
                @"BEGIN
    DBMS_OUTPUT.put_line ('Hello World!');
END;"
            }
        },

        // BLOCK WITH CASE (CASE ~ END CASE)
        new(@"DECLARE
  c_grade CHAR( 1 );
  c_rank  VARCHAR2( 20 );
BEGIN
  c_grade := 'B';
  CASE c_grade
  WHEN 'A' THEN
    c_rank := 'Excellent' ;
  WHEN 'B' THEN
    c_rank := 'Very Good' ;
  WHEN 'C' THEN
    c_rank := 'Good' ;
  WHEN 'D' THEN
    c_rank := 'Fair' ;
  WHEN 'F' THEN
    c_rank := 'Poor' ;
  ELSE
    c_rank := 'No such grade' ;
  END CASE;
  DBMS_OUTPUT.PUT_LINE( c_rank );
END;
DECLARE
  c_grade CHAR( 1 );
  c_rank  VARCHAR2( 20 );
BEGIN
  c_grade := 'B';
  CASE c_grade
  WHEN 'A' THEN
    c_rank := 'Excellent' ;
  WHEN 'B' THEN
    c_rank := 'Very Good' ;
  WHEN 'C' THEN
    c_rank := 'Good' ;
  WHEN 'D' THEN
    c_rank := 'Fair' ;
  WHEN 'F' THEN
    c_rank := 'Poor' ;
  ELSE
    c_rank := 'No such grade' ;
  END CASE;
  DBMS_OUTPUT.PUT_LINE( c_rank );
END;")
        {
            ExpectedResult = new[]
            {
                @"DECLARE
  c_grade CHAR( 1 );
  c_rank  VARCHAR2( 20 );
BEGIN
  c_grade := 'B';
  CASE c_grade
  WHEN 'A' THEN
    c_rank := 'Excellent' ;
  WHEN 'B' THEN
    c_rank := 'Very Good' ;
  WHEN 'C' THEN
    c_rank := 'Good' ;
  WHEN 'D' THEN
    c_rank := 'Fair' ;
  WHEN 'F' THEN
    c_rank := 'Poor' ;
  ELSE
    c_rank := 'No such grade' ;
  END CASE;
  DBMS_OUTPUT.PUT_LINE( c_rank );
END;",
                @"DECLARE
  c_grade CHAR( 1 );
  c_rank  VARCHAR2( 20 );
BEGIN
  c_grade := 'B';
  CASE c_grade
  WHEN 'A' THEN
    c_rank := 'Excellent' ;
  WHEN 'B' THEN
    c_rank := 'Very Good' ;
  WHEN 'C' THEN
    c_rank := 'Good' ;
  WHEN 'D' THEN
    c_rank := 'Fair' ;
  WHEN 'F' THEN
    c_rank := 'Poor' ;
  ELSE
    c_rank := 'No such grade' ;
  END CASE;
  DBMS_OUTPUT.PUT_LINE( c_rank );
END;"
            }
        },

        // BLOCK WITH NULL (NULL;)
        new(@"BEGIN
    NULL;
END;

BEGIN
    NULL;
END;")
        {
            ExpectedResult = new[]
            {
                @"BEGIN
    NULL;
END;",
                @"BEGIN
    NULL;
END;"
            }
        },

        // BLOCK WITH IF (IF ~ END IF)
        new(@"DECLARE
  b_profitable BOOLEAN;
  n_sales      NUMBER;
  n_costs      NUMBER;
BEGIN
  b_profitable := false;   
  IF n_sales > n_costs THEN
    b_profitable := true;
  END IF;
END;
DECLARE
  b_profitable BOOLEAN;
  n_sales      NUMBER;
  n_costs      NUMBER;
BEGIN
  b_profitable := false;   
  IF n_sales > n_costs THEN
    b_profitable := true;
  END IF;
END;")
        {
            ExpectedResult = new[]
            {
                @"DECLARE
  b_profitable BOOLEAN;
  n_sales      NUMBER;
  n_costs      NUMBER;
BEGIN
  b_profitable := false;   
  IF n_sales > n_costs THEN
    b_profitable := true;
  END IF;
END;",
                @"DECLARE
  b_profitable BOOLEAN;
  n_sales      NUMBER;
  n_costs      NUMBER;
BEGIN
  b_profitable := false;   
  IF n_sales > n_costs THEN
    b_profitable := true;
  END IF;
END;"
            }
        },

        // BLOCK WITH LOOP (LOOP ~ END LOOP)
        new(@"DECLARE
NUM1 NUMBER :=1;

BEGIN
    LOOP
    DBMS_OUTPUT.PUT_LINE(NUM1);
    NUM1 := NUM1+1; --NUM = NUM +1
    EXIT WHEN NUM1 >10;
    END LOOP;
END;

DECLARE
NUM1 NUMBER :=1;

BEGIN
    LOOP
    DBMS_OUTPUT.PUT_LINE(NUM1);
    NUM1 := NUM1+1; --NUM = NUM +1
    EXIT WHEN NUM1 >10;
    END LOOP;
END;")
        {
            ExpectedResult = new[]
            {
                @"DECLARE
NUM1 NUMBER :=1;

BEGIN
    LOOP
    DBMS_OUTPUT.PUT_LINE(NUM1);
    NUM1 := NUM1+1; --NUM = NUM +1
    EXIT WHEN NUM1 >10;
    END LOOP;
END;",
                @"DECLARE
NUM1 NUMBER :=1;

BEGIN
    LOOP
    DBMS_OUTPUT.PUT_LINE(NUM1);
    NUM1 := NUM1+1; --NUM = NUM +1
    EXIT WHEN NUM1 >10;
    END LOOP;
END;"
            }
        },

        // BEGIN ~ { BEGIN ~ END } ~ END
        new(@"begin
  -- Make GC_NAB field for Next Action By Dropdown 
  begin 
  if 'VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10, ))'; 
  elsif ('VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')=0) or 
    'VARCHAR2' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10))'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2)'; 
  end if; 
  commit; 
  end; 
  -- Make GC_NABID field for Next Action By Dropdown 
  begin 
  if 'NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER(, ))'; 
  elsif ('NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')=0) or 
    'NUMBER' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER())'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER)'; 
  end if; 
  commit; 
  end;
end;


begin
  -- Make GC_NAB field for Next Action By Dropdown 
  begin 
  if 'VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10, ))'; 
  elsif ('VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')=0) or 
    'VARCHAR2' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10))'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2)'; 
  end if; 
  commit; 
  end; 
  -- Make GC_NABID field for Next Action By Dropdown 
  begin 
  if 'NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER(, ))'; 
  elsif ('NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')=0) or 
    'NUMBER' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER())'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER)'; 
  end if; 
  commit; 
  end;
end;
")
        {
            ExpectedResult = new[]
            {
                @"begin
  -- Make GC_NAB field for Next Action By Dropdown 
  begin 
  if 'VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10, ))'; 
  elsif ('VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')=0) or 
    'VARCHAR2' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10))'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2)'; 
  end if; 
  commit; 
  end; 
  -- Make GC_NABID field for Next Action By Dropdown 
  begin 
  if 'NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER(, ))'; 
  elsif ('NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')=0) or 
    'NUMBER' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER())'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER)'; 
  end if; 
  commit; 
  end;
end;",
                @"begin
  -- Make GC_NAB field for Next Action By Dropdown 
  begin 
  if 'VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10, ))'; 
  elsif ('VARCHAR2' = 'NUMBER' and length('VARCHAR2')>0 and length('')=0) or 
    'VARCHAR2' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2(10))'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NAB VARCHAR2)'; 
  end if; 
  commit; 
  end; 
  -- Make GC_NABID field for Next Action By Dropdown 
  begin 
  if 'NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')>0 then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER(, ))'; 
  elsif ('NUMBER' = 'NUMBER' and length('NUMBER')>0 and length('')=0) or 
    'NUMBER' = 'VARCHAR2' then 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER())'; 
  else 
    execute immediate 'alter table ""SERVICEMAIL6"".""ETD_GUESTCARE"" add(GC_NABID NUMBER)'; 
  end if; 
  commit; 
  end;
end;"
            }
        },

        // BLOCK WITH CLOSE CURSOR (CLOSE <CURSOR>)
        new(@"CREATE OR REPLACE Function FindCourse
   ( name_in IN varchar2 )
   RETURN number
IS
   cnumber number;

   CURSOR c1
   IS
     SELECT course_number
     FROM courses_tbl
     WHERE course_name = name_in;

BEGIN

   OPEN c1;
   FETCH c1 INTO cnumber;

   if c1%notfound then
      cnumber := 9999;
   end if;

   CLOSE c1;

RETURN cnumber;

END;

CREATE OR REPLACE Function FindCourse
   ( name_in IN varchar2 )
   RETURN number
IS
   cnumber number;

   CURSOR c1
   IS
     SELECT course_number
     FROM courses_tbl
     WHERE course_name = name_in;

BEGIN

   OPEN c1;
   FETCH c1 INTO cnumber;

   if c1%notfound then
      cnumber := 9999;
   end if;

   CLOSE c1;

RETURN cnumber;

END;")
        {
            ExpectedResult = new[]
            {
                @"CREATE OR REPLACE Function FindCourse
   ( name_in IN varchar2 )
   RETURN number
IS
   cnumber number;

   CURSOR c1
   IS
     SELECT course_number
     FROM courses_tbl
     WHERE course_name = name_in;

BEGIN

   OPEN c1;
   FETCH c1 INTO cnumber;

   if c1%notfound then
      cnumber := 9999;
   end if;

   CLOSE c1;

RETURN cnumber;

END;",
                @"CREATE OR REPLACE Function FindCourse
   ( name_in IN varchar2 )
   RETURN number
IS
   cnumber number;

   CURSOR c1
   IS
     SELECT course_number
     FROM courses_tbl
     WHERE course_name = name_in;

BEGIN

   OPEN c1;
   FETCH c1 INTO cnumber;

   if c1%notfound then
      cnumber := 9999;
   end if;

   CLOSE c1;

RETURN cnumber;

END;"
            },
        },

        // CREATE [OR REPLACE] PROCEDURE
        new(@"
CREATE OR REPLACE PROCEDURE studentInsert(
    pName pl_student.name%TYPE,
    pKor pl_student.kor%TYPE,
    pEng pl_student.eng%TYPE,
    pMath pl_student.math%TYPE
)
IS
BEGIN
    INSERT INTO pl_student VALUES(
        (SELECT NVL(MAX(hakbun)+1,1) FROM pl_student),
       pName,pKor,pEng,pMath
    );
    COMMIT;
END;
")
        {
            ExpectedResult = new[]
            {
                @"CREATE OR REPLACE PROCEDURE studentInsert(
    pName pl_student.name%TYPE,
    pKor pl_student.kor%TYPE,
    pEng pl_student.eng%TYPE,
    pMath pl_student.math%TYPE
)
IS
BEGIN
    INSERT INTO pl_student VALUES(
        (SELECT NVL(MAX(hakbun)+1,1) FROM pl_student),
       pName,pKor,pEng,pMath
    );
    COMMIT;
END;"
            }
        },

        // CREATE [OR REPLACE] FUNCTION
        new(@"
CREATE OR REPLACE FUNCTION get_complete_address(in_person_id IN NUMBER) 
    RETURN VARCHAR2
    IS person_details VARCHAR2(130);

BEGIN 
    SELECT 'Name-'||person.first_name||' '|| person.last_name||', City-'|| address.city ||', State-'||address.state||', Country-'||address.country||', ZIP Code-'||address.zip_code 
    INTO person_details
    FROM person_info person, person_address_details address
    WHERE person.person_id = in_person_id 
    AND address.person_id = person.person_id;

    RETURN(person_details); 

END get_complete_address;

")
        {
            ExpectedResult = new[]
            {
                @"CREATE OR REPLACE FUNCTION get_complete_address(in_person_id IN NUMBER) 
    RETURN VARCHAR2
    IS person_details VARCHAR2(130);

BEGIN 
    SELECT 'Name-'||person.first_name||' '|| person.last_name||', City-'|| address.city ||', State-'||address.state||', Country-'||address.country||', ZIP Code-'||address.zip_code 
    INTO person_details
    FROM person_info person, person_address_details address
    WHERE person.person_id = in_person_id 
    AND address.person_id = person.person_id;

    RETURN(person_details); 

END get_complete_address;"
            }
        },

        // CREATE [OR REPLACE] PACKAGE
        new(@"
CREATE OR REPLACE PACKAGE emp_mgmt AS 
   FUNCTION hire (last_name VARCHAR2, job_id VARCHAR2, 
      manager_id NUMBER, salary NUMBER, 
      commission_pct NUMBER, department_id NUMBER) 
      RETURN NUMBER; 
   FUNCTION create_dept(department_id NUMBER, location_id NUMBER) 
      RETURN NUMBER; 
   PROCEDURE remove_emp(employee_id NUMBER); 
   PROCEDURE remove_dept(department_id NUMBER); 
   PROCEDURE increase_sal(employee_id NUMBER, salary_incr NUMBER); 
   PROCEDURE increase_comm(employee_id NUMBER, comm_incr NUMBER); 
   no_comm EXCEPTION; 
   no_sal EXCEPTION; 
END emp_mgmt;  ")
        {
            ExpectedResult = new[]
            {
                @"CREATE OR REPLACE PACKAGE emp_mgmt AS 
   FUNCTION hire (last_name VARCHAR2, job_id VARCHAR2, 
      manager_id NUMBER, salary NUMBER, 
      commission_pct NUMBER, department_id NUMBER) 
      RETURN NUMBER; 
   FUNCTION create_dept(department_id NUMBER, location_id NUMBER) 
      RETURN NUMBER; 
   PROCEDURE remove_emp(employee_id NUMBER); 
   PROCEDURE remove_dept(department_id NUMBER); 
   PROCEDURE increase_sal(employee_id NUMBER, salary_incr NUMBER); 
   PROCEDURE increase_comm(employee_id NUMBER, comm_incr NUMBER); 
   no_comm EXCEPTION; 
   no_sal EXCEPTION; 
END emp_mgmt;"
            }
        },

        // CREATE [OR REPLACE] TRIGGER
        new(@"
CREATE OR REPLACE TRIGGER customers_audit_trg
    AFTER 
    UPDATE OR DELETE 
    ON customers
    FOR EACH ROW    
DECLARE
   l_transaction VARCHAR2(10);
BEGIN
   -- determine the transaction type
   l_transaction := CASE  
         WHEN UPDATING THEN 'UPDATE'
         WHEN DELETING THEN 'DELETE'
   END;

   -- insert a row into the audit table   
   INSERT INTO audits (table_name, transaction_name, by_user, transaction_date)
   VALUES('CUSTOMERS', l_transaction, USER, SYSDATE);
END;
")
        {
            ExpectedResult = new[]
            {
                @"CREATE OR REPLACE TRIGGER customers_audit_trg
    AFTER 
    UPDATE OR DELETE 
    ON customers
    FOR EACH ROW    
DECLARE
   l_transaction VARCHAR2(10);
BEGIN
   -- determine the transaction type
   l_transaction := CASE  
         WHEN UPDATING THEN 'UPDATE'
         WHEN DELETING THEN 'DELETE'
   END;

   -- insert a row into the audit table   
   INSERT INTO audits (table_name, transaction_name, by_user, transaction_date)
   VALUES('CUSTOMERS', l_transaction, USER, SYSDATE);
END;"
            }
        },

        // CREATE [OR REPLACE] TYPE
        new(@"CREATE TYPE customer_typ_demo AS OBJECT
    ( customer_id        NUMBER(6)
    , cust_first_name    VARCHAR2(20)
    , cust_last_name     VARCHAR2(20)
    , cust_address       CUST_ADDRESS_TYP
    , phone_numbers      PHONE_LIST_TYP
    , nls_language       VARCHAR2(3)
    , nls_territory      VARCHAR2(30)
    , credit_limit       NUMBER(9,2)
    , cust_email         VARCHAR2(30)
    , cust_orders        ORDER_LIST_TYP
    ) ;
")
        {
            ExpectedResult = new[]
            {
                @"CREATE TYPE customer_typ_demo AS OBJECT
    ( customer_id        NUMBER(6)
    , cust_first_name    VARCHAR2(20)
    , cust_last_name     VARCHAR2(20)
    , cust_address       CUST_ADDRESS_TYP
    , phone_numbers      PHONE_LIST_TYP
    , nls_language       VARCHAR2(3)
    , nls_territory      VARCHAR2(30)
    , credit_limit       NUMBER(9,2)
    , cust_email         VARCHAR2(30)
    , cust_orders        ORDER_LIST_TYP
    )"
            }
        }
    };
}
