namespace Qsi.Oracle.Internal
{
    internal partial class OracleLexerInternal
    {
        public OracleDialect Dialect { get; } = new();

        protected bool isValidDelimiter()
        {
            return InputStream.LA(1) == Text[2] && (InputStream.LA(2) == '\'');
        }

        protected void CategorizeIdentifier()
        {
            Type = Dialect.KeywordMap.TryGetValue(Text.ToLower(), out var keywordId) 
                ? keywordId 
                : IDENTIFIER;
        }
    }
}
