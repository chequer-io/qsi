using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Qsi.Data;
using Qsi.MySql.Internal;
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

        public bool UseDelimiter { get; set; } = true;

        private readonly int _version;

        public MySqlScriptParser(Version version)
        {
            _version = MySQLUtility.VersionToInt(version);
        }

        public override IEnumerable<QsiScript> Parse(string input, CancellationToken cancellationToken)
        {
            if (!UseDelimiter)
            {
                var collector = new ScriptCollector(this, input);
                var parser = MySQLUtility.CreateParser(input, _version);
                parser.BuildParseTree = false;
                parser.TrimParseTree = false;
                parser.AddParseListener(collector);
                parser.query();

                return collector.Scripts;
            }

            return base.Parse(input, cancellationToken);
        }

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

        private class ScriptCollector : IParseTreeListener
        {
            public List<QsiScript> Scripts { get; }

            private readonly MySqlScriptParser _parser;
            private readonly CommonScriptCursor _cursor;
            private readonly string _input;
            private int _depth;

            public ScriptCollector(MySqlScriptParser parser, string input)
            {
                Scripts = new List<QsiScript>();
                _parser = parser;
                _cursor = new CommonScriptCursor(string.Empty);
                _input = input;
            }

            public void VisitTerminal(ITerminalNode node)
            {
            }

            public void VisitErrorNode(IErrorNode node)
            {
            }

            public void EnterEveryRule(ParserRuleContext ctx)
            {
                if (ctx is not MySqlParserInternal.SimpleStatementContext &&
                    ctx is not MySqlParserInternal.BeginWorkContext)
                {
                    return;
                }

                _depth++;
            }

            public void ExitEveryRule(ParserRuleContext ctx)
            {
                if (ctx is not MySqlParserInternal.SimpleStatementContext &&
                    ctx is not MySqlParserInternal.BeginWorkContext)
                {
                    return;
                }

                if (--_depth == 0 && ctx.Start != null)
                {
                    var stopToken = ctx.Stop ?? ctx.Start;
                    var startIndex = ctx.Start.StartIndex;
                    var endIndex = stopToken.StopIndex;
                    var script = _input[startIndex..(endIndex + 1)];

                    _cursor.Reset(script);

                    Token[] tokens = _parser.ParseTokens(_cursor)
                        .Where(t => !TokenType.Trivia.HasFlag(t.Type))
                        .TakeWhile(t => t.Type == TokenType.Keyword)
                        .Take(2)
                        .ToArray();

                    var scriptType = _parser.GetSuitableType(_cursor, tokens, tokens);
                    var start = new QsiScriptPosition(ctx.Start.Line, ctx.Start.Column + 1);
                    var end = new QsiScriptPosition(stopToken.Line, stopToken.Column + 1);

                    Scripts.Add(new QsiScript(script, scriptType, start, end));
                }
            }
        }
    }
}
