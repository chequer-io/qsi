using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Qsi.Data;
using Qsi.MySql;
using VerifyNUnit;

namespace Qsi.Tests.Vendor.MySql;

public class MySqlScriptParserTests
{
    public MySqlScriptParser Parser { get; } = new();

    [TestCase("SELECT 1; SELECT ';';")]
    [TestCase("TABLE actor")]
    [TestCase("DESC actor")]
    [TestCase("DEALLOCATE PREPARE statement")]
    [TestCase("SET STATEMENT max_statement_time=1000 FOR SELECT 1")]
    [TestCase("SET STATEMENT optimizer_switch='materialization=off' FOR SELECT 2")]
    [TestCase("SET STATEMENT join_cache_level=6, optimizer_switch='mrr=on' FOR SELECT 3")]
    [TestCase("SET STATEMENT sort_buffer_size = 100000 for SET SESSION sort_buffer_size = 200000")]
    [TestCase("SET STATEMENT @t = (SELECT SUBSTRING('abc' FROM 2 FOR 1)) FOR SELECT @t")]
    [TestCase("SET STATEMENT @t = 5 /* FOR */ FOR SELECT @t")]
    [TestCase("SET STATEMENT @t = 'FOR' FOR SELECT @t")]
    [TestCase("LOAD DATA INFILE 'something.csv' INTO TABLE tbl1")]
    [TestCase(@"LOAD DATA INFILE '/var/lib/mysql-files/test.csv' INTO TABLE test FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n' IGNORE 1 LINES (col1, col2) SET col1 = 'value'")]
    public async Task Test_Parse(string sql)
    {
        QsiScript[] result = Parser.Parse(sql, default).ToArray();

        await Verifier.Verify(result).UseDirectory("verified");
    }

    [TestCase("SELECT 1; SELECT ';';", ExpectedResult = QsiScriptType.Select)]
    [TestCase("TABLE actor", ExpectedResult = QsiScriptType.Select)]
    [TestCase("DESC actor", ExpectedResult = QsiScriptType.Describe)]
    [TestCase("DEALLOCATE PREPARE statement", ExpectedResult = QsiScriptType.DropPrepare)]
    [TestCase("SET STATEMENT max_statement_time=1000 FOR SELECT 1", ExpectedResult = QsiScriptType.Select)]
    [TestCase("SET STATEMENT optimizer_switch='materialization=off' FOR SELECT 2", ExpectedResult = QsiScriptType.Select)]
    [TestCase("SET STATEMENT join_cache_level=6, optimizer_switch='mrr=on' FOR SELECT 3", ExpectedResult = QsiScriptType.Select)]
    [TestCase("SET STATEMENT sort_buffer_size = 100000 for SET SESSION sort_buffer_size = 200000", ExpectedResult = QsiScriptType.Set)]
    [TestCase("SET STATEMENT @t = (SELECT SUBSTRING('abc' FROM 2 FOR 1)) FOR SELECT @t", ExpectedResult = QsiScriptType.Select)]
    [TestCase("SET STATEMENT @t = 5 /* FOR */ FOR SELECT @t", ExpectedResult = QsiScriptType.Select)]
    [TestCase("SET STATEMENT @t = 'FOR' FOR SELECT @t", ExpectedResult = QsiScriptType.Select)]
    [TestCase("LOAD DATA INFILE 'something.csv' INTO TABLE tbl1", ExpectedResult = QsiScriptType.Insert)]
    [TestCase(@"LOAD DATA INFILE '/var/lib/mysql-files/test.csv' INTO TABLE test FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n' IGNORE 1 LINES (col1, col2) SET col1 = 'value'", ExpectedResult = QsiScriptType.Insert)]
    public QsiScriptType Test_GetSuitableType(string sql)
    {
        return Parser.GetSuitableType(sql);
    }

    [TestCase("SET STATEMENT @t = 'FOR' FOR SELECT @t")]
    [TestCase("SET STATEMENT @t = 'FOR' FOR/*aa*/SELECT @t")]
    [TestCase("SET STATEMENT @t = 'FOR'/*a*/FOR/*b*/SELECT @t")]
    public async Task Test_TrySplitSetStatement_ReturnsTrue(string sql)
    {
        var result = Parser.TrySplitSetStatement(sql, out var setPart, out var statementPart);

        Assert.IsTrue(result);

        await Verifier.Verify(new { Set = setPart, Statement = statementPart }).UseDirectory("verified");
    }

    [TestCase("SELECT 1", ExpectedResult = false)]
    [TestCase("SET @t = 'a'", ExpectedResult = false)]
    public bool Test_TrySplitSetStatement_ReturnsFalse(string sql)
    {
        return Parser.TrySplitSetStatement(sql, out _, out _);
    }
}
