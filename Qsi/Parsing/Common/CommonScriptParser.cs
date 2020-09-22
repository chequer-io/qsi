using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Qsi.Data;
using Qsi.Parsing.Common.Rules;

namespace Qsi.Parsing.Common
{
    public class CommonScriptParser : IQsiScriptParser
    {
        public string Delimiter { get; set; } = ";";

        private readonly ITokenRule _whiteSpace = new LookbehindWhiteSpaceRule();
        private readonly ITokenRule _newLine = new LookaheadNewLineRule();
        private readonly ITokenRule _singleQuote = new LookbehindCharacterRule('\'');
        private readonly ITokenRule _doubleQuote = new LookbehindCharacterRule('"');
        private readonly ITokenRule _backQuote = new LookbehindCharacterRule('`');
        private readonly ITokenRule _squareBracketRight = new LookbehindCharacterRule(']');
        private readonly ITokenRule _multilineCommentClosing = new LookbehindKeywordRule("*/");

        private readonly Regex _dollarQuote = new Regex(@"\G\$(?:[\p{L}_][\p{L}\d_]*)?\$");

        public IEnumerable<QsiScript> Parse(in string input)
        {
            var context = new ParseContext(input, Delimiter);
            var cursor = context.Cursor;
            ref int? fragmentStart = ref context._fragmentStart;
            ref var fragmentEnd = ref context._fragmentEnd;

            while (cursor.Index < cursor.Length)
            {
                if (IsEndOfScript(context))
                {
                    FlushScript(context);
                }
                else if (SkipToNextTransition(context, out var token))
                {
                    FlushToken(context);

                    if (context.Tokens.Count > 0 || token.Type != TokenType.WhiteSpace)
                        context.Tokens.Add(token);
                }
                else
                {
                    fragmentStart ??= cursor.Index;
                    fragmentEnd = cursor.Index;
                }

                cursor.Index++;
            }

            FlushScript(context);

            return context.Scripts;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlushToken(ParseContext context)
        {
            if (!context._fragmentStart.HasValue)
                return;

            context.Tokens.Add(new Token(TokenType.Fragment, context._fragmentStart.Value..(context._fragmentEnd + 1)));
            context._fragmentStart = null;
            context._fragmentEnd = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlushScript(ParseContext context)
        {
            FlushToken(context);

            if (context.Tokens.Count == 0)
                return;

            var tokenBuffer = new List<Token>();
            List<Token> tokens = context.Tokens;

            foreach (var section in SplitScriptSection(context))
            {
                var (offset, end) = section.GetOffsetAndLength(tokens.Count);
                end += offset;

                for (int i = offset; i < end; i++)
                {
                    var token = tokens[i];

                    if (token.Type == TokenType.WhiteSpace && (i == end - 1 || tokenBuffer.Count == 0))
                        continue;

                    tokenBuffer.Add(token);
                }

                if (tokenBuffer.Count == 0)
                    continue;

                var script = CreateScript(context, tokenBuffer);
                tokenBuffer.Clear();

                if (script != null)
                {
                    context.Scripts.Add(script);
                    context._lastLine = script.End.Line - 1;
                }
            }

            tokens.Clear();
        }

        protected virtual IEnumerable<Range> SplitScriptSection(ParseContext context)
        {
            if (context.Tokens.Count == 0)
                yield break;

            int bodyStartIndex = context.Tokens.FindIndex(t => t.Type == TokenType.Fragment);
            int bodyEndIndex = context.Tokens.FindLastIndex(t => t.Type == TokenType.Fragment);

            if (bodyStartIndex > 0)
                yield return ..bodyStartIndex;

            yield return new Range(
                bodyStartIndex == -1 ? Index.Start : bodyStartIndex,
                bodyEndIndex == -1 ? Index.End : bodyEndIndex + 1);

            if (bodyEndIndex >= 0 && bodyEndIndex + 1 < context.Tokens.Count)
                yield return (bodyEndIndex + 1)..;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool IsEndOfScript(ParseContext context)
        {
            if (context.Cursor.StartsWith(context.Delimiter))
            {
                context.Cursor.Index += context.Delimiter.Length - 1;
                return true;
            }

            return false;
        }

        protected virtual (QsiScriptPosition Start, QsiScriptPosition End) MeasurePosition(ParseContext context, int start, int end)
        {
            var value = context.Cursor.Value;
            ref int[] lineMap = ref context._lineMap;

            if (lineMap == null)
            {
                var map = new List<int>();
                int index = -1;

                while ((index = value.IndexOf('\n', index + 1)) >= 0)
                    map.Add(index);

                lineMap = map.ToArray();
            }

            var startLine = GetLine(start, context._lastLine) + 1;
            var startColumn = start + 1;

            if (startLine > 1)
                startColumn = start - lineMap[startLine - 2];

            var endLine = GetLine(end, startLine - 1) + 1;
            var endColumn = end + 1;

            if (endLine > 1)
                endColumn = end - lineMap[endLine - 2];

            return (new QsiScriptPosition(startLine, startColumn), new QsiScriptPosition(endLine, endColumn + 1));

            int GetLine(int index, int startIndex)
            {
                for (int i = startIndex; i < context._lineMap.Length; i++)
                {
                    if (index < context._lineMap[i])
                        return i;
                }

                return context._lineMap.Length;
            }
        }

        protected virtual QsiScript CreateScript(ParseContext context, IList<Token> tokens)
        {
            var startIndex = tokens[0].Span.Start.GetOffset(context.Cursor.Length);
            var endIndex = tokens[^1].Span.End.GetOffset(context.Cursor.Length) - 1;
            var (startPosition, endPosition) = MeasurePosition(context, startIndex, endIndex);
            var script = context.Cursor.Value[startIndex..(endIndex + 1)];

            return new QsiScript(script, QsiScriptType.Unknown, startPosition, endPosition);
        }

        protected virtual bool SkipToNextTransition(ParseContext context, out Token token)
        {
            int offset;
            TokenType tokenType;
            ITokenRule rule;
            var cursor = context.Cursor;

            switch (cursor.Current)
            {
                case '\'':
                    offset = 1;
                    rule = _singleQuote;
                    tokenType = TokenType.Fragment;
                    break;

                case '"':
                    offset = 1;
                    rule = _doubleQuote;
                    tokenType = TokenType.Fragment;
                    break;

                case '`':
                    offset = 1;
                    rule = _backQuote;
                    tokenType = TokenType.Fragment;
                    break;

                case '[':
                    offset = 1;
                    rule = _squareBracketRight;
                    tokenType = TokenType.Fragment;
                    break;

                case '/' when cursor.Next == '*':
                    offset = 2;
                    rule = _multilineCommentClosing;
                    tokenType = TokenType.MultiLineComment;
                    break;

                case '-' when cursor.Next == '-':
                    offset = 2;
                    rule = _newLine;
                    tokenType = TokenType.SingeLineComment;
                    break;

                case '$':
                    var match = _dollarQuote.Match(cursor.Value, cursor.Index);

                    if (match.Success)
                    {
                        offset = match.Length;
                        rule = new LookbehindKeywordRule(match.Value);
                        tokenType = TokenType.Fragment;
                        break;
                    }

                    goto default;

                case var c when char.IsWhiteSpace(c):
                    offset = 1;
                    rule = _whiteSpace;
                    tokenType = TokenType.WhiteSpace;
                    break;

                default:
                    token = default;
                    return false;
            }

            int start = cursor.Index;
            cursor.Index += offset;
            rule.Run(cursor);

            token = new Token(tokenType, start..(cursor.Index + 1));

            return true;
        }

        #region Context
        protected class ParseContext
        {
            public List<QsiScript> Scripts { get; }

            public CommonScriptCursor Cursor { get; }

            public string Delimiter { get; set; }

            public List<Token> Tokens { get; }

            internal int? _fragmentStart;
            internal int _fragmentEnd;

            internal int _lastLine;
            internal int[] _lineMap;

            public ParseContext(string input, string delimiter)
            {
                Scripts = new List<QsiScript>();
                Cursor = new CommonScriptCursor(input);
                Delimiter = delimiter;
                Tokens = new List<Token>();
            }
        }
        #endregion

        #region Token
        protected enum TokenType
        {
            Fragment,
            WhiteSpace,
            SingeLineComment,
            MultiLineComment
        }

        protected readonly struct Token
        {
            public Range Span { get; }

            public TokenType Type { get; }

            public Token(TokenType type, Range span)
            {
                Type = type;
                Span = span;
            }

            public override string ToString()
            {
                return $"{Span} ({Type})";
            }
        }
        #endregion
    }
}
