using System;
using Qsi.Parsing.Common;

namespace Qsi.Oracle
{
    /*
SINGLE_QUOTED_STRING: '\'' ( '\'\'' | ~['] )* '\'';

QUOTED_STRING
    : Q '\'!' ~[\u0000]*? '!\''
    | Q '\'[' ~[\u0000]*? ']\''
    | Q '\'{' ~[\u0000]*? '}\''
    | Q '\'<' ~[\u0000]*? '>\''
    | Q '\'(' ~[\u0000]*? ')\''
    ;

NATIONAL_STRING
    : N (
          SINGLE_QUOTED_STRING
        | QUOTED_STRING
    );
     */
    public sealed class OracleCompat : CommonScriptParser
    {
        private const TokenType TokenTypeNationalString = (TokenType)(1 << 8);
        private const TokenType TokenTypeQuotedString = (TokenType)(1 << 9);

        private const string delOpen = "![{<(";
        private const string delClose = "!]}>)";

        private static readonly OracleCompat _compat = new();

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
            token = default;

            if (cursor.Length - cursor.Index - 1 < 4)
                return false;

            ReadOnlySpan<char> span = cursor.Value.AsSpan(cursor.Index + 1);

            if (span[0] != '\'')
                return false;

            var delimiterIndex = delOpen.IndexOf(span[1]);

            if (delimiterIndex == -1)
                return false;

            var closeIndex = cursor.Value.IndexOf($"{delClose[delimiterIndex]}'", cursor.Index + 1, StringComparison.OrdinalIgnoreCase);

            if (closeIndex == -1)
                return false;

            token = new Token(TokenTypeQuotedString, new Range(cursor.Index, closeIndex + 2));
            cursor.Index = closeIndex + 2;

            return true;
        }

        private bool TryParseNational(CommonScriptCursor cursor, out Token token)
        {
            throw new NotImplementedException();
        }

        public static string Normalize(string script)
        {
            return null;
        }

        public static bool GetLiteralValue(ReadOnlySpan<char> value)
        {
            // '...'
            if (value[0] == '\'')
                return true;

            // NQ'...' -> Q'...'
            if (value.StartsWith("N", StringComparison.OrdinalIgnoreCase))
                value = value[1..];

            if (!value.StartsWith("Q'", StringComparison.OrdinalIgnoreCase))
                return true;

            // Q'...'
            var delimiterIndex = delOpen.IndexOf(value[2]);
            char delimiterClose = delimiterIndex == -1 ? value[2] : delClose[delimiterIndex];

            return value[^2] == delimiterClose;
        }
    }
}
