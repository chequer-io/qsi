using System.Threading;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;
using PhoenixSqlParserInternal = PhoenixSql.PhoenixSqlParser;

namespace Qsi.PhoenixSql
{
    public class PhoenixSqlParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
