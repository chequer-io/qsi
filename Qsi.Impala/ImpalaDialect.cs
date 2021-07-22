using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Qsi.Impala.Internal;

namespace Qsi.Impala
{
    public abstract class ImpalaDialect
    {
        // Map from keyword string to token id.
        // We use a linked hash map because the insertion order is important.
        // for example, we want "and" to come after "&&" to make sure error reporting
        // uses "and" as a display name and not "&&".
        // Please keep the puts sorted alphabetically by keyword (where the order
        // does not affect the desired error reporting)
        internal Dictionary<string, int> KeywordMap { get; }

        // Reserved words are words that cannot be used as identifiers. It is a superset of
        // keywords.
        internal HashSet<string> ReservedWords { get; }

        // map from token id to token description
        internal Dictionary<int, string> TokenIdMap { get; }

        public ImpalaDialect()
        {
            // initilize keywords
            KeywordMap = LoadKeywordMap();

            // Initilize tokenIdMap for error reporting
            TokenIdMap = new Dictionary<int, string>();

            foreach (var (key, value) in KeywordMap)
                TokenIdMap[value] = key;

            // add non-keyword tokens. Please keep this in the same order as they are used in this
            // file.
            TokenIdMap[Lexer.Eof] = "EOF";
            TokenIdMap[ImpalaLexerInternal.DOTDOTDOT] = "...";
            TokenIdMap[ImpalaLexerInternal.COLON] = ":";
            TokenIdMap[ImpalaLexerInternal.SEMICOLON] = ";";
            TokenIdMap[ImpalaLexerInternal.COMMA] = "COMMA";
            TokenIdMap[ImpalaLexerInternal.DOT] = ".";
            TokenIdMap[ImpalaLexerInternal.STAR] = "*";
            TokenIdMap[ImpalaLexerInternal.LPAREN] = "(";
            TokenIdMap[ImpalaLexerInternal.RPAREN] = ")";
            TokenIdMap[ImpalaLexerInternal.LBRACKET] = "[";
            TokenIdMap[ImpalaLexerInternal.RBRACKET] = "]";
            TokenIdMap[ImpalaLexerInternal.DIVIDE] = "/";
            TokenIdMap[ImpalaLexerInternal.MOD] = "%";
            TokenIdMap[ImpalaLexerInternal.ADD] = "+";
            TokenIdMap[ImpalaLexerInternal.SUBTRACT] = "-";
            TokenIdMap[ImpalaLexerInternal.BITAND] = "&";
            TokenIdMap[ImpalaLexerInternal.BITOR] = "|";
            TokenIdMap[ImpalaLexerInternal.BITXOR] = "^";
            TokenIdMap[ImpalaLexerInternal.BITNOT] = "~";
            TokenIdMap[ImpalaLexerInternal.EQUAL] = "=";
            TokenIdMap[ImpalaLexerInternal.NOT] = "!";
            TokenIdMap[ImpalaLexerInternal.LESSTHAN] = "<";
            TokenIdMap[ImpalaLexerInternal.GREATERTHAN] = ">";
            TokenIdMap[ImpalaLexerInternal.UNMATCHED_STRING_LITERAL] = "UNMATCHED STRING LITERAL";
            TokenIdMap[ImpalaLexerInternal.NOTEQUAL] = "!=";
            TokenIdMap[ImpalaLexerInternal.INTEGER_LITERAL] = "INTEGER LITERAL";
            TokenIdMap[ImpalaLexerInternal.NUMERIC_OVERFLOW] = "NUMERIC OVERFLOW";
            TokenIdMap[ImpalaLexerInternal.DECIMAL_LITERAL] = "DECIMAL LITERAL";
            TokenIdMap[ImpalaLexerInternal.EMPTY_IDENT] = "EMPTY IDENTIFIER";
            TokenIdMap[ImpalaLexerInternal.IDENT] = "IDENTIFIER";
            TokenIdMap[ImpalaLexerInternal.STRING_LITERAL] = "STRING LITERAL";
            TokenIdMap[ImpalaLexerInternal.COMMENTED_PLAN_HINT_START] = "COMMENTED_PLAN_HINT_START";
            TokenIdMap[ImpalaLexerInternal.COMMENTED_PLAN_HINT_END] = "COMMENTED_PLAN_HINT_END";
            TokenIdMap[ImpalaLexerInternal.UNEXPECTED_CHAR] = "Unexpected character";

            ReservedWords = new HashSet<string>(KeywordMap.Select(kv => kv.Key));
        }

        protected abstract Dictionary<string, int> LoadKeywordMap();

        public void SetBuiltInFunctions(IEnumerable<string> builtInFunctions)
        {
            // Remove impala builtin function names
            foreach (var builtInFunction in builtInFunctions)
            {
                ReservedWords.Remove(builtInFunction);
            }
        }

        public bool IsReserved(string value)
        {
            return ReservedWords.Contains(value);
        }

        internal bool IsKeyword(int tokenId)
        {
            if (!TokenIdMap.TryGetValue(tokenId, out var token))
                return false;

            return KeywordMap.ContainsKey(token.ToLower());
        }

        public bool IsKeyword(string value)
        {
            if (KeywordMap.TryGetValue(value.ToLower(), out var keywordId))
                return keywordId != ImpalaLexerInternal.IDENT;

            return false;
        }
    }
}
