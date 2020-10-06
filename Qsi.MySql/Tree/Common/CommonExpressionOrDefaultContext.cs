using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonExpressionOrDefaultContext
    {
        public ExpressionContext Expression { get; }

        public bool IsDefault => Expression == null;

        public CommonExpressionOrDefaultContext(ExpressionContext expression)
        {
            Expression = expression;
        }
    }
}
