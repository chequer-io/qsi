﻿namespace Qsi.Parsing.Common.Rules
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
                cursor.Index = cursor.Length - 1;
            }
            else
            {
                cursor.Index = index;
            }

            return true;
        }
    }
}