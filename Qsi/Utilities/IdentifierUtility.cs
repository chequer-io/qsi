using System;
using System.Collections.Generic;
using System.Text;

namespace Qsi.Utilities
{
    public static class IdentifierUtility
    {
        private static readonly char[] _openParen = { '\'', '"', '`', '[' };
        private static readonly char[] _closeParen = { '\'', '"', '`', ']' };

        private static readonly char[] _escapedChars = { '\'', '"', '`', '\\', 'n', 'r', 't', 'b', '0' };
        private static readonly char[] _escapeChars = { '\'', '"', '`', '\\', '\n', '\r', '\t', '\b', '\0' };

        public static bool IsEscaped(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 2)
                return false;

            int index = Array.IndexOf(_openParen, value[0]);

            if (index == -1)
                return false;

            return value[^1] == _closeParen[index];
        }

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

        public static string[] Parse(string value)
        {
            var result = new List<string>(4);
            ReadOnlySpan<char> span = value.AsSpan();
            var buffer = new StringBuilder();
            char? closeParen = null;

            for (int i = 0; i < span.Length; i++)
            {
                var c = span[i];
                bool end = i == buffer.Length - 1;

                switch (c)
                {
                    case '.' when !closeParen.HasValue:
                        result.Add(buffer.ToString());
                        buffer.Clear();
                        break;

                    case '\\':
                        int escapeIndex = end ? -1 : Array.IndexOf(_escapedChars, span[i + 1]);

                        // ignore
                        if (escapeIndex == -1)
                            continue;

                        buffer.Append(_escapeChars[escapeIndex]);
                        i++;
                        break;

                    case var _ when closeParen.HasValue && c == closeParen.Value:
                        if (!end && span[i + 1] == c)
                        {
                            buffer.Append(c);
                            i++;
                        }
                        else
                        {
                            closeParen = null;
                        }

                        break;

                    case '\'':
                    case '"':
                    case '`':
                    case '[':
                        if (closeParen.HasValue)
                        {
                            goto default;
                        }
                        else
                        {
                            closeParen = _closeParen[Array.IndexOf(_openParen, c)];
                        }

                        break;

                    default:
                        buffer.Append(c);
                        break;
                }
            }

            if (span[^1] != '.')
                result.Add(buffer.ToString());

            return result.ToArray();
        }
    }
}
