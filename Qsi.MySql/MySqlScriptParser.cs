using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qsi.Parsing.Common;

namespace Qsi.MySql
{
    public class MySqlScriptParser : CommonScriptParser
    {
        private readonly Regex _delimiterPattern = new Regex(@"\G(\S+)\s");

        protected override bool IsEndOfScript(ParseContext context)
        {
            List<Token> tokens = context.Tokens;

            if (tokens.Count > 1 &&
                tokens[^1].Type == TokenType.WhiteSpace &&
                tokens[^2].Type == TokenType.Keyword &&
                context.Cursor.Value[tokens[^2].Span].Equals("DELIMITER", StringComparison.OrdinalIgnoreCase) &&
                tokens.SkipLast(2).All(t => t.Type != TokenType.Keyword && t.Type != TokenType.Fragment))
            {
                var match = _delimiterPattern.Match(context.Cursor.Value, context.Cursor.Index);

                if (match.Success)
                {
                    var delimiter = match.Groups[1];

                    context.Cursor.Index = delimiter.Index + delimiter.Length - 1;
                    context.Delimiter = delimiter.Value;
                    context.Tokens.Add(new Token(TokenType.Fragment, delimiter.Index..(context.Cursor.Index + 1)));

                    return true;
                }
            }

            return base.IsEndOfScript(context);
        }
    }
}
