using Google.Protobuf;
using Qsi.Diagnostics;
using Qsi.PhoenixSql.Internal;

namespace Qsi.PhoenixSql.Diagnostics
{
    public class PhoenixSqlRawParser : IRawTreeParser
    {
        public IRawTree Parse(string input)
        {
            return new PhoenixSqlRawTreeNode((IMessage)ParserInternal.Parse(input));
        }
    }
}
