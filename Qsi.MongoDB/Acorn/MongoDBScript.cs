using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qsi.MongoDB.Acorn
{
    public sealed class MongoDBScript
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

            int line = 0, column = 0;
            int capturedIndex = 0;
            var sb = new StringBuilder(Script);
            var ranges = new List<Range>();
            
            for (int i = 0; i < Script.Length; i++)
            {
                column++;
                var ch = Script[i];
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
                    if (ch == '\r' || ch == '\n')
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
                    case '\r':
                    case '\n':
                        if (isKeywordMatched && isPatternMatched)
                            AddItem(capturedIndex, capturedIndex..i);

                        capturedIndex = -1;
                        isKeywordMatched = false;
                        isPatternMatched = false;
                        isLineStart = true;
                        break;

                    case 's':
                        if (isLineStart && remainLength >= 4 && Script[i..].StartsWith("show"))
                        {
                            isKeywordMatched = true;
                            capturedIndex = i;
                            i += 3;
                            column += 3;
                        }

                        isLineStart = false;
                        break;

                    case 'u':
                        if (isLineStart && remainLength >= 3 && Script[i..].StartsWith("use"))
                        {
                            isKeywordMatched = true;
                            capturedIndex = i;
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
                            capturedIndex = i;
                            i++;
                            column++;
                        }

                        if (Script[i..].StartsWith("/*"))
                        {
                            multilineRemark = true;
                            capturedIndex = i;
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
            // SpecialStatements = ranges;

            void AddItem(int index, Range range)
            {
                var length = range.End.Value - range.Start.Value;
                sb.Remove(index, length);
                sb.Insert(index, new string(' ', length));

                ranges.Add(range);
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
}
