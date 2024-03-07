using System.Collections.Generic;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Shared.Extensions;

namespace Qsi.PostgreSql;

public class PostgreSqlScriptParser : CommonScriptParser
{
    protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IEnumerable<Token> tokens, Token[] leadingTokens)
    {
        return leadingTokens.Length switch
        {
            >= 1 when "TABLE".EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]) => QsiScriptType.Select,
            _ => base.GetSuitableType(cursor, tokens, leadingTokens)
        };
    }
}