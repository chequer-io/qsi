using Qsi.JSql;
using Qsi.JSql.Tree;
using Qsi.Oracle.Tree;

namespace Qsi.Oracle
{
    public sealed class OracleParser : JSqlParser
    {
        protected override JSqlTableVisitor CreateTableVisitor()
        {
            return new OracleTableVisitor(this);
        }

        protected override JSqlExpressionVisitor CreateExpressionVisitor()
        {
            return new OracleExpressionVisitor(this);
        }

        protected override JSqlIdentifierVisitor CreateIdentifierVisitor()
        {
            return new OracleIdentifierVisitor(this);
        }
    }
}
