using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

namespace Qsi.MySql.Internal
{
    internal abstract class MySQLBaseLexer : Lexer, IMySQLRecognizerCommon
    {
        #region SqlMode
        protected const int NoMode = 0;
        protected const int AnsiQuotes = 1 << 0;
        protected const int HighNotPrecedence = 1 << 1;
        protected const int PipesAsConcat = 1 << 2;
        protected const int IgnoreSpace = 1 << 3;
        protected const int NoBackslashEscapes = 1 << 4;
        #endregion

        public long serverVersion { get; set; }

        protected bool inVersionComment;

        private readonly Queue<IToken> _pendingTokens = new Queue<IToken>();
        private readonly HashSet<string> _charsets = new HashSet<string>();

        protected MySQLBaseLexer(ICharStream input) : base(input)
        {
        }

        protected MySQLBaseLexer(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }

        // C++ Antlr Lexer::setType
        protected void setType(int type)
        {
            Type = type;
        }

        // C++ Antlr Lexer::getText
        protected string getText()
        {
            return Text;
        }

        #region library/parsers/mysql/MySQLRecognizerCommon.cpp
        // MySQLRecognizerCommon::isSqlModeActive
        public bool isSqlModeActive(int mode)
        {
            return mode == 0;
        }
        #endregion

        #region library/parsers/mysql/MySQLBaseLexer.cpp
        // MySQLBaseLexer::reset
        public override void Reset()
        {
            inVersionComment = false;
            base.Reset();
        }

        // MySQLBaseLexer::nextToken
        public override IToken NextToken()
        {
            if (_pendingTokens.TryDequeue(out var pending))
                return pending;

            var next = base.NextToken();

            if (_pendingTokens.TryDequeue(out pending))
            {
                _pendingTokens.Enqueue(next);
                return pending;
            }

            return next;
        }

        // MySQLBaseLexer::checkVersion
        protected bool checkVersion(string text)
        {
            if (text.Length < 8)
                return false;

            long version = long.Parse(text[3..]);

            if (version > serverVersion)
                return false;

            inVersionComment = true;
            return true;
        }

        // MySQLBaseLexer::checkCharset
        protected int checkCharset(string text)
        {
            return _charsets.Contains(text) ? MySQLLexer.UNDERSCORE_CHARSET : MySQLLexer.IDENTIFIER;
        }

        // MySQLBaseLexer::emitDot
        protected void emitDot()
        {
            var token = TokenFactory.Create(
                new Tuple<ITokenSource, ICharStream>(this, (ICharStream)InputStream),
                MySQLLexer.DOT_SYMBOL,
                Text,
                Channel,
                TokenStartCharIndex,
                TokenStartCharIndex,
                TokenStartLine,
                TokenStartColumn);

            _pendingTokens.Enqueue(token);

            // ++TokenStartCharIndex;
        }

        // MySQLBaseLexer::determineFunction
        protected int determineFunction(int proposed)
        {
            if (isSqlModeActive(IgnoreSpace))
            {
                int input = InputStream.LA(1);

                while (input == ' ' || input == '\t' || input == '\r' || input == '\n')
                {
                    Interpreter.Consume((ICharStream)InputStream);
                    Channel = Hidden;
                    Type = MySQLLexer.WHITESPACE;
                    input = InputStream.LA(1);
                }
            }

            return InputStream.LA(1) == '(' ? proposed : MySQLLexer.IDENTIFIER;
        }

        // MySQLBaseLexer::determineNumericType
        protected int determineNumericType(string text)
        {
            const string long_str = "2147483647";
            const int long_len = 10;
            const string signed_long_str = "-2147483648";
            const string longlong_str = "9223372036854775807";
            const int longlong_len = 19;
            const string signed_longlong_str = "-9223372036854775808";
            const int signed_longlong_len = 19;
            const string unsigned_longlong_str = "18446744073709551615";
            const int unsigned_longlong_len = 20;

            int length = text.Length - 1;

            if (length < long_len)
                return MySQLLexer.INT_NUMBER;

            bool negative = false;
            int index = 0;

            switch (text[0])
            {
                case '+':
                    index++;
                    length--;
                    break;

                case '-':
                    index++;
                    length--;
                    negative = true;
                    break;
            }

            while (text[index] == '0' && length > 0)
            {
                index++;
                length--;
            }

            if (length < long_len)
                return MySQLLexer.INT_NUMBER;

            int smaller;
            int bigger;
            string cmp;

            if (negative)
            {
                if (length == long_len)
                {
                    cmp = signed_long_str[1..];
                    smaller = MySQLLexer.INT_NUMBER;
                    bigger = MySQLLexer.LONG_NUMBER;
                }
                else if (length < signed_longlong_len)
                {
                    return MySQLLexer.LONG_NUMBER;
                }
                else if (length > signed_longlong_len)
                {
                    return MySQLLexer.DECIMAL_NUMBER;
                }
                else
                {
                    cmp = signed_longlong_str[1..];
                    smaller = MySQLLexer.LONG_NUMBER;
                    bigger = MySQLLexer.DECIMAL_NUMBER;
                }
            }
            else
            {
                if (length == long_len)
                {
                    cmp = long_str;
                    smaller = MySQLLexer.INT_NUMBER;
                    bigger = MySQLLexer.LONG_NUMBER;
                }
                else if (length < longlong_len)
                {
                    return MySQLLexer.LONG_NUMBER;
                }
                else if (length > longlong_len)
                {
                    if (length > unsigned_longlong_len)
                        return MySQLLexer.DECIMAL_NUMBER;

                    cmp = unsigned_longlong_str;
                    smaller = MySQLLexer.ULONGLONG_NUMBER;
                    bigger = MySQLLexer.DECIMAL_NUMBER;
                }
                else
                {
                    cmp = longlong_str;
                    smaller = MySQLLexer.LONG_NUMBER;
                    bigger = MySQLLexer.ULONGLONG_NUMBER;
                }
            }

            int cmpIndex = 0;

            while (cmpIndex < cmp.Length && cmp[cmpIndex++] == text[index++])
            {
            }

            return text[index - 1] <= cmp[cmpIndex - 1] ? smaller : bigger;
        }
        #endregion
    }
}
