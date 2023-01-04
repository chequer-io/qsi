using System;
using System.Collections.Generic;
using System.Text;
using Qsi.Data;
using Qsi.Shared;
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

            return Unescape(value.AsSpan()[openParen.Length..^closeParen.Length], openParen, closeParen);
        }

        private static string Unescape(ReadOnlySpan<char> value, string open, string close)
        {
            if (value.IsEmpty)
                return string.Empty;

            ReadOnlySpan<char> transition = new string(new[] { close[0], '\\' });

            var index = value.IndexOfAny(transition);

            if (index == -1)
                return value.ToString();

            var builder = StringBuilderCache.Acquire(value.Length);

            do
            {
                if (index >= value.Length - 1)
                    throw new InvalidOperationException($"Failed to unescape string: {value}");
                
                bool end = index == value.Length - 1;

                if (index > 0)
                {
                    builder.Append(value[..index]);
                    value = value[index..];
                }

                if (value[0] is '\\')
                {
                    // ignore
                    if (end)
                        continue;

                    int escapeIndex = Array.IndexOf(_escapedChars, value[1]);

                    // ignore
                    if (escapeIndex == -1)
                    {
                        value = value[1..];
                        continue;
                    }

                    value = value[2..];
                    builder.Append(_escapeChars[escapeIndex]);
                }
                else if (close.Length == 1 || value.StartsWith(close))
                {
                    // ignore
                    if (end)
                        continue;

                    if (!value[close.Length..].StartsWith(close))
                    {
                        value = value[close.Length..];
                        continue;
                    }

                    value = value[(close.Length * 2)..];
                    builder.Append(close);
                }
            } while (!value.IsEmpty && (index = value.IndexOfAny(transition)) >= 0);

            if (!value.IsEmpty)
                builder.Append(value);

            return StringBuilderCache.GetStringAndRelease(builder);
        }

        public static string Escape(in ReadOnlySpan<char> value, EscapeQuotes quotes, EscapeBehavior behavior)
        {
            var buffer = new StringBuilder(value.Length * 2);
            var open = _openParen[(int)quotes];
            var close = _closeParen[(int)quotes];

            buffer.Append(open);

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                var escapeIndex = _escapeChars.IndexOf(c);

                if (escapeIndex >= 5)
                {
                    buffer.Append('\\');
                    buffer.Append(_escapedChars[escapeIndex]);
                }
                else if (close[0] == c && close.Length == 1 || value[i..].StartsWith(close))
                {
                    buffer.Append(behavior == EscapeBehavior.Backslash ? '\\' : c);
                    buffer.Append(c);
                    i += close.Length - 1;
                }
                else
                {
                    buffer.Append(c);
                }
            }

            buffer.Append(close);

            return buffer.ToString();
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
                                     span[i..].StartsWith(closeParen)):
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

                                if (paren.Length == 1 && paren[0] == c || span[i..].StartsWith(paren))
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

    public enum EscapeQuotes
    {
        Single = 0,
        Double = 1,
        Back = 2,
        SquareBracket = 3,
        DoubleDollar = 4
    }

    public enum EscapeBehavior
    {
        Backslash,
        TwoTime
    }
}
