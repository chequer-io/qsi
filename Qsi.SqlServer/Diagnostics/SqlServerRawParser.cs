using Microsoft.SqlServer.Management.SqlParser.Common;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Qsi.Diagnostics;

namespace Qsi.SqlServer.Diagnostics
{
    public class SqlServerRawParser : IRawTreeParser
    {
        private readonly DatabaseCompatibilityLevel _compatibilityLevel;
        
        public SqlServerRawParser(DatabaseCompatibilityLevel compatibilityLevel)
        {
            _compatibilityLevel = compatibilityLevel;
        }
        
        public IRawTree Parse(string input)
        {
            var parserResult = Parser.Parse(input, new ParseOptions
            {
                CompatibilityLevel = _compatibilityLevel
            });

            return new SqlServerRawTree(parserResult.Script, _compatibilityLevel);
        }
    }
}
