namespace Qsi.Parsing.Common.Rules;

// {space}*|..
//         ^
public sealed class LookbehindWhiteSpaceRule : ITokenRule
{
    public bool Run(CommonScriptCursor cursor)
    {
        for (int i = cursor.Index; i < cursor.Length; i++)
        {
            if (char.IsWhiteSpace(cursor.Value[i]))
                continue;

            cursor.Index = i - 1;
            return true;
        }

        cursor.Index = cursor.Length - 1;
        return true;
    }
}