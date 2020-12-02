using System;
using System.Collections.Generic;
using System.Text;
using PrimarSql.Data.Models.Columns;
using Qsi.Shared.Extensions;
using Qsi.Utilities;

namespace Qsi.PrimarSql.Utilities
{
    internal static class PartUtility
    {
        private static readonly string[] _openParen = { "'", "\"", "`", "[", "$$" };
        private static readonly string[] _closeParen = { "'", "\"", "`", "]", "$$" };

        public static IPart[] Parse(string value)
        {
            var result = new List<IPart>();
            ReadOnlySpan<char> span = value.AsSpan();
            var buffer = new StringBuilder();
            var escaped = false;
            string closeParen = null;

            void Add()
            {
                if (buffer[0] == '[' && buffer[^1] == ']')
                {
                    var i = buffer.ToString()[1..^1];

                    if (i.Length == 0)
                        throw new InvalidOperationException("Identifier index is empty");

                    result.Add(new IndexPart(int.Parse(i)));
                }
                else
                {
                    result.Add(new IdentifierPart(escaped ? IdentifierUtility.Unescape(buffer.ToString()) : buffer.ToString()));
                }

                buffer.Clear();
            }

            for (int i = 0; i < span.Length; i++)
            {
                var c = span[i];
                bool end = i == span.Length - 1;

                switch (c)
                {
                    case '[' when closeParen == null:
                        Add();
                        escaped = false;

                        buffer.Append(c);
                        closeParen = _closeParen[_openParen.IndexOf("[")];
                        break;

                    case '.' when closeParen == null:
                        Add();
                        escaped = false;
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

            Add();

            return result.ToArray();
        }
    }
}
