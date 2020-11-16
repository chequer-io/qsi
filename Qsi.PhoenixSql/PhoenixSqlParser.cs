using System.Threading;
using PhoenixSql;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PhoenixSql.Tree;
using Qsi.Tree;
using Qsi.Utilities;
using PhoenixSqlParserInternal = PhoenixSql.PhoenixSqlParser;

namespace Qsi.PhoenixSql
{
    public class PhoenixSqlParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var result = PhoenixSqlParserInternal.Parse(script.Script);

            switch (result)
            {
                case SelectStatement selectStatement:
                    return TableVisitor.VisitSelectStatement(selectStatement);

                case IDMLStatement dmlStatement:
                    return ActionVisitor.Visit(dmlStatement);
            }

            throw TreeHelper.NotSupportedTree(result);
        }
    }
}
