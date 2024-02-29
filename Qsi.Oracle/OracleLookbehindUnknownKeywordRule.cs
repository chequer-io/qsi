using Qsi.Parsing.Common;
using Qsi.Parsing.Common.Rules;

namespace Qsi.Oracle;

public class OracleLookbehindUnknownKeywordRule : ITokenRule
{
    public bool Run(CommonScriptCursor cursor)
    {
        for (int i = cursor.Index; i < cursor.Length; i++)
        {
            var c = cursor.Value[i];

            if (IsOracleIdentifier(c))
                continue;

            cursor.Index = i - 1;
            return true;
        }

        cursor.Index = cursor.Length - 1;
        return true;
    }

    public static bool IsOracleIdentifier(char ch)
    {
        return ch is >= 'a' and <= 'z' || // a-z
               ch is >= 'A' and <= 'Z' || // A-Z
               ch == '_' || // _
               ch == '$' || // $
               ch == '#' || // #
               ch >= '\u0080'; // utf8
    }
}
