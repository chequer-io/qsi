using NUnit.Framework;

namespace Qsi.Tests.Oracle;

public partial class OracleScriptParserTest
{
    public static TestCaseData[] ParseTokens_TestDatas()
    {
        return new TestCaseData[]
        {
            new("SELECT * FROM C##TEST.TEST_TABLE")
            {
                ExpectedResult = new[] { "SELECT", "*", "FROM", "C##TEST", ".", "TEST_TABLE" }
            },
            new("SELECT * FROM 한글테이블")
            {
                ExpectedResult = new[] { "SELECT", "*", "FROM", "한글테이블" }
            },
            new("SELECT * FROM 한글_테_이블")
            {
                ExpectedResult = new[] { "SELECT", "*", "FROM", "한글_테_이블" }
            },
            new("SELECT * FROM C##TEST.TEST_TABLE WHERE ID = 1")
            {
                ExpectedResult = new[] { "SELECT", "*", "FROM", "C##TEST", ".", "TEST_TABLE", "WHERE", "ID", "=", "1" }
            },
            new("SELECT * FROM C##TEST.TEST_TABLE WHERE ID = 1 AND NAME = 'TEST'")
            {
                ExpectedResult = new[] { "SELECT", "*", "FROM", "C##TEST", ".", "TEST_TABLE", "WHERE", "ID", "=", "1", "AND", "NAME", "=", "'TEST'" }
            },
            new("SELECT * FROM C##TEST.TEST_TABLE WHERE ID = 1 AND NAME = 'TEST' AND AGE = 20")
            {
                ExpectedResult = new[] { "SELECT", "*", "FROM", "C##TEST", ".", "TEST_TABLE", "WHERE", "ID", "=", "1", "AND", "NAME", "=", "'TEST'", "AND", "AGE", "=", "20" }
            },
            new("ALTER SESSION SET CURRENT_SCHEMA = C##TEST")
            {
                ExpectedResult = new[] { "ALTER", "SESSION", "SET", "CURRENT_SCHEMA", "=", "C##TEST" }
            },
            new("ALTER SESSION SET CURRENT_SCHEMA = \"C##TEST\"")
            {
                ExpectedResult = new[] { "ALTER", "SESSION", "SET", "CURRENT_SCHEMA", "=", "\"C##TEST\"" }
            },
            new("ALTER SESSION SET CURRENT_SCHEMA = \"C##\"\"TEST\"")
            {
                ExpectedResult = new[] { "ALTER", "SESSION", "SET", "CURRENT_SCHEMA", "=", "\"C##\"\"TEST\"" }
            },
            new("ALTER SESSION SET CURRENT_SCHEMA = C###")
            {
                ExpectedResult = new[] { "ALTER", "SESSION", "SET", "CURRENT_SCHEMA", "=", "C###" }
            },
            new("ALTER SESSION SET CURRENT_SCHEMA = C##$")
            {
                ExpectedResult = new[] { "ALTER", "SESSION", "SET", "CURRENT_SCHEMA", "=", "C##$" }
            },
            new("ALTER SESSION SET CURRENT_SCHEMA = C####")
            {
                ExpectedResult = new[] { "ALTER", "SESSION", "SET", "CURRENT_SCHEMA", "=", "C####" }
            },
            new("ALTER SESSION SET CURRENT_SCHEMA = C##__")
            {
                ExpectedResult = new[] { "ALTER", "SESSION", "SET", "CURRENT_SCHEMA", "=", "C##__" }
            },
            new("CREATE USER c## IDENTIFIED BY querypie")
            {
                ExpectedResult = new[] { "CREATE", "USER", "c##", "IDENTIFIED", "BY", "querypie" }
            },
            new("CREATE USER c##$ IDENTIFIED BY querypie")
            {
                ExpectedResult = new[] { "CREATE", "USER", "c##$", "IDENTIFIED", "BY", "querypie" }
            },
            new("CREATE USER c###$ IDENTIFIED BY querypie")
            {
                ExpectedResult = new[] { "CREATE", "USER", "c###$", "IDENTIFIED", "BY", "querypie" }
            },
            new("CREATE USER c##test IDENTIFIED BY querypie")
            {
                ExpectedResult = new[] { "CREATE", "USER", "c##test", "IDENTIFIED", "BY", "querypie" }
            }
        };
    }
}
