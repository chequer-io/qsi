using System.IO;
using Antlr4.Runtime;

namespace Qsi.MySql.Internal
{
    internal abstract class MySqlBaseRecognizer : Parser, IMySqlRecognizerCommon
    {
        #region SqlMode
        protected const int NoMode = 0;
        protected const int AnsiQuotes = 1 << 0;
        protected const int HighNotPrecedence = 1 << 1;
        protected const int PipesAsConcat = 1 << 2;
        protected const int IgnoreSpace = 1 << 3;
        protected const int NoBackslashEscapes = 1 << 4;
        #endregion

        public int serverVersion { get; init; }

        protected MySqlBaseRecognizer(ITokenStream input) : base(input)
        {
        }

        protected MySqlBaseRecognizer(ITokenStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }

        #region library/parsers/mysql/MySQLRecognizerCommon.cpp
        // MySQLRecognizerCommon::isSqlModeActive
        public bool isSqlModeActive(int mode)
        {
            return mode == 0;
        }
        #endregion
    }
}
