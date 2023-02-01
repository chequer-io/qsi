using PgQuery;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;

namespace Qsi.PostgreSql
{
    public partial class PostgreSqlDeparser : IQsiTreeDeparser
    {
        public string Deparse(IQsiTreeNode node, QsiScript script)
        {
            var result = Visitor.Visit(node);

            return Parser.Deparse(result);
        }

        internal Node ConvertToPgNode(IQsiTreeNode node)
        {
            return Visitor.Visit(node);
        }
    }
}
