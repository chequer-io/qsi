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
    public async Task Test_Parse(string sql)
    {
        QsiScript[] result = Parser.Parse(sql, default).ToArray();

        await Verifier.Verify(result).UseDirectory("verified");
    }
}
