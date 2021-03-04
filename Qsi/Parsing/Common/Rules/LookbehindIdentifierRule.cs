namespace Qsi.Parsing.Common.Rules
{
    // ..'|liter\'al'|..
    //               ^
    // ..'|liter''al'|..
    //               ^
    public sealed class LookbehindIdentifierRule : ITokenRule
    {
        private readonly char _quote;
        private readonly bool _backslashEscape;

        public LookbehindIdentifierRule(char quote, bool useBackslashEscape)
        {
            _quote = quote;
            _backslashEscape = useBackslashEscape;
        }

        public bool Run(CommonScriptCursor cursor)
        {
            int index = cursor.Index - 1;

            while ((index = cursor.Value.IndexOf(_quote, index + 1)) >= 0)
            {
                if (_backslashEscape && index > cursor.Index && cursor.Value[index - 1] == '\\')
                    continue;

                if (index + 1 < cursor.Value.Length && cursor.Value[index + 1] == _quote)
                {
                    index++;
                    continue;
                }

                break;
            }

            if (index == -1)
                return false;

            cursor.Index = index;
            return true;
        }
    }
}
