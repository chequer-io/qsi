﻿using System;

namespace Qsi.Parsing.Common.Rules
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

        public bool Run(CommonScriptCursor cursor)
        {
            int index = cursor.Value.IndexOf(_keyword, cursor.Index, StringComparison.Ordinal);

            if (index == -1)
                return false;

            cursor.Index = index + _keyword.Length - 1;
            return true;
        }
    }
}