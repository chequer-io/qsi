using System;

namespace Qsi.Parsing.Common
{
    public partial class CommonScriptParser
    {
        [Flags]
        protected enum TokenType
        {
            Keyword = 1 << 1,
            Literal = 1 << 2,
            Fragment = 1 << 3,
            WhiteSpace = 1 << 4,
            SingeLineComment = 1 << 5,
            MultiLineComment = 1 << 6,

            Effective = Keyword | Literal | Fragment,
            Trivia = WhiteSpace | SingeLineComment | MultiLineComment
        }

        protected readonly struct Token
        {
            public Range Span { get; }

            public TokenType Type { get; }

            public Token(TokenType type, Range span)
            {
                Type = type;
                Span = span;
            }

            public override string ToString()
            {
                return $"{Span} ({Type})";
            }
        }
    }
}
