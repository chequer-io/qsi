namespace Qsi.Parsing.Common.Rules;

// ..|LB..
//   ^
public sealed class LookaheadLineBreakRule : ITokenRule
{
    private static readonly char[] _breakChars = { '\n', '\r', '\0' };

    public bool Run(CommonScriptCursor cursor)
    {
        int count = cursor.Length - cursor.Index;
        int breakIndex = -1;

        foreach (var breakChar in _breakChars)
        {
            int index = cursor.Value.IndexOf(breakChar, cursor.Index, count);

            if (index == -1)
                continue;

            breakIndex = index;
            count = breakIndex - cursor.Index + 1;
        }

        if (breakIndex == -1)
            cursor.Index = cursor.Length - 1;
        else
            cursor.Index = breakIndex - 1;

        return true;
    }
}