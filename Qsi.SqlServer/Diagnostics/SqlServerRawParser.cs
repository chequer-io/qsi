using Microsoft.SqlServer.Management.SqlParser.Common;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Qsi.Diagnostics;

namespace Qsi.SqlServer.Diagnostics
{
    public class SqlServerRawParser : IRawTreeParser
    {
        private readonly DatabaseCompatibilityLevel _compatibilityLevel;
        private readonly TransactSqlVersion _transactSqlVersion;

        public SqlServerRawParser(DatabaseCompatibilityLevel compatibilityLevel, TransactSqlVersion transactSqlVersion)
        {
            _compatibilityLevel = compatibilityLevel;
            _transactSqlVersion = transactSqlVersion;
        }

        public IRawTree Parse(string input)
        {
            var parserResult = Parser.Parse(input, new ParseOptions
            {
                CompatibilityLevel = _compatibilityLevel,
                TransactSqlVersion = _transactSqlVersion
            });

            return new SqlServerRawTree(parserResult.Script, _compatibilityLevel);
        }
    }
}
