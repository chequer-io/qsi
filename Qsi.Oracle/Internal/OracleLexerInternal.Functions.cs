namespace Qsi.Oracle.Internal
{
    internal partial class OracleLexerInternal
    {
        protected LexHint Hint { get; set; }

        protected bool IsValidDelimiter()
        {
            return InputStream.LA(1) == Text[2] && (InputStream.LA(2) == '\'');
        }

        protected void CategorizeIdentifier()
        {
            Type = OracleDialect.KeywordMap.TryGetValue(Text, out var keywordId)
                ? keywordId
                : TK_IDENTIFIER;
        }

        protected bool IsCommentPlanHint()
        {
            return OracleUtility.IsCommentPlanHint(Text);
        }

        protected enum LexHint
        {
            Default,
            SingleLineComment,
            MultiLineComment
        }
    }
}
