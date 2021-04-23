using System;
using System.Text.RegularExpressions;

namespace Qsi.Hana.Internal
{
    internal partial class HanaParserInternal
    {
        public bool IsQuotedNumeric()
        {
            var text = CurrentToken.Text;

            if (text.Length < 3)
                return false;

            if (text[0] != '\'' && text[^1] != '\'')
                return false;

            ReadOnlySpan<char> textSpan = text.AsSpan(1, text.Length - 2);

            foreach (var ch in textSpan)
            {
                if (!char.IsNumber(ch))
                    return false;
            }

            return true;
        }

        public bool IsMatch(string pattern)
        {
            return Regex.IsMatch(CurrentToken.Text, pattern);
        }
    }
}
