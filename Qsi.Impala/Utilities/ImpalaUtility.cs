using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Qsi.Impala.Common.Thrift;
using Qsi.Impala.Internal;
using Qsi.Shared;

namespace Qsi.Impala.Utilities
{
    public static class ImpalaUtility
    {
        internal static TReservedWordsVersion GetReservedWordsVersion(Version version)
        {
            if (version.Major >= 3)
                return TReservedWordsVersion.IMPALA_3_0;

            return TReservedWordsVersion.IMPALA_2_11;
        }

        internal static ImpalaParserInternal CreateParserInternal(string input, Version version, IEnumerable<string> builtInFunctions)
        {
            var stream = new StringInputStream(input);
            var lexer = new ImpalaLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ImpalaParserInternal(tokens);

            lexer.Setup(GetReservedWordsVersion(version), builtInFunctions);

            return parser;
        }

        public static bool IsCommentPlanHint(ReadOnlySpan<char> text)
        {
            if (text.StartsWith("/*") || text.StartsWith("--"))
            {
                ReadOnlySpan<char> span = text[2..];

                while (!span.IsEmpty && span[0] != '+')
                {
                    if (!char.IsWhiteSpace(span[0]))
                        return false;

                    span = span[1..];
                }

                return !span.IsEmpty && span[0] == '+';
            }

            return false;
        }
    }
}
