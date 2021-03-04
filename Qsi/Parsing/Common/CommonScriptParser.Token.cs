using System;

namespace Qsi.Parsing.Common
{
    public partial class CommonScriptParser
    {
        [Flags]
        public enum TokenType
        {
            Keyword = 1 << 1,
            Literal = 1 << 2,
            Identifier = 1 << 3,
            Fragment = 1 << 4,
            WhiteSpace = 1 << 5,
            SingeLineComment = 1 << 6,
            MultiLineComment = 1 << 7,

            Effective = Keyword | Literal | Identifier | Fragment,
            Trivia = WhiteSpace | SingeLineComment | MultiLineComment
        }

        public readonly struct Token
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
