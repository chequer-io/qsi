using System;
using System.Text;
using Qsi.Parsing.Common;
using Qsi.Parsing.Common.Rules;
using Qsi.Utilities;

namespace Qsi.Oracle
{
    public sealed class OracleCompat : CommonScriptParser
    {
        private const TokenType TokenTypeNationalString = (TokenType)(1 << 8);
        private const TokenType TokenTypeQuotedString = (TokenType)(1 << 9);

        private const string delOpen = "![{<(";
        private const string delClose = "!]}>)";

        private static readonly OracleCompat _compat = new();

        private readonly ITokenRule _singleQuote = new LookbehindIdentifierRule('\'', true);

        protected override bool TryParseToken(CommonScriptCursor cursor, out Token token)
        {
            var index = cursor.Index;

            switch (cursor.Current)
            {
                case 'Q' when TryParseQuoted(cursor, out token):
                    return true;

                case 'N' when TryParseNational(cursor, out token):
                    return true;

                default:
                    cursor.Index = index;
                    break;
            }

            return base.TryParseToken(cursor, out token);
        }

        private bool TryParseQuoted(CommonScriptCursor cursor, out Token token)
        {
            var index = cursor.Index;

            token = default;

            // Q'<>'
            if (cursor.Length - cursor.Index < 5)
                return false;

            cursor.Index++;

            if (cursor.Current != '\'')
                return false;

            cursor.Index++;

            var delimiterIndex = delOpen.IndexOf(cursor.Current);

            if (delimiterIndex == -1)
                return false;

            cursor.Index++;

            var closeIndex = cursor.Value.IndexOf($"{delClose[delimiterIndex]}'", cursor.Index, StringComparison.OrdinalIgnoreCase);

            if (closeIndex == -1)
                return false;

            token = new Token(TokenTypeQuotedString, new Range(index, closeIndex + 2));
            cursor.Index = closeIndex + 2;

            return true;
        }

        private bool TryParseNational(CommonScriptCursor cursor, out Token token)
        {
            var index = cursor.Index;

            token = default;

            if (cursor.Length - index < 3)
                return false;

            cursor.Index++;

            if (cursor.Current is 'Q' or 'q' && TryParseQuoted(cursor, out token))
            {
                token = new Token(
                    TokenTypeNationalString | TokenTypeQuotedString,
                    new Range(index, token.Span.End)
                );

                return true;
            }

            if (cursor.Current is not '\'')
                return false;

            cursor.Index++;

            if (!_singleQuote.Run(cursor))
                return false;

            token = new Token(TokenTypeNationalString, new Range(index, cursor.Index + 1));
            return true;
        }

        public static string Normalize(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                return script;

            var builder = new StringBuilder(script.Length);
            var cursor = new CommonScriptCursor(script);

            foreach (var token in _compat.ParseTokens(cursor))
                builder.Append(GetNormalizedTokenValue(script, token));

            return builder.ToString();
        }

        public static string GetNormalizedTokenValue(string script, Token token)
        {
            var value = script[token.Span];

            switch (token.Type)
            {
                // N'..'
                case TokenTypeNationalString:
                    return value[1..];

                // Q'<1'2'3>'
                case TokenTypeQuotedString:
                    return IdentifierUtility.Escape(value.AsSpan()[3..^2], EscapeQuotes.Single, EscapeBehavior.TwoTime);

                // NQ'<1'2'3>'
                case TokenTypeNationalString | TokenTypeQuotedString:
                    return IdentifierUtility.Escape(value.AsSpan()[4..^2], EscapeQuotes.Single, EscapeBehavior.TwoTime);
            }

            return value;
        }
    }
}
