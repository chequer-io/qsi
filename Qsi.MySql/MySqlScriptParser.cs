using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Shared.Extensions;

namespace Qsi.MySql
{
    public class MySqlScriptParser : CommonScriptParser
    {
        private const string table = "TABLE";
        private const string delimiter = "DELIMITER";
        private const string deallocate = "DEALLOCATE";
        private const string prepare = "PREPARE";

        private readonly Regex _delimiterPattern = new(@"\G\S+(?=\s|$)");

        protected override bool IsEndOfScript(ParseContext context)
        {
            IReadOnlyList<Token> tokens = context.Tokens;

            if (tokens.Count > 1 &&
                tokens[^1].Type == TokenType.WhiteSpace &&
                tokens[^2].Type == TokenType.Keyword &&
                delimiter.EqualsIgnoreCase(context.Cursor.Value[tokens[^2].Span]) &&
                tokens.SkipLast(2).All(t => TokenType.Trivia.HasFlag(t.Type)))
            {
                var match = _delimiterPattern.Match(context.Cursor.Value, context.Cursor.Index);

                if (match.Success)
                {
                    context.Cursor.Index = match.Index + match.Length - 1;
                    context.Delimiter = match.Value;
                    context.AddToken(new Token(TokenType.Fragment, match.Index..(context.Cursor.Index + 1)));

                    return true;
                }
            }

            return base.IsEndOfScript(context);
        }

        protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IReadOnlyList<Token> tokens, Token[] leadingTokens)
        {
            return leadingTokens.Length switch
            {
                >= 1 when table.EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]) => QsiScriptType.Select,
                >= 2 when deallocate.EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]) &&
                          prepare.EqualsIgnoreCase(cursor.Value[leadingTokens[1].Span]) => QsiScriptType.DropPrepare,
                _ => base.GetSuitableType(cursor, tokens, leadingTokens)
            };
        }
    }
}
