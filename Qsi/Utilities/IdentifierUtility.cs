using System;

namespace Qsi.Utilities
{
    public static class IdentifierUtility
    {
        private static readonly char[] _openParen = { '\'', '"', '`' };
        private static readonly char[] _closeParen = { '\'', '"', '`' };

        private static readonly char[] _escapedChars = { '\'', '"', '`', '\\', 'n', 'r', 't', 'b', '0' };
        private static readonly char[] _escapeChars = { '\'', '"', '`', '\\', '\n', '\r', '\t', '\b', '\0' };

        public static string Unescape(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            int index = Array.IndexOf(_openParen, value[0]);

            if (index == -1 || _closeParen[index] != value[^1])
                return value;

            return Unescape(value[1..^1], _openParen[index], _closeParen[index]);
        }

        private static string Unescape(in ReadOnlySpan<char> value, char open, char close)
        {
            var buffer = new char[value.Length];
            int index = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                char c = value[i];
                bool end = i == buffer.Length - 1;

                if (c == '\\')
                {
                    int escapeIndex = end ? -1 : Array.IndexOf(_escapedChars, value[i + 1]);

                    // ignore
                    if (escapeIndex == -1)
                        continue;

                    i++;
                    c = _escapeChars[escapeIndex];
                }
                else if (c == open)
                {
                    if (end || value[i + 1] != open)
                    {
#if DEBUG
                        throw new Exception($"Invalid identifier format: {open}{value.ToString()}{close}");
#else
                        continue;
#endif
                    }

                    i++;
                }

                buffer[index++] = c;
            }

            return new string(buffer, 0, index);
        }
    }
}
