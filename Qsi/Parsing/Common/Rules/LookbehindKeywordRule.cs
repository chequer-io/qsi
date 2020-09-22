﻿namespace Qsi.Parsing.Common.Rules
{
    // ..Keyword|..
    //          ^
    public sealed class LookbehindKeywordRule : ITokenRule
    {
        private readonly string _keyword;

        public LookbehindKeywordRule(string keyword)
        {
            _keyword = keyword;
        }

        public void Run(CommonScriptCursor cursor)
        {
            for (int i = cursor.Index; i < cursor.Length; i++)
            {
                if (!cursor.StartsWith(_keyword, i))
                    continue;

                cursor.Index = i + _keyword.Length - 1;
                return;
            }

            cursor.Index = cursor.Length - 1;
        }
    }
}