using System.Threading;
using PgQuery;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PostgreSql.NewTree;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public class PostgreSqlParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            return PgNodeVisitor.Visit(ParseProtobuf(script).Stmt);
        }

        private RawStmt ParseProtobuf(QsiScript script)
        {
            var parseResult = Parser.ParseProtobuf(script.Script);

            if (parseResult.Stmts.Count == 0)
                throw new QsiException(QsiError.Syntax);

            return parseResult.Stmts[0];
        }
    }
}
