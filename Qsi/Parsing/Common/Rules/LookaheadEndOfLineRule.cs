namespace Qsi.Parsing.Common.Rules
{
    // ..|EOL..
    //   ^
    public sealed class LookaheadEndOfLineRule : ITokenRule
    {
        public bool Run(CommonScriptCursor cursor)
        {
            int index = cursor.Value.IndexOf('\n', cursor.Index);

            if (index == -1)
            {
                index = cursor.Value.IndexOf('\r', cursor.Index);
            }
            else
            {
                int index2 = cursor.Value.IndexOf('\r', cursor.Index, index - cursor.Index + 1);

                if (index2 >= 0 && index2 < index)
                    index = index2;
            }

            if (index == -1)
                cursor.Index = cursor.Length - 1;
            else
                cursor.Index = index - 1;

            return true;
        }
    }
}
