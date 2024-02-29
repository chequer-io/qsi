using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qsi.MongoDB.Internal.Nodes.Location;

namespace Qsi.MongoDB.Acorn;

internal sealed class MongoDBScript
{
    public string Script { get; private set; }

    public string PureScript { get; private set; }

    public IEnumerable<MongoDBStatement> SpecialStatements { get; private set; }

    public IEnumerable<MongoDBStatement> JavascriptStatements { get; private set; }

    public IEnumerable<MongoDBStatement> Statements
    {
        get
        {
            return _statements ??= JavascriptStatements
                .Concat(SpecialStatements)
                .OrderBy(r => r.Range.Start.Value)
                .ToArray();
        }
    }

    private MongoDBStatement[] _statements;

    private MongoDBScript()
    {
    }

    private void ParseSpecialStatements()
    {
        bool isKeywordMatched = false;
        bool isPatternMatched = false;
        bool multilineRemark = false;
        bool singlelineRemark = false;
        bool isLineStart = true;

        int line = 1, column = 0;
        int capturedIndex = 0;
        int capturedLine = -1, capturedColumn = -1;
        var sb = new StringBuilder(Script);
        var statements = new List<MongoDBStatement>();

        for (int i = 0; i < Script.Length; i++)
        {
            column++;
            var ch = Script[i];
            var nextCh = i + 1 < Script.Length ? Script[i + 1] : '\0';
            var remainLength = Script.Length - i;

            if (multilineRemark)
            {
                if (ch == '*' && Script[i..].StartsWith("*/"))
                {
                    i++;
                    column++;
                    var length = (i - capturedIndex) + 1;
                    sb.Remove(capturedIndex, length);
                    sb.Insert(capturedIndex, new string(' ', length));
                    multilineRemark = false;
                }

                continue;
            }

            if (singlelineRemark)
            {
                if (ch is '\r' or '\n')
                {
                    var length = i - capturedIndex;
                    sb.Remove(capturedIndex, length);
                    sb.Insert(capturedIndex, new string(' ', length));
                    singlelineRemark = false;
                }

                continue;
            }

            switch (ch)
            {
                case '\r' when nextCh == '\n':
                case '\n' when nextCh == '\r':
                    if (isKeywordMatched && isPatternMatched)
                        AddItem(capturedIndex, capturedIndex..i);

                    Reset();
                    i++;
                    line++;
                    column = 0;
                    break;

                case '\n':
                case '\r':
                    if (isKeywordMatched && isPatternMatched)
                        AddItem(capturedIndex, capturedIndex..i);

                    Reset();
                    line++;
                    column = 0;
                    break;

                case 's':
                    if (isLineStart &&
                        remainLength >= 4 &&
                        Script[i..].StartsWith("show") &&
                        Script[i + 4] is ' ' or '\t')
                    {
                        isKeywordMatched = true;
                        Capture(i);
                        i += 3;
                        column += 3;
                    }

                    isLineStart = false;
                    break;

                case 'u':
                    if (isLineStart &&
                        remainLength >= 3 &&
                        Script[i..].StartsWith("use") &&
                        Script[i + 3] is ' ' or '\t')
                    {
                        isKeywordMatched = true;
                        Capture(i);
                        i += 2;
                        column += 2;
                    }

                    isLineStart = false;
                    break;

                case ' ':
                case '\t':
                    if (isKeywordMatched && !isPatternMatched)
                        isPatternMatched = true;

                    break;

                case '/':
                    if (Script[i..].StartsWith("//"))
                    {
                        singlelineRemark = true;
                        Capture(i);
                        i++;
                        column++;
                    }

                    if (Script[i..].StartsWith("/*"))
                    {
                        multilineRemark = true;
                        Capture(i);
                        i++;
                        column++;
                    }

                    break;

                default:
                    isLineStart = false;
                    break;
            }
        }

        if (isKeywordMatched && isPatternMatched)
        {
            AddItem(capturedIndex, capturedIndex..Script.Length);
        }
        else if (multilineRemark)
        {
            var length = Script.Length - capturedIndex;
            sb.Remove(capturedIndex, length);
            sb.Insert(capturedIndex, new string(' ', length));
        }

        PureScript = sb.ToString();
        SpecialStatements = statements;

        void Capture(int value)
        {
            capturedLine = line;
            capturedColumn = column;
            capturedIndex = value;
        }

        void Reset()
        {
            capturedLine = -1;
            capturedColumn = -1;
            capturedIndex = -1;
            isKeywordMatched = false;
            isPatternMatched = false;
            isLineStart = true;
        }

        void AddItem(int index, Range range)
        {
            var length = range.End.Value - range.Start.Value;
            sb.Remove(index, length);
            sb.Insert(index, new string(' ', length));

            statements.Add(new MongoDBStatement
            {
                Range = range,
                Start = new Location(capturedLine, capturedColumn - 1),
                End = new Location(line, column),
            });
        }
    }

    private void ParseJavascriptStatements()
    {
        JavascriptStatements = AcornParser.Parse(PureScript);
    }

    public static MongoDBScript Parse(string script)
    {
        var mongoDbScript = new MongoDBScript
        {
            Script = script
        };

        mongoDbScript.ParseSpecialStatements();
        mongoDbScript.ParseJavascriptStatements();

        return mongoDbScript;
    }
}
