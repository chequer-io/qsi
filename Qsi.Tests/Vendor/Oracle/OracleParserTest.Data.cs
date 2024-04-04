using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Qsi.Tests.Oracle;

public partial class OracleParserTest
{
    private static readonly TestCaseData[] Parse_TestDatas =
    {
        new("SELECT 1 FROM DUAL"),
        new("SELECT 1 FROM ((((DUAL))))"),
        new("SELECT * FROM (HR.FIRST) a JOIN (HR.SECOND) b ON a.id = b.id"),
        new("SELECT * FROM ((HR.FIRST)) a JOIN ((HR.SECOND)) b ON a.id = b.id"),
        new("MERGE INTO HR.FIRST a USING HR.SECOND b ON (a.id = b.id) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET a.name = b.name " +
            "WHEN NOT MATCHED THEN " +
            "INSERT (id, name) VALUES (b.id, b.name)"),
        new("MERGE INTO emp a " +
            "USING (" +
            "SELECT aa.empno, aa.job, aa.deptno FROM emp aa, dept bb " +
            "WHERE aa.empno = 7788 AND aa.deptno = bb.deptno" +
            ") b ON (a.empno = b.empno) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET a.job = b.job , a.deptno = b.deptno " +
            "WHEN NOT MATCHED THEN " +
            "INSERT (a.empno, a.job, a.deptno) VALUES (b.empno, b.job, b.deptno);"),
        new("MERGE INTO emp a " +
            "USING dual ON (a.empno = 7788) " +
            "WHEN MATCHED THEN " +
            "UPDATE SET a.deptno = 20 WHERE a.job = 'ANALYST' " +
            "DELETE WHERE a.job <> 'ANALYST';"),
    };

    private static IEnumerable<TestCaseData> Parse_Oracle19Datas
        => ReadSqlFromResources("oracle_19").Select(sql => new TestCaseData(sql)
        {
            TestName = TrimRemark(sql)
        });

    private static readonly Regex _regex = new("^(-- https://docs.oracle.com/en/database/oracle/oracle-database/19/.+)\\n");

    private static string TrimRemark(string sql)
    {
        var matches = _regex.Matches(sql);

        return matches.Count == 0
            ? sql
            : sql[matches[0].Length..];
    }

    private static IEnumerable<string> ReadSqlFromResources(string folder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames();

        return resources
            .Where(r => r.StartsWith($"Qsi.Tests.Resources.Oracle.{folder}") && r.EndsWith(".sql"))
            .Select(filePath =>
            {
                using var stream = assembly.GetManifestResourceStream(filePath)!;
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            });
    }
}
