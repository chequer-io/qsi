using Antlr4.Runtime;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonWhereContext : ICommonContext
    {
        public IToken Start { get; }

        public IToken Stop { get; }

        public ExpressionContext Expression { get; }

        public CommonWhereContext(FromClauseContext context)
        {
            Start = context.WHERE().Symbol;
            Stop = context.whereExpr.Stop;
            Expression = context.whereExpr;
        }

        public CommonWhereContext(ExpressionContext expression, IToken start, IToken stop)
        {
            Expression = expression;
            Start = start;
            Stop = stop;
        }
    }
}
