using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Qsi.Impala.Internal;

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
            var stream = new AntlrInputStream(input);
            var lexer = new ImpalaLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ImpalaParserInternal(tokens);
            parser.AddErrorListener(new ErrorListener());

            lexer.Setup(GetReservedWordsVersion(version), builtInFunctions);
            parser.Setup(lexer);

            return parser;
        }
    }
}
