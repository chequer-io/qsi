using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Qsi.Parsing;
using Qsi.Shared;
using SqlParserSymbols = Qsi.Impala.Internal.ImpalaLexerInternal;

namespace Qsi.Impala.Internal
{
    internal abstract class ImpalaBaseParser : Parser
    {
        protected ImpalaBaseLexer Lexer => (ImpalaBaseLexer)TokenStream.TokenSource;

        protected ImpalaBaseParser(ITokenStream input) : base(input)
        {
        }

        protected ImpalaBaseParser(ITokenStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }

        // to avoid reporting trivial tokens as expected tokens in error messages
        protected bool ReportExpectedToken(int tokenId, int numExpectedTokens)
        {
            if (Lexer.IsKeyword(tokenId) || tokenId is ImpalaLexerInternal.COMMA or ImpalaLexerInternal.IDENT)
                return true;

            // if this is the only valid token, always report it
            return numExpectedTokens == 1;
        }

        protected string GetErrorTypeMessage(int lastTokenId)
        {
            return lastTokenId switch
            {
                SqlParserSymbols.UNMATCHED_STRING_LITERAL => "Unmatched string literal",
                SqlParserSymbols.NUMERIC_OVERFLOW => "Numeric overflow",
                _ => "Syntax error"
            };
        }

        public override void NotifyErrorListeners(IToken offendingToken, string msg, RecognitionException e)
        {
            SyntaxError(offendingToken, null);
        }

        protected void ParseError(string symbolName, int symbolId)
        {
            ParseError(symbolName, symbolId, null);
        }

        protected void ParseError(string symbolName, int symbolId, string expectedTokenName)
        {
            SyntaxError(CurrentToken, expectedTokenName);
        }

        private void SyntaxError(IToken token, string expectedTokenName)
        {
            var stmt = ((StringInputStream)Lexer.InputStream).Input;
            var tokens = GetNextTokens(CurrentToken).ToArray();

            throw new QsiSyntaxErrorException(
                token.Line,
                token.Column,
                GetErrorMessage(token, stmt, expectedTokenName, tokens)
            );
        }

        private IEnumerable<int> GetNextTokens(IToken token)
        {
            var seen = new HashSet<ATNState>();
            return NextTokens(Atn, token, Atn.states[State], seen);

            static IEnumerable<int> NextTokens(ATN atn, IToken token, ATNState state, ISet<ATNState> seen)
            {
                if (!seen.Add(state))
                    yield break;

                foreach (var transition in state.TransitionsArray)
                {
                    if (transition is RuleTransition ruleTransition)
                    {
                        foreach (var t in NextTokens(atn, token, atn.ruleToStartState[ruleTransition.ruleIndex], seen))
                            yield return t;
                    }
                    else if (transition.IsEpsilon)
                    {
                        foreach (var t in NextTokens(atn, token, transition.target, seen))
                            yield return t;
                    }
                    else if (transition.Label != null)
                    {
                        foreach (var t in transition.Label.ToArray())
                            yield return t;
                    }
                }
            }
        }

        protected string GetErrorMessage(IToken errorToken, string stmt, string expectedTokenName, int[] expectedTokenIds)
        {
            if (errorToken == null || stmt == null)
                return null;

            var result = new StringBuilder();
            result.Append(GetErrorTypeMessage(errorToken.Type) + " in line ");
            result.Append($"{errorToken.Column}:{errorToken.Line}\n");

            var errorLine = stmt.Split('\n')[errorToken.Line - 1];

            // If the error is that additional tokens are expected past the end,
            // errorToken_.right will be past the end of the string.
            var tokenLength = Math.Max(0, errorToken.StopIndex - errorToken.StartIndex) + 1;
            var tokenRight = errorToken.Column + tokenLength;
            int lastCharIndex = Math.Min(errorLine.Length, tokenRight);
            const int maxPrintLength = 60;
            int errorLoc;

            if (errorLine.Length <= maxPrintLength)
            {
                // The line is short. Print the entire line.
                result.Append(errorLine);
                result.Append('\n');
                errorLoc = tokenRight;
            }
            else
            {
                // The line is too long. Print maxPrintLength/2 characters before the error and
                // after the error.
                const int contextLength = maxPrintLength / 2 - 3;
                string leftSubStr;

                if (tokenRight > maxPrintLength / 2)
                {
                    leftSubStr = "..." + errorLine[(tokenRight - contextLength)..lastCharIndex];
                }
                else
                {
                    leftSubStr = errorLine[..tokenRight];
                }

                errorLoc = leftSubStr.Length;
                result.Append(leftSubStr);

                if (errorLine.Length - tokenRight > maxPrintLength / 2)
                {
                    result.Append(errorLine[tokenRight..(tokenRight + contextLength)] + "...");
                }
                else
                {
                    result.Append(errorLine[lastCharIndex..]);
                }

                result.Append('\n');
            }

            // print error indicator
            for (int i = 0; i < errorLoc - 1; i++)
                result.Append(' ');

            result.Append("^\n");

            // only report encountered and expected tokens for syntax errors
            if (errorToken.Type
                is SqlParserSymbols.UNMATCHED_STRING_LITERAL
                or SqlParserSymbols.NUMERIC_OVERFLOW)
            {
                return result.ToString();
            }

            // append last encountered token
            result.Append("Encountered: ");

            if (Lexer.TokenIdMap.TryGetValue(errorToken.Type, out var lastToken))
            {
                result.Append(lastToken);
            }
            else if (Lexer.IsReserved(errorToken.Text))
            {
                result
                    .Append("A reserved word cannot be used as an identifier: ")
                    .Append(errorToken.Text);
            }
            else
            {
                result.Append("Unknown last token with id: " + errorToken.Type);
            }

            // append expected tokens
            result.Append('\n');
            result.Append("Expected: ");

            if (expectedTokenName == null)
            {
                IEnumerable<string> tokenNames = expectedTokenIds
                    .Where(x => ReportExpectedToken(x, expectedTokenIds.Length))
                    .Select(x => Lexer.TokenIdMap[x]);

                result.AppendJoin(", ", tokenNames);
            }
            else
            {
                result.Append(expectedTokenName);
            }

            result.Append('\n');

            return result.ToString();
        }
    }
}
