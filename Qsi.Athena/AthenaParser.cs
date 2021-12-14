using System.Threading;
using Qsi.Athena.Internal;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Athena
{
    public class AthenaParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var (_, result) = SqlParser.Parse(script.Script, parser => parser.singleStatement());

            var statement = result.statement();

            switch (statement)
            {
                default:
                    throw TreeHelper.NotSupportedTree(statement);
            }
        }
    }
}
