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

        public void Run(CommonScriptCursor cursor)
        {
            if (_single)
            {
                for (int i = cursor.Index; i < cursor.Length; i++)
                {
                    if (cursor.Value[i] != _char)
                        continue;

                    cursor.Index = i;
                    return;
                }
            }
            else
            {
                for (int i = cursor.Index; i < cursor.Length; i++)
                {
                    if (_chars.IndexOf(cursor.Value[i]) == -1)
                    {
                        continue;
                    }

                    cursor.Index = i;
                    return;
                }
            }

            cursor.Index = cursor.Length - 1;
        }
    }
}