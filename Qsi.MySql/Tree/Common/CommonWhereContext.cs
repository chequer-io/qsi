using Antlr4.Runtime;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonWhereContext
    {
        public IToken Start { get; }

        public ExpressionContext Expression { get; }

        public CommonWhereContext(FromClauseContext context)
        {
            Start = context.WHERE().Symbol;
            Expression = context.whereExpr;
        }
    }
}
