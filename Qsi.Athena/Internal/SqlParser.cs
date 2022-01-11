using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Qsi.Shared.Antlr4;

namespace Qsi.Athena.Internal
{
    internal static class SqlParser
    {
        private static readonly IAntlrErrorListener<int> LexerErrorListener = new LexerErrorListener();

        private static readonly ErrorHandler ParserErrorHandler = ErrorHandler.CreateBuilder()
            .SpecialRule(SqlBaseParser.RULE_expression, "<expression>")
            .SpecialRule(SqlBaseParser.RULE_booleanExpression, "<expression>")
            .SpecialRule(SqlBaseParser.RULE_valueExpression, "<expression>")
            .SpecialRule(SqlBaseParser.RULE_primaryExpression, "<expression>")
            .SpecialRule(SqlBaseParser.RULE_identifier, "<identifier>")
            .SpecialRule(SqlBaseParser.RULE_string, "<string>")
            .SpecialRule(SqlBaseParser.RULE_query, "<query>")
            .SpecialRule(SqlBaseParser.RULE_dataType, "<type>")
            .SpecialToken(SqlBaseLexer.INTEGER_VALUE, "<integer>")
            .IgnoredRule(SqlBaseParser.RULE_nonReserved)
            .Build();

        public static (SqlBaseParser Parser, T Result) Parse<T>(string sql, Func<SqlBaseParser, T> parse) where T : ParserRuleContext
        {
            try
            {
                var lexer = new SqlBaseLexer(new CaseInsensitiveStream(new AntlrInputStream(sql)));
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new SqlBaseParser(tokenStream);

                parser.AddParseListener(new PostProcessor(parser));

                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(LexerErrorListener);

                parser.RemoveErrorListeners();
                parser.AddErrorListener(ParserErrorHandler);

                try
                {
                    // first, try parsing with potentially faster SLL mode
                    parser.Interpreter.PredictionMode = PredictionMode.SLL;
                    return (parser, parse?.Invoke(parser));
                }
                catch (ParseCanceledException)
                {
                    // if we fail, parse with LL mode
                    tokenStream.Seek(0); // rewind input stream
                    parser.Reset();

                    parser.Interpreter.PredictionMode = PredictionMode.LL;
                    return (parser, parse?.Invoke(parser));
                }
            }
            catch (StackOverflowException)
            {
                throw new ParsingException("Statement is too large (stack overflow while parsing)");
            }
        }

        private class PostProcessor : SqlBaseBaseListener
        {
            private readonly SqlBaseParser _parser;

            public PostProcessor(SqlBaseParser parser)
            {
                _parser = parser;
            }

            public override void ExitQuotedIdentifier(SqlBaseParser.QuotedIdentifierContext context)
            {
                var token = context.QUOTED_IDENTIFIER().Symbol;

                if (token.Text.Length == 2)
                {
                    throw new ParsingException(
                        "Zero-length delimited identifier not allowed",
                        token.Line,
                        token.Column + 1
                    );
                }
            }

            public override void ExitBackQuotedIdentifier(SqlBaseParser.BackQuotedIdentifierContext context)
            {
                var token = context.BACKQUOTED_IDENTIFIER().Symbol;

                if (token.Text.Length == 2)
                {
                    throw new ParsingException(
                        "Zero-length delimited identifier not allowed",
                        token.Line,
                        token.Column + 1
                    );
                }
            }

            public override void ExitDigitIdentifier(SqlBaseParser.DigitIdentifierContext context)
            {
                var token = context.DIGIT_IDENTIFIER().Symbol;

                throw new ParsingException(
                    "identifiers must not start with a digit; surround the identifier with double or back quotes",
                    token.Line,
                    token.Column + 1
                );
            }

            public override void ExitNonReserved(SqlBaseParser.NonReservedContext context)
            {
                // we can't modify the tree during rule enter/exit event handling unless we're dealing with a terminal.
                // Otherwise, ANTLR gets confused and fires spurious notifications.
                if (context.children[0] is ParserRuleContext parserRuleContext)
                {
                    int rule = parserRuleContext.RuleIndex;
                    throw new ParsingException($"nonReserved can only contain tokens. Found nested rule: {_parser.RuleNames[rule]}");
                }

                // replace nonReserved words with IDENT tokens
                if (context.Parent is not ParserRuleContext parent)
                    throw new QsiException(QsiError.Syntax);

                parent.RemoveLastChild();

                var token = (IToken)context.children[0].Payload;

                var newToken = new CommonToken(
                    new Tuple<ITokenSource, ICharStream>(token.TokenSource, token.InputStream),
                    SqlBaseLexer.IDENTIFIER,
                    token.Channel,
                    token.StartIndex,
                    token.StopIndex
                );

                parent.AddChild(new TerminalNodeImpl(newToken));
            }
        }
    }
}
