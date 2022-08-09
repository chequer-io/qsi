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
