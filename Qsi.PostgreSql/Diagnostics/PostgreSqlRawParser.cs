using Qsi.Diagnostics;

namespace Qsi.PostgreSql.Diagnostics;

public class PostgreSqlRawParser : IRawTreeParser
{
    public IRawTree Parse(string input)
    {
        var result = PgQuery.Parser.Parse(input);

        return new PostgreSqlRawTree(result.Stmts[0].Stmt);
    }
}
