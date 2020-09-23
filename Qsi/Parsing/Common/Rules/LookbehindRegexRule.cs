﻿﻿﻿using System.Text.RegularExpressions;

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

        public bool Run(CommonScriptCursor cursor)
        {
            var match = _regex.Match(cursor.Value, cursor.Index);

            if (!match.Success)
                return false;

            cursor.Index = match.Index + match.Length - 1;
            return true;
        }
    }
}