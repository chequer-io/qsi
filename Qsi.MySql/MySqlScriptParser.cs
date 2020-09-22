using System.Linq;
using System.Text.RegularExpressions;
using Qsi.Parsing.Common;

namespace Qsi.MySql
{
    public class MySqlScriptParser : CommonScriptParser
    {
        private readonly Regex _delimiterPattern = new Regex(@"\GR\s+(\S+)\s", RegexOptions.IgnoreCase);

        protected override bool IsEndOfScript(ParseContext context)
        {
            if (context.Cursor.StartsWithIgnoreCase("DELIMITER") && context.Tokens.All(t => t.Type != TokenType.Fragment))
            {
                var match = _delimiterPattern.Match(context.Cursor.Value, context.Cursor.Index + 8);

                if (match.Success)
                {
                    int start = context.Cursor.Index;
                    var delimiter = match.Groups[1];

                    context.Cursor.Index = delimiter.Index + delimiter.Length - 1;
                    context.Delimiter = delimiter.Value;
                    context.Tokens.Add(new Token(TokenType.Fragment, start..(context.Cursor.Index + 1)));
                    return true;
                }
            }

            return base.IsEndOfScript(context);
        }
    }
}
