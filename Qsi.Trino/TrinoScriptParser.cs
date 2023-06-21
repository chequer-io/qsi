using System.Collections.Generic;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Shared.Extensions;

namespace Qsi.Trino;

public class TrinoScriptParser : CommonScriptParser
{
    private const string table = "TABLE";
    private const string values = "VALUES";

    protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IEnumerable<Token> tokens, Token[] leadingTokens)
    {
        return leadingTokens.Length switch
        {
            >= 1 when table.EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]) ||
                      values.EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]) => QsiScriptType.Select,
            _ => base.GetSuitableType(cursor, tokens, leadingTokens)
        };
    }
}