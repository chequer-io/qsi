﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;

namespace Qsi.SqlServer
{
    public class SqlServerScriptParser : IQsiScriptParser
    {
        private readonly TSqlParserInternal _parser;

        public SqlServerScriptParser(TransactSqlVersion transactSqlVersion)
        {
            _parser = new TSqlParserInternal(transactSqlVersion, false);
        }

        public IEnumerable<QsiScript> Parse(in string input, CancellationToken cancellationToken)
        {
            var result = _parser.Parse(input);
            var sc = input;

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

                var text = sc[first.Offset..(last.Offset + last.Text.Length)];

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
            var token = tokens[0];

            if (token.TokenType > TSqlTokenType.EndOfFile && token.TokenType < TSqlTokenType.Bang &&
                Enum.TryParse<QsiScriptType>(token.Text, true, out var type))
            {
                return type;
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
