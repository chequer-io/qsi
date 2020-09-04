using Microsoft.SqlServer.Management.SqlParser.Parser;
using Qsi.Diagnostics;

namespace Qsi.SqlServer.Diagnostics
{
    public class SqlServerRawParser : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            var parserResult = Parser.Parse(input);

            return new SqlServerRawTree(parserResult.Script);
        }
    }
}
