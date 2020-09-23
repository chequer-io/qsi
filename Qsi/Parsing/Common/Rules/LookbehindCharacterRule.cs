﻿namespace Qsi.Parsing.Common.Rules
{
    // ..chars|..
    //        ^
    public sealed class LookbehindCharacterRule : ITokenRule
    {
        private readonly char _char;
        private readonly string _chars;
        private readonly bool _single;

        public LookbehindCharacterRule(params char[] chars)
        {
            _char = chars[0];
            _single = chars.Length == 1;

            if (!_single)
                _chars = new string(chars);
        }

        public bool Run(CommonScriptCursor cursor)
        {
            int index = -1;

            if (_single)
            {
                index = cursor.Value.IndexOf(_char, cursor.Index);
            }
            else
            {
                for (int i = cursor.Index; i < cursor.Length; i++)
                {
                    if (_chars.IndexOf(cursor.Value[i]) == -1)
                        continue;

                    index = i;
                    break;
                }
            }

            if (index == -1)
                return false;

            cursor.Index = index;
            return true;
        }
    }
}