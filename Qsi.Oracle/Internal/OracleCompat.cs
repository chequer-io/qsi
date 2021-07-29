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
    internal sealed class OracleCompat : CommonScriptParser
    {
        private const TokenType TokenTypeNationalString = (TokenType)(1 << 8);
        private const TokenType TokenTypeQuotedString = (TokenType)(1 << 9);

        private const string delOpen = "![{<(";
        private const string delClose = "!]}>)";

        private static readonly OracleCompat _compat = new();

        protected override bool TryParseToken(CommonScriptCursor cursor, out Token token)
        {
            var current = char.ToUpperInvariant(cursor.Current);

            if (current is 'Q' or 'N')
            {
            }

            return base.TryParseToken(cursor, out token);
        }

        public static string Normalize(string script)
        {
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
