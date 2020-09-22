namespace Qsi.Parsing.Common.Rules
{
    // ABCabc|..
    //       ^
    public sealed class LookbehindUnknownKeywordRule : ITokenRule
    {
        public void Run(CommonScriptCursor cursor)
        {
            for (int i = cursor.Index; i < cursor.Length; i++)
            {
                var c = cursor.Value[i];

                // A-Za-z
                if (65 <= c && c <= 90 || 97 <= c && c <= 122)
                    continue;

                cursor.Index = i - 1;
                return;
            }

            cursor.Index = cursor.Length - 1;
        }
    }
}