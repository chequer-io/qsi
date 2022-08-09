using NUnit.Framework;

namespace Qsi.Tests.Oracle;

public partial class OracleScriptParserTest
{
    private static readonly TestCaseData[] Parse_TestDatas =
    {
        new("SELECT 1 FROM DUAL;;") { ExpectedResult = "SELECT 1 FROM DUAL" },
        new("CREATE TABLE a(id NUMBER);") { ExpectedResult = "CREATE TABLE a(id NUMBER);" },
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
            ExpectedResult = @"CREATE OR REPLACE PROCEDURE studentInsert(
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
    };
}
