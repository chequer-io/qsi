using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Qsi.Cql.Internal
{
    internal static class InputStreamExtension
    {
        public static bool StartsWith(this IIntStream stream, string value)
        {
            return StartsWith(stream, 0, value);
        }

        public static bool StartsWith(this IIntStream stream, int offset, string value)
        {
            if (stream.Index + offset + value.Length >= stream.Size)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != (char)stream.LA(offset + i + 1))
                    return false;
            }

            return true;
        }

        public static string Substring(this IIntStream stream, int startIndex, int length)
        {
            if (stream is ICharStream charStream)
                return charStream.GetText(Interval.Of(startIndex, startIndex + length - 1));

            throw new NotSupportedException();
        }
    }
}
