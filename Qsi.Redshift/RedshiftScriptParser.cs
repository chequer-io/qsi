using System.Collections.Generic;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.PostgreSql;
using static Qsi.Shared.Extensions.StringExtension;

namespace Qsi.Redshift;

public class RedshiftScriptParser : PostgreSqlScriptParser
{
    public QsiScript ToPostgreSqlScript(QsiScript script)
    {
        switch (script.ScriptType)
        {
            // DELETE actor;
            // DELETE FROM actor;
            case QsiScriptType.Delete:
            {
                var sql = script.Script;

                IEnumerable<Token> tokens = ParseTokens(new CommonScriptCursor(script.Script));
                Token[] leadingTokens = GetLeadingTokens(script.Script, tokens, TokenType.Keyword, 2);

                if (leadingTokens.Length != 2 || !sql[leadingTokens[1].Span].EqualsIgnoreCase("FROM"))
                    return new QsiScript(
                        $"{sql[..leadingTokens[0].Span.End]} FROM{sql[leadingTokens[0].Span.End..]}",
                        script.ScriptType
                    );

                break;
            }
        }

        return script;
    }
}
