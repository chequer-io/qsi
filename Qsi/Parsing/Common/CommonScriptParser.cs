using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Parsing.Common.Rules;

namespace Qsi.Parsing.Common
{
    public class CommonScriptParser : IQsiScriptParser
    {
        private readonly ITokenRule _whiteSpace = new LookbehindWhiteSpaceRule();
        private readonly ITokenRule _newLine = new LookaheadEndOfLineRule();
        private readonly ITokenRule _keyword = new LookbehindUnknownKeywordRule();
        private readonly ITokenRule _singleQuote = new LookbehindLiteralRule('\'', true);
        private readonly ITokenRule _doubleQuote = new LookbehindLiteralRule('"', true);
        private readonly ITokenRule _backQuote = new LookbehindCharacterRule('`');
        private readonly ITokenRule _squareBracketRight = new LookbehindCharacterRule(']');
        private readonly ITokenRule _multilineCommentClosing = new LookbehindKeywordRule("*/");

        private readonly Regex _dollarQuote = new Regex(@"\G\$(?:[\p{L}_][\p{L}\d_]*)?\$");
        private readonly string _delimiter;

        public CommonScriptParser(string delimiter = ";")
        {
            _delimiter = delimiter;
        }

        public IEnumerable<QsiScript> Parse(in string input, CancellationToken cancellationToken)
        {
            var context = new ParseContext(input, _delimiter, cancellationToken);
            var cursor = context.Cursor;
            ref int? fragmentStart = ref context.FragmentStart;
            ref var fragmentEnd = ref context.FragmentEnd;

            while (cursor.Index < cursor.Length)
            {
                if (IsEndOfScript(context))
                {
                    FlushScript(context);
                }
                else if (TryParseToken(context, out var token))
                {
                    FlushToken(context);

                    if (context.Tokens.Count > 0 || token.Type != TokenType.WhiteSpace)
                        context.AddToken(token);
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
            if (!context.FragmentStart.HasValue)
                return;

            context.AddToken(new Token(TokenType.Fragment, context.FragmentStart.Value..(context.FragmentEnd + 1)));
            context.FragmentStart = null;
            context.FragmentEnd = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlushScript(ParseContext context)
        {
            FlushToken(context);

            if (context.Tokens.Count == 0)
                return;

            foreach (var section in SplitScriptSection(context))
            {
                var (offset, length) = section.GetOffsetAndLength(context.Tokens.Count);
                var end = offset + length;

                Token[] tokenBuffer = ArrayPool<Token>.Shared.Rent(length);
                var tokenIndex = 0;

                for (int i = offset; i < end; i++)
                {
                    var token = context.Tokens[i];

                    if (token.Type == TokenType.WhiteSpace && (i == end - 1 || tokenIndex == 0))
                        continue;

                    tokenBuffer[tokenIndex++] = token;
                }

                if (tokenIndex > 0)
                {
                    var script = CreateScript(context, new ArraySegment<Token>(tokenBuffer, 0, tokenIndex));

                    if (script != null)
                    {
                        context.Scripts.Add(script);
                        context.LastLine = script.End.Line - 1;
                    }
                }

                ArrayPool<Token>.Shared.Return(tokenBuffer);
            }

            context.ClearTokens();
        }

        protected virtual IEnumerable<Range> SplitScriptSection(ParseContext context)
        {
            if (context.Tokens.Count == 0)
                yield break;

            int bodyStartIndex = context.Tokens.FindIndex(t => TokenType.Effective.HasFlag(t.Type));
            int bodyEndIndex = context.Tokens.FindLastIndex(t => TokenType.Effective.HasFlag(t.Type));

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
            ref int[] lineMap = ref context.LineMap;

            if (lineMap == null)
            {
                var map = new List<int>();
                int index = -1;

                while ((index = value.IndexOf('\n', index + 1)) >= 0)
                    map.Add(index);

                lineMap = map.ToArray();
            }

            var startLine = GetLine(start, context.LastLine) + 1;
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
                for (int i = startIndex; i < context.LineMap.Length; i++)
                {
                    if (index < context.LineMap[i])
                        return i;
                }

                return context.LineMap.Length;
            }
        }

        protected virtual QsiScript CreateScript(ParseContext context, IReadOnlyList<Token> tokens)
        {
            var startIndex = tokens[0].Span.Start.GetOffset(context.Cursor.Length);
            var endIndex = tokens[^1].Span.End.GetOffset(context.Cursor.Length) - 1;
            var (startPosition, endPosition) = MeasurePosition(context, startIndex, endIndex);
            var script = context.Cursor.Value[startIndex..(endIndex + 1)];
            var scriptType = GetSuitableScriptType(context, tokens);

            return new QsiScript(script, scriptType, startPosition, endPosition);
        }

        protected virtual QsiScriptType GetSuitableScriptType(ParseContext context, IReadOnlyList<Token> tokens)
        {
            var firstToken = tokens[0];

            if (firstToken.Type == TokenType.Keyword &&
                Enum.TryParse<QsiScriptType>(context.Cursor.Value[firstToken.Span], true, out var type))
            {
                return type;
            }

            if (tokens.All(t =>
                t.Type == TokenType.WhiteSpace ||
                t.Type == TokenType.SingeLineComment ||
                t.Type == TokenType.MultiLineComment))
            {
                return QsiScriptType.CommentGroup;
            }

            return QsiScriptType.Unknown;
        }

        protected virtual bool TryParseToken(ParseContext context, out Token token)
        {
            int offset;
            TokenType tokenType;
            ITokenRule rule;
            var cursor = context.Cursor;

            switch (cursor.Current)
            {
                case '_':
                case var c when 'A' <= c && c <= 'Z' || 'a' <= c && c <= 'z':
                    offset = 1;
                    rule = _keyword;
                    tokenType = TokenType.Keyword;
                    break;

                case '\'':
                    offset = 1;
                    rule = _singleQuote;
                    tokenType = TokenType.Literal;
                    break;

                case '"':
                    offset = 1;
                    rule = _doubleQuote;
                    tokenType = TokenType.Literal;
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

            if (rule.Run(cursor))
            {
                token = new Token(tokenType, start..(cursor.Index + 1));
                return true;
            }

            if (tokenType == TokenType.MultiLineComment)
            {
                cursor.Index = cursor.Length - 1;
                token = new Token(tokenType, start..(cursor.Index + 1));
                return true;
            }

            token = default;
            cursor.Index = start;

            return false;
        }

        #region Context
        protected class ParseContext
        {
            public readonly List<QsiScript> Scripts;

            public readonly CommonScriptCursor Cursor;

            public string Delimiter { get; set; }

            public readonly IReadOnlyList<Token> Tokens;

            internal int? FragmentStart;
            internal int FragmentEnd;

            internal int LastLine;
            internal int[] LineMap;

            private readonly List<Token> _tokens;
            private readonly CancellationToken _cancellationToken;

            public ParseContext(string input, string delimiter, CancellationToken cancellationToken)
            {
                Scripts = new List<QsiScript>();
                Cursor = new CommonScriptCursor(input);
                Delimiter = delimiter;
                _tokens = new List<Token>();
                Tokens = _tokens;
                _cancellationToken = cancellationToken;
            }

            public void AddToken(Token token)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                _tokens.Add(token);
            }

            public void ClearTokens()
            {
                _tokens.Clear();
            }
        }
        #endregion

        #region Token
        [Flags]
        protected enum TokenType
        {
            Keyword = 1 << 1,
            Literal = 1 << 2,
            Fragment = 1 << 3,
            WhiteSpace = 1 << 4,
            SingeLineComment = 1 << 5,
            MultiLineComment = 1 << 6,

            Effective = Keyword | Literal | Fragment,
            Trivia = WhiteSpace | SingeLineComment | MultiLineComment
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
