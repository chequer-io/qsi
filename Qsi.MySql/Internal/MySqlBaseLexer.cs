using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

namespace Qsi.MySql.Internal
{
    internal abstract class MySqlBaseLexer : Lexer, IMySqlRecognizerCommon
    {
        #region SqlMode
        protected const int NoMode = 0;
        protected const int AnsiQuotes = 1 << 0;
        protected const int HighNotPrecedence = 1 << 1;
        protected const int PipesAsConcat = 1 << 2;
        protected const int IgnoreSpace = 1 << 3;
        protected const int NoBackslashEscapes = 1 << 4;
        #endregion

        #region Charsets
        // modules/db.mysql/res/mysql_rdbms_info.xml
        private static readonly string[] CharacterSets =
        {
            "_big5",
            "_dec8",
            "_cp850",
            "_hp8",
            "_koi8r",
            "_latin1",
            "_latin2",
            "_swe7",
            "_ascii",
            "_ujis",
            "_sjis",
            "_hebrew",
            "_tis620",
            "_euckr",
            "_koi8u",
            "_gb18030",
            "_gb2312",
            "_greek",
            "_cp1250",
            "_gbk",
            "_latin5",
            "_armscii8",
            "_utf8",
            "_ucs2",
            "_cp866",
            "_keybcs2",
            "_macce",
            "_macroman",
            "_cp852",
            "_latin7",
            "_cp1251",
            "_cp1256",
            "_cp1257",
            "_binary",
            "_geostd8",
            "_cp932",
            "_eucjpms",
            "_utf8mb4",
            "_utf16",
            "_utf32"
        };
        #endregion

        public int serverVersion
        {
            get => _serverVersion;
            init
            {
                _serverVersion = value;
                _mySqlVersion = MySqlSymbolInfo.numberToVersion(value);

                #region modules/db.mysql.parser/src/mysql_parser_module.cpp
                // updateServerVersion
                if (value < 50503)
                {
                    Charsets.Remove("_utf8mb4");
                    Charsets.Remove("_utf16");
                    Charsets.Remove("_utf32");
                }
                else
                {
                    Charsets.Add("_utf8mb3");
                }
                #endregion
            }
        }

        public HashSet<string> Charsets { get; } = new(CharacterSets);

        protected bool inVersionComment;

        private readonly int _serverVersion;
        private readonly MySqlVersion _mySqlVersion;
        private readonly Queue<IToken> _pendingTokens = new();
        private Dictionary<string, int> _symbols;

        protected MySqlBaseLexer(ICharStream input) : base(input)
        {
        }

        protected MySqlBaseLexer(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
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
            return Charsets.Contains(text) ? MySqlLexerInternal.UNDERSCORE_CHARSET : MySqlLexerInternal.IDENTIFIER;
        }

        // MySQLBaseLexer::emitDot
        protected void emitDot()
        {
            var source = new Tuple<ITokenSource, ICharStream>(this, (ICharStream)InputStream);

            var dotToken = TokenFactory.Create(
                source,
                MySqlLexerInternal.DOT_SYMBOL,
                ".",
                Channel,
                TokenStartCharIndex,
                TokenStartCharIndex,
                TokenStartLine,
                TokenStartColumn);

            var identifierToken = TokenFactory.Create(
                source,
                MySqlLexerInternal.IDENTIFIER,
                Text[1..],
                Channel,
                TokenStartCharIndex + 1,
                TokenStartCharIndex + Text.Length - 1,
                TokenStartLine,
                TokenStartColumn);

            _pendingTokens.Enqueue(dotToken);
            _pendingTokens.Enqueue(identifierToken);

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
                    Type = MySqlLexerInternal.WHITESPACE;
                    input = InputStream.LA(1);
                }
            }

            return InputStream.LA(1) == '(' ? proposed : MySqlLexerInternal.IDENTIFIER;
        }

        // MySQLBaseLexer::determineNumericType
        protected static int determineNumericType(string text)
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

            int length = text.Length;

            if (length < long_len)
                return MySqlLexerInternal.INT_NUMBER;

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
                return MySqlLexerInternal.INT_NUMBER;

            int smaller;
            int bigger;
            string cmp;

            if (negative)
            {
                switch (length)
                {
                    case long_len:
                        cmp = signed_long_str[1..];
                        smaller = MySqlLexerInternal.INT_NUMBER;
                        bigger = MySqlLexerInternal.LONG_NUMBER;
                        break;

                    case < signed_longlong_len:
                        return MySqlLexerInternal.LONG_NUMBER;

                    case > signed_longlong_len:
                        return MySqlLexerInternal.DECIMAL_NUMBER;

                    default:
                        cmp = signed_longlong_str[1..];
                        smaller = MySqlLexerInternal.LONG_NUMBER;
                        bigger = MySqlLexerInternal.DECIMAL_NUMBER;
                        break;
                }
            }
            else
            {
                switch (length)
                {
                    case long_len:
                        cmp = long_str;
                        smaller = MySqlLexerInternal.INT_NUMBER;
                        bigger = MySqlLexerInternal.LONG_NUMBER;
                        break;

                    case < longlong_len:
                        return MySqlLexerInternal.LONG_NUMBER;

                    case > longlong_len when length > unsigned_longlong_len:
                        return MySqlLexerInternal.DECIMAL_NUMBER;

                    case > longlong_len:
                        cmp = unsigned_longlong_str;
                        smaller = MySqlLexerInternal.ULONGLONG_NUMBER;
                        bigger = MySqlLexerInternal.DECIMAL_NUMBER;
                        break;

                    default:
                        cmp = longlong_str;
                        smaller = MySqlLexerInternal.LONG_NUMBER;
                        bigger = MySqlLexerInternal.ULONGLONG_NUMBER;
                        break;
                }
            }

            int cmpIndex = 0;

            while (cmpIndex < cmp.Length && cmp[cmpIndex++] == text[index++])
            {
            }

            return text[index - 1] <= cmp[cmpIndex - 1] ? smaller : bigger;
        }

        // MySQLBaseLexer::keywordFromText
        public int keywordFromText(string name)
        {
            if (!MySqlSymbolInfo.isKeyword(name, _mySqlVersion))
                return -2; // INVALID_INDEX - 1

            if (_symbols == null)
            {
                var max = ((Vocabulary)Vocabulary).getMaxTokenType();
                _symbols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                
                for (int i = 0; i <= max; i++)
                {
                    var key = Vocabulary.GetSymbolicName(i);

                    if (string.IsNullOrEmpty(key))
                        continue;

                    _symbols[key] = i;
                }
            }

            if (_symbols.TryGetValue(name, out var type))
                return type;

            return -2; // INVALID_INDEX - 1
        }
        #endregion

        protected bool IsDotIdentifier()
        {
            var ch = (char)InputStream.LA(-2);

            if (IsUnquotedLetter(ch) || ch is '`' or '"')
                return true;

            return IsUnquotedNoDigitLetter((char)InputStream.LA(1));
        }

        private static bool IsDigit(char c)
        {
            return c is >= '0' and <= '9';
        }

        private static bool IsUnquotedLetter(char c)
        {
            // 0-9a-zA-Z\u0080-\uffff_$
            return IsDigit(c) || IsUnquotedNoDigitLetter(c);
        }

        private static bool IsUnquotedNoDigitLetter(char c)
        {
            // a-zA-Z\u0080-\uffff_$
            return c is
                (>= 'a' and <= 'z') or
                (>= 'A' and <= 'Z') or
                (>= '\u0080' and <= '\uffff') or
                '_' or
                '$';
        }
    }
}
