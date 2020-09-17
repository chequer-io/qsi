using Qsi.Diagnostics;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;

namespace Qsi.SqlServer.Diagnostics
{
    public sealed class SqlServerRawTreeParser : IRawTreeParser
    {
        private readonly TSqlParserInternal _parser;
        
        public SqlServerRawTreeParser(TransactSqlVersion version)
        {
            _parser = new TSqlParserInternal(version, false);
        }
        
        public IRawTree Parse(string input)
        {
            return SqlServerRawTreeVisitor.CreateRawTree(_parser.Parse(input));
        }
    }
}
