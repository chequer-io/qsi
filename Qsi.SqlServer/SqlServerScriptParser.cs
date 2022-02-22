using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Parsing.Common;
using Qsi.Shared.Extensions;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;

namespace Qsi.SqlServer
{
    public class SqlServerScriptParser : CommonScriptParser
    {
        private const string Exec = "EXEC";
        private const string Merge = "MERGE";
        private const string BulkInsert = "BULKINSERT";

        private readonly TSqlParserInternal _parser;

        public SqlServerScriptParser(TransactSqlVersion transactSqlVersion)
        {
            _parser = new TSqlParserInternal(transactSqlVersion, false);
        }

        protected override QsiScriptType GetSuitableType(CommonScriptCursor cursor, IReadOnlyList<Token> tokens, Token[] leadingTokens)
        {
            if (leadingTokens.Length >= 1)
            {
                if (Exec.EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]))
                    return QsiScriptType.Execute;

                if (Merge.EqualsIgnoreCase(cursor.Value[leadingTokens[0].Span]))
                    return QsiScriptType.MergeInto;
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

            if (result is not TSqlScript script)
                return Enumerable.Empty<QsiScript>();

            IList<TSqlParserToken> tokens = script.ScriptTokenStream;
            var scripts = new List<QsiScript>();

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

                        AddScript();
                    }

                    start = statement.FirstTokenIndex;
                    end = statement.LastTokenIndex;

                    AddScript();

                    index = statement.LastTokenIndex;
                }
            }

            if (index < tokens.Count - 1)
            {
                start = index + 1;
                end = tokens.Count - 1;

                AddScript();
            }

            return scripts;

            void AddScript()
            {
                bool trim = false;

                for (; start <= end; start++)
                {
                    if (IsTrivia(tokens[start].TokenType))
                        continue;

                    trim = true;
                    break;
                }

                if (!trim)
                    return;

                for (; end > start; end--)
                {
                    if (IsTrivia(tokens[end].TokenType))
                        continue;

                    break;
                }

                var count = end - start + 1;
                var listSegment = new ListSegment<TSqlParserToken>(tokens, start, count);

                if (listSegment.All(s => s.TokenType is TSqlTokenType.WhiteSpace or TSqlTokenType.Go or TSqlTokenType.Semicolon))
                    return;

                var first = listSegment[0];
                var last = listSegment[^1];
                var (startPosition, endPosition) = GetScriptPosition(first, last);

                var text = input[first.Offset..(last.Offset + last.Text.Length)];

                scripts.Add(new QsiScript(
                    in text,
                    GetScriptType(ref listSegment),
                    startPosition,
                    endPosition)
                );
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsTrivia(TSqlTokenType tokenType)
        {
            return tokenType
                is TSqlTokenType.WhiteSpace
                or TSqlTokenType.Semicolon
                or TSqlTokenType.EndOfFile;
        }

        private static QsiScriptType GetScriptType(ref ListSegment<TSqlParserToken> tokens)
        {
            TSqlParserToken[] leadingTokens = tokens.AsEnumerable()
                .Where(t => t.TokenType is > TSqlTokenType.EndOfFile and < TSqlTokenType.Bang)
                .Take(2)
                .ToArray();

            if (leadingTokens.Length >= 2)
            {
                var fragment = $"{leadingTokens[0].Text}{leadingTokens[1].Text}";

                if (Enum.TryParse<QsiScriptType>(fragment, true, out var type) &&
                    type is not QsiScriptType.Trivia)
                {
                    return type;
                }

                if (BulkInsert.EqualsIgnoreCase(fragment))
                    return QsiScriptType.Insert;
            }

            if (leadingTokens.Length >= 1)
            {
                if (Enum.TryParse<QsiScriptType>(leadingTokens[0].Text, true, out var type) &&
                    type is not QsiScriptType.Trivia)
                {
                    return type;
                }

                if (Exec.EqualsIgnoreCase(leadingTokens[0].Text))
                    return QsiScriptType.Execute;
            }

            if (tokens.All(t => t.TokenType
                    is TSqlTokenType.MultilineComment
                    or TSqlTokenType.SingleLineComment
                    or TSqlTokenType.WhiteSpace))
            {
                return QsiScriptType.Trivia;
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

            var start = new QsiScriptPosition(startToken.Line, startToken.Column, startToken.Offset);
            var end = new QsiScriptPosition(endToken.Line + lineCount, endColumn, endToken.Offset);

            return (start, end);
        }
    }
}
