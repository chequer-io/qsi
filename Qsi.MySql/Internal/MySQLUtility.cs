using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;

namespace Qsi.MySql.Internal
{
    internal static class MySQLUtility
    {
        // backend/wbpublic/grtdb/db_helpers.cpp
        // bec::version_to_int
        //
        // Version  |  GrtVersionRef
        // ---------+---------------
        // Major    |  Major
        // Minor    |  Minor
        // Build    |  Release
        // Revision |  Build
        public static int VersionToInt(Version version)
        {
            if (version == null || version.Major == -1)
                return 80000;

            int result = version.Major * 10000;

            if (version.Minor > 0)
                result += version.Minor * 100;

            if (version.Build > 0)
                result += version.Build;

            return result;
        }

        public static MySqlParserInternal CreateParser(string input, int version, bool mariaDBCompatibility)
        {
            var stream = new AntlrInputStream(input);

            var lexer = new MySqlLexerInternal(stream)
            {
                serverVersion = version,
                MariaDB = mariaDBCompatibility
            };

            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new MySqlLexerErrorHandler());

            var tokens = new CommonTokenStream(lexer);

            var parser = new MySqlParserInternal(tokens)
            {
                serverVersion = version,
                MariaDB = mariaDBCompatibility,
                Interpreter =
                {
                    PredictionMode = PredictionMode.SLL
                }
            };

            parser.RemoveErrorListeners();
            parser.AddErrorListener(new MySqlParserErrorHandler());

            return parser;
        }
    }
}
