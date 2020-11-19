using Antlr4.Runtime;

namespace Qsi.Cql.Internal
{
    internal static class IInputStreamExtension
    {
        public static string Substring(this IIntStream stream, int start, int end)
        {
            int length = end - start + 1;
            var buffer = new char[length];

            for (int i = 0; i < length; i++)
                buffer[i] = (char)stream.LA(i);

            return new string(buffer);
        }
    }
}
