﻿using System.Text.RegularExpressions;

namespace Qsi.Parsing.Common.Rules
{
    // ..Pattern|..
    //          ^
    public sealed class LookbehindRegexRule : ITokenRule
    {
        private readonly Regex _regex;

        public LookbehindRegexRule(Regex regex)
        {
            _regex = regex;
        }

        public void Run(CommonScriptCursor cursor)
        {
            for (int i = cursor.Index; i < cursor.Length; i++)
            {
                var match = _regex.Match(cursor.Value, i);

                if (!match.Success)
                    continue;

                cursor.Index = i + match.Length - 1;
                return;
            }

            cursor.Index = cursor.Length - 1;
        }
    }
}