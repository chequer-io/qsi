using Qsi.JSql;
using Qsi.JSql.Tree;
using Qsi.Oracle.Tree;

namespace Qsi.Oracle
{
    public sealed class OracleParser : JSqlParser
    {
        protected override JSqlTableVisitor CreateTableVisitor(IJSqlVisitorSet set)
        {
            return new OracleTableVisitor(set);
        }

        protected override JSqlExpressionVisitor CreateExpressionVisitor(IJSqlVisitorSet set)
        {
            return new OracleExpressionVisitor(set);
        }

        protected override JSqlIdentifierVisitor CreateIdentifierVisitor(IJSqlVisitorSet set)
        {
            return new OracleIdentifierVisitor(set);
        }
    }
}
