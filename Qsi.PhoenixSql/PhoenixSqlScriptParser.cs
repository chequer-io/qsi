using System;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Parsing.Common;

namespace Qsi.PhoenixSql
{
    public sealed class PhoenixSqlScriptParser : CommonScriptParser
    {
        private const string upsert = "UPSERT";

        protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IReadOnlyList<Token> tokens, Token[] leadingTokens)
        {
            if (upsert.Equals(cursor.Value[leadingTokens[0].Span], StringComparison.OrdinalIgnoreCase))
                return QsiScriptType.Insert;

            return base.GetSuitableType(cursor, tokens, leadingTokens);
        }
    }
}
