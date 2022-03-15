using System.Collections.Generic;
using Qsi.Data;
using Qsi.PostgreSql;
using static Qsi.Shared.Extensions.StringExtension;

namespace Qsi.Redshift;

public class RedshiftScriptParser : PostgreSqlScriptParser
{
    protected override QsiScript CreateScript(ParseContext context, IReadOnlyList<Token> tokens)
    {
        string input = context.Cursor.Value;
        var startIndex = tokens[0].Span.Start.GetOffset(context.Cursor.Length);
        var endIndex = tokens[^1].Span.End.GetOffset(context.Cursor.Length) - 1;
        var (startPosition, endPosition) = MeasurePosition(context, startIndex, endIndex);
        var script = input[startIndex..(endIndex + 1)];

        Token[] leadingTokens = GetLeadingTokens(input, tokens, TokenType.Keyword, 2);
        var scriptType = GetSuitableType(context.Cursor, tokens, leadingTokens);

        if (scriptType is QsiScriptType.Delete &&
            (leadingTokens.Length != 2 || !input[leadingTokens[1].Span].EqualsIgnoreCase("FROM")))
        {
            script = $"{input[startIndex..leadingTokens[0].Span.End]} FROM{input[leadingTokens[0].Span.End..(endIndex + 1)]}";
        }

        return new QsiScript(script, scriptType, startPosition, endPosition);
    }
}
