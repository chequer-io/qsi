using Antlr4.Runtime;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonExpressionOrDefaultContext
    {
        public ParserRuleContext Context { get; }

        public ExpressionContext Expression { get; }

        public bool IsDefault => Expression == null;

        public CommonExpressionOrDefaultContext(ExpressionOrDefaultContext context)
        {
            Context = context;
            Expression = context.expression();
        }

        public CommonExpressionOrDefaultContext(UpdatedElementContext context)
        {
            Context = context;
            Expression = context.expression();
        }
    }
}
