namespace Qsi.Parsing.Common.Rules
{
    // ..|[\r\n]..
    //   ^
    public sealed class LookaheadNewLineRule : ITokenRule
    {
        public void Run(CommonScriptCursor cursor)
        {
            for (int i = cursor.Index; i < cursor.Length; i++)
            {
                var c = cursor.Value[i];

                if (c != '\r' && c != '\n')
                    continue;

                cursor.Index = i - 1;
                return;
            }

            cursor.Index = cursor.Length - 1;
        }
    }
}