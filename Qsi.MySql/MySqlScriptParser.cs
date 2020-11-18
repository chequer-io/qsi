using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qsi.Data;
using Qsi.Parsing.Common;

namespace Qsi.MySql
{
    public class MySqlScriptParser : CommonScriptParser
    {
        private const string delimiter = "DELIMITER";
        private const string deallocate = "DEALLOCATE";
        private const string prepare = "PREPARE";

        private readonly Regex _delimiterPattern = new Regex(@"\G\S+(?=\s|$)");

        protected override bool IsEndOfScript(ParseContext context)
        {
            IReadOnlyList<Token> tokens = context.Tokens;

            if (tokens.Count > 1 &&
                tokens[^1].Type == TokenType.WhiteSpace &&
                tokens[^2].Type == TokenType.Keyword &&
                delimiter.Equals(context.Cursor.Value[tokens[^2].Span], StringComparison.OrdinalIgnoreCase) &&
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
            if (leadingTokens.Length >= 2 &&
                deallocate.Equals(cursor.Value[leadingTokens[0].Span], StringComparison.OrdinalIgnoreCase) &&
                prepare.Equals(cursor.Value[leadingTokens[1].Span], StringComparison.OrdinalIgnoreCase))
            {
                return QsiScriptType.DropPrepare;
            }

            return base.GetSuitableType(cursor, tokens, leadingTokens);
        }
    }
}
