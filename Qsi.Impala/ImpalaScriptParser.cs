using System;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Impala.Utilities;
using Qsi.Parsing.Common;
using Qsi.Shared.Extensions;

namespace Qsi.Impala;

public class ImpalaScriptParser : CommonScriptParser
{
    public ImpalaScriptParser()
    {
        EnablePoundComment = false;
        TrimDelimiter = false;
    }

    protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IEnumerable<Token> tokens, Token[] leadingTokens)
    {
        if (leadingTokens.Length >= 1 && cursor.Value[leadingTokens[0].Span].EqualsIgnoreCase("VALUES"))
            return QsiScriptType.Select;

        return base.GetSuitableType(cursor, tokens, leadingTokens);
    }

    protected override bool TryParseToken(CommonScriptCursor cursor, out Token token)
    {
        if (!base.TryParseToken(cursor, out token))
            return false;

        if (token.Type is TokenType.MultiLineComment or TokenType.SingeLineComment)
        {
            var (offset, length) = token.Span.GetOffsetAndLength(cursor.Length);
            ReadOnlySpan<char> span = cursor.Value.AsSpan(offset, length);

            if (ImpalaUtility.IsCommentPlanHint(span))
                token = new Token(TokenType.Fragment, token.Span);
        }

        return true;
    }
}