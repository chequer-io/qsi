using Antlr4.Runtime;

namespace Qsi.MySql.Tree.Common
{
    internal readonly struct ParserRuleContextWrapper<T> : IParserRuleContext
    {
        public T Value { get; }

        public IToken Start { get; }

        public IToken Stop { get; }

        public ParserRuleContextWrapper(T value, IToken start, IToken stop)
        {
            Value = value;
            Start = start;
            Stop = stop;
        }

        public ParserRuleContextWrapper(T value, ParserRuleContext context) : this(value, context.Start, context.Stop)
        {
        }
    }
}
