using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;

namespace Qsi.SqlServer
{
    public class SqlServerScriptParser : CommonScriptParser
    {
        private const string Exec = "EXEC";
        private const string BulkInsert = "BULKINSERT";

        private readonly TSqlParserInternal _parser;

        public SqlServerScriptParser(TransactSqlVersion transactSqlVersion)
        {
            _parser = new TSqlParserInternal(transactSqlVersion, false);
        }

        protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IReadOnlyList<Token> tokens, Token[] leadingTokens)
        {
            if (leadingTokens.Length >= 1 &&
                Exec.Equals(cursor.Value[leadingTokens[0].Span], StringComparison.OrdinalIgnoreCase))
            {
                return QsiScriptType.Execute;
            }

            return base.GetSuitableType(cursor, tokens, leadingTokens);
        }

        public override IEnumerable<QsiScript> Parse(string input, CancellationToken cancellationToken)
        {
            TSqlFragment result;

            try
            {
                result = _parser.Parse(input);
            }
            catch (Exception)
            {
                return base.Parse(input, cancellationToken);
            }

            if (!(result is TSqlScript script))
                return Enumerable.Empty<QsiScript>();

            IList<TSqlParserToken> tokenStream = script.ScriptTokenStream;
            var list = new List<QsiScript>();

            var index = -1;
            int start, end;

            foreach (var batch in script.Batches)
            {
                foreach (var statement in batch.Statements)
                {
                    if (index < statement.FirstTokenIndex && statement.FirstTokenIndex != 0)
                    {
                        start = index + 1;
                        end = statement.FirstTokenIndex - 1;

                        TrimTrivia();
                        AddScript();
                    }

                    start = statement.FirstTokenIndex;
                    end = statement.LastTokenIndex;

                    AddScript();

                    index = statement.LastTokenIndex;
                }
            }

            if (index < tokenStream.Count - 1)
            {
                start = index + 1;
                end = tokenStream.Count - 1;

                TrimTrivia();
                AddScript();
            }

            return list;

            void TrimTrivia()
            {
                bool trim = false;

                for (; start <= end; start++)
                {
                    if (IsTrivia(tokenStream[start].TokenType))
                        continue;

                    trim = true;
                    break;
                }

                if (!trim)
                {
                    start = -1;
                    end = -1;
                }

                for (; end > start; end--)
                {
                    if (IsTrivia(tokenStream[end].TokenType))
                        continue;

                    break;
                }
            }

            void AddScript()
            {
                if (end == -1 || start == -1)
                    return;

                var count = end - start + 1;
                var listSegment = new ListSegment<TSqlParserToken>(tokenStream, start, count);

                if (listSegment.All(s =>
                    s.TokenType == TSqlTokenType.WhiteSpace ||
                    s.TokenType == TSqlTokenType.Go ||
                    s.TokenType == TSqlTokenType.Semicolon))
                {
                    return;
                }

                var first = listSegment[0];
                var last = listSegment[^1];
                var (startPosition, endPosition) = GetScriptPosition(first, last);

                var text = input[first.Offset..(last.Offset + last.Text.Length)];

                list.Add(new QsiScript(
                    in text,
                    GetScriptType(ref listSegment),
                    startPosition,
                    endPosition)
                );
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTrivia(TSqlTokenType tokenType)
        {
            return tokenType == TSqlTokenType.WhiteSpace ||
                   tokenType == TSqlTokenType.Semicolon ||
                   tokenType == TSqlTokenType.EndOfFile;
        }

        private QsiScriptType GetScriptType(ref ListSegment<TSqlParserToken> tokens)
        {
            TSqlParserToken[] leadingTokens = tokens.AsEnumerable()
                .Where(t => t.TokenType > TSqlTokenType.EndOfFile && t.TokenType < TSqlTokenType.Bang)
                .Take(2)
                .ToArray();

            if (leadingTokens.Length >= 2)
            {
                var fragment = $"{leadingTokens[0].Text}{leadingTokens[1].Text}";

                if (Enum.TryParse<QsiScriptType>(fragment, true, out var type))
                    return type;

                if (BulkInsert.Equals(fragment, StringComparison.OrdinalIgnoreCase))
                    return QsiScriptType.Insert;
            }

            if (leadingTokens.Length >= 1)
            {
                if (Enum.TryParse<QsiScriptType>(leadingTokens[0].Text, true, out var type))
                    return type;

                if (Exec.Equals(leadingTokens[0].Text, StringComparison.OrdinalIgnoreCase))
                    return QsiScriptType.Execute;
            }

            if (tokens.All(t =>
                t.TokenType == TSqlTokenType.MultilineComment ||
                t.TokenType == TSqlTokenType.SingleLineComment ||
                t.TokenType == TSqlTokenType.WhiteSpace))
            {
                return QsiScriptType.Comment;
            }

            return QsiScriptType.Unknown;
        }

        private (QsiScriptPosition Start, QsiScriptPosition End) GetScriptPosition(TSqlParserToken startToken, TSqlParserToken endToken)
        {
            var endTokenText = endToken.Text;
            int endColumn = endToken.Column + endTokenText.Length;

            int index = -1;
            int lineCount = 0;

            while ((index = endTokenText.IndexOf('\n', index + 1)) >= 0)
            {
                lineCount++;
                endColumn = endTokenText.Length - index;
            }

            var start = new QsiScriptPosition(startToken.Line, startToken.Column);
            var end = new QsiScriptPosition(endToken.Line + lineCount, endColumn);

            return (start, end);
        }
    }
}
