using Antlr4.Runtime;
using static Qsi.MySql.Internal.MySqlParser;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct CommonLimitClauseContext : ICommonContext
    {
        public IToken Start { get; }

        public IToken Stop { get; }

        public LimitClauseAtomContext Offset { get; }

        public LimitClauseAtomContext Limit { get; }

        public CommonLimitClauseContext(LimitClauseContext context)
        {
            Start = context.Start;
            Stop = context.Stop;
            Offset = context.offset;
            Limit = context.limit;
        }

        public CommonLimitClauseContext(LimitClauseAtomContext offset, LimitClauseAtomContext limit, IToken start, IToken stop)
        {
            Offset = offset;
            Limit = limit;
            Start = start;
            Stop = stop;
        }
    }
}
