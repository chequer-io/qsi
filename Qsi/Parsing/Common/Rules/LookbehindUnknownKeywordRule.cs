﻿namespace Qsi.Parsing.Common.Rules
{
    public sealed class LookbehindUnknownKeywordRule : ITokenRule
    {
        public bool Run(CommonScriptCursor cursor)
        {
            for (int i = cursor.Index; i < cursor.Length; i++)
            {
                var c = cursor.Value[i];

                if ('A' <= c && c <= 'Z' || 'a' <= c && c <= 'z' || '0' <= c && c <= '9' ||c == '_')
                    continue;

                cursor.Index = i - 1;
                return true;
            }

            cursor.Index = cursor.Length - 1;
            return true;
        }
    }
}