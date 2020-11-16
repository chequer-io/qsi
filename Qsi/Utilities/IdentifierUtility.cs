using System;
using System.Collections.Generic;
using System.Text;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Shared.Extensions;

namespace Qsi.Utilities
{
    public static class IdentifierUtility
    {
        private static readonly string[] _openParen = { "'", "\"", "`", "[", "$$" };
        private static readonly string[] _closeParen = { "'", "\"", "`", "]", "$$" };

        private static readonly char[] _escapedChars = { '\'', '"', '`', '[', ']', '\\', 'n', 'r', 't', 'b', '0' };
        private static readonly char[] _escapeChars = { '\'', '"', '`', '[', ']', '\\', '\n', '\r', '\t', '\b', '\0' };

        public static bool IsEscaped(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            int index = _openParen.IndexOf(value.StartsWith);

            if (index == -1)
                return false;

            return value.EndsWith(_closeParen[index]);
        }

        public static string Unescape(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            int index = _openParen.IndexOf(value.StartsWith);

            if (index == -1 || !value.EndsWith(_closeParen[index]))
                return value;

            var openParen = _openParen[index];
            var closeParen = _closeParen[index];

            return Unescape(value[openParen.Length..^closeParen.Length], openParen, closeParen);
        }

        private static string Unescape(in ReadOnlySpan<char> value, string open, string close)
        {
            var buffer = new char[value.Length];
            int index = 0;
            bool singleParent = close.Length == 1;

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
                else if (value.Slice(i).StartsWith(close))
                {
                    if (end || !value.Slice(i + close.Length).StartsWith(close))
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

        public static QsiIdentifier[] Parse(string value)
        {
            var result = new List<QsiIdentifier>();
            ReadOnlySpan<char> span = value.AsSpan();
            var buffer = new StringBuilder();
            var escaped = false;
            string closeParen = null;

            for (int i = 0; i < span.Length; i++)
            {
                var c = span[i];
                bool end = i == span.Length - 1;

                switch (c)
                {
                    case '.' when closeParen == null:
                        result.Add(new QsiIdentifier(buffer.ToString(), escaped));
                        escaped = false;
                        buffer.Clear();
                        break;

                    case '\\' when !end && closeParen?.Length == 1:
                        buffer.Append(c);

                        if (span[i + 1] == closeParen[0])
                        {
                            i++;
                            buffer.Append(span[i]);
                        }

                        break;

                    case var _ when closeParen != null &&
                                    (closeParen.Length == 1 && closeParen[0] == c ||
                                     span.Slice(i).StartsWith(closeParen)):
                        buffer.Append(c);
                        i += closeParen.Length - 1;
                        closeParen = null;
                        break;

                    case '\'':
                    case '"':
                    case '`':
                    case '[':
                    case '$' when !end && span[i + 1] == '$':
                        buffer.Append(c);

                        if (closeParen == null)
                        {
                            int index = -1;

                            for (int j = 0; j < _openParen.Length; j++)
                            {
                                var paren = _openParen[j];

                                if (paren.Length == 1 && paren[0] == c || span.Slice(i).StartsWith(paren))
                                {
                                    index = j;
                                    break;
                                }
                            }

                            escaped = true;
                            closeParen = _closeParen[index];
                        }

                        break;

                    default:
                        buffer.Append(c);
                        break;
                }
            }

            result.Add(new QsiIdentifier(buffer.ToString(), escaped));

            return result.ToArray();
        }
    }
}
