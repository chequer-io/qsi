using System.Threading;
using Qsi.Data;
using Qsi.JSql;
using Qsi.JSql.Tree;
using Qsi.Oracle.Tree;
using Qsi.Parsing.Common;
using Qsi.Tree;

namespace Qsi.Oracle
{
    public sealed class OracleParser : JSqlParser
    {
        public override IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            script = new QsiScript(OracleCompat.Normalize(script.Script), script.ScriptType);
            return base.Parse(script, cancellationToken);
        }

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
