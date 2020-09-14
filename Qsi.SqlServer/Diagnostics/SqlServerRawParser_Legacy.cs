using Microsoft.SqlServer.Management.SqlParser.Common;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Qsi.Diagnostics;

namespace Qsi.SqlServer.Diagnostics
{
    public class SqlServerRawParser_Legacy : IRawTreeParser
    {
        private readonly DatabaseCompatibilityLevel _compatibilityLevel;
        private readonly TransactSqlVersion _transactSqlVersion;

        public SqlServerRawParser_Legacy(DatabaseCompatibilityLevel compatibilityLevel, TransactSqlVersion transactSqlVersion)
        {
            _compatibilityLevel = compatibilityLevel;
            _transactSqlVersion = transactSqlVersion;
        }

        public IRawTree Parse(string input)
        {
            var parserOptions = new ParseOptions
            {
                CompatibilityLevel = _compatibilityLevel,
                TransactSqlVersion = _transactSqlVersion
            };

            var parserResult = Parser.Parse(input, parserOptions);

            return new SqlServerRawTree_Legacy(parserResult.Script, parserOptions);
        }
    }
}
