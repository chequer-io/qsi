using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Qsi.Parsing;
using MySQLLexer = Qsi.MySql.Internal.MySqlLexerInternal;
using MySQLParser = Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql.Internal;

internal class MySqlParserErrorHandler : IAntlrErrorListener<IToken>
{
    private static readonly HashSet<int> _simpleRules = new(new[]
    {
        MySQLParser.RULE_identifier,
        MySQLParser.RULE_qualifiedIdentifier
    });

    private static readonly Dictionary<int, string> _objectNames = new()
    {
        [MySQLParser.RULE_columnName] = "column",
        [MySQLParser.RULE_columnRef] = "column",
        [MySQLParser.RULE_columnInternalRef] = "column",
        [MySQLParser.RULE_indexName] = "index",
        [MySQLParser.RULE_indexRef] = "index",
        [MySQLParser.RULE_schemaName] = "schema",
        [MySQLParser.RULE_schemaRef] = "schema",
        [MySQLParser.RULE_procedureName] = "procedure",
        [MySQLParser.RULE_procedureRef] = "procedure",
        [MySQLParser.RULE_functionName] = "function",
        [MySQLParser.RULE_functionRef] = "function",
        [MySQLParser.RULE_triggerName] = "trigger",
        [MySQLParser.RULE_triggerRef] = "trigger",
        [MySQLParser.RULE_viewName] = "view",
        [MySQLParser.RULE_viewRef] = "view",
        [MySQLParser.RULE_tablespaceName] = "tablespace",
        [MySQLParser.RULE_tablespaceRef] = "tablespace",
        [MySQLParser.RULE_logfileGroupName] = "logfile group",
        [MySQLParser.RULE_logfileGroupRef] = "logfile group",
        [MySQLParser.RULE_eventName] = "event",
        [MySQLParser.RULE_eventRef] = "event",
        [MySQLParser.RULE_udfName] = "udf",
        [MySQLParser.RULE_serverName] = "server",
        [MySQLParser.RULE_serverRef] = "server",
        [MySQLParser.RULE_engineRef] = "engine",
        [MySQLParser.RULE_tableName] = "table",
        [MySQLParser.RULE_tableRef] = "table",
        [MySQLParser.RULE_filterTableRef] = "table",
        [MySQLParser.RULE_tableRefWithWildcard] = "table",
        [MySQLParser.RULE_parameterName] = "parameter",
        [MySQLParser.RULE_labelIdentifier] = "label",
        [MySQLParser.RULE_labelRef] = "label",
        [MySQLParser.RULE_roleIdentifier] = "role",
        [MySQLParser.RULE_roleRef] = "role",
        [MySQLParser.RULE_pluginRef] = "plugin",
        [MySQLParser.RULE_componentRef] = "component",
        [MySQLParser.RULE_resourceGroupRef] = "resource group",
        [MySQLParser.RULE_windowName] = "window",
    };

    // modules/db.mysql.parser/src/mysql_parser_module.cpp
    // ParserErrorListener::syntaxError
    public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int column, string msg, RecognitionException e)
    {
        var parser = (MySQLParser)recognizer;
        var lexer = (MySqlBaseLexer)parser.TokenStream.TokenSource;
        bool isEof = offendingSymbol.Type == MySQLLexer.Eof;

        if (isEof)
            offendingSymbol = parser.TokenStream.LT(-1) ?? offendingSymbol;

        var wrongText = offendingSymbol.Text;
        var expected = parser.GetExpectedTokens();
        bool invalidForVersion = false;
        var tokenType = offendingSymbol.Type;

        if (tokenType != MySQLLexer.IDENTIFIER && expected.Contains(tokenType))
        {
            invalidForVersion = true;
        }
        else
        {
            tokenType = lexer.keywordFromText(wrongText);
            invalidForVersion = expected.Contains(tokenType);
        }

        if (invalidForVersion)
        {
            expected = new IntervalSet(expected);
            expected.Remove(tokenType);
        }

        string expectedText;
        RuleContext context = parser.RuleContext;

        while (_simpleRules.Contains(context.RuleIndex))
            context = context.Parent;

        switch (context.RuleIndex)
        {
            case MySQLParser.RULE_functionCall:
                expectedText = "a complete function call or other expression";
                break;

            case MySQLParser.RULE_expr:
                expectedText = "an expression";
                break;

            case MySQLParser.RULE_columnName:
            case MySQLParser.RULE_indexName:
            case MySQLParser.RULE_schemaName:
            case MySQLParser.RULE_procedureName:
            case MySQLParser.RULE_functionName:
            case MySQLParser.RULE_triggerName:
            case MySQLParser.RULE_viewName:
            case MySQLParser.RULE_tablespaceName:
            case MySQLParser.RULE_logfileGroupName:
            case MySQLParser.RULE_eventName:
            case MySQLParser.RULE_udfName:
            case MySQLParser.RULE_serverName:
            case MySQLParser.RULE_tableName:
            case MySQLParser.RULE_parameterName:
            case MySQLParser.RULE_labelIdentifier:
            case MySQLParser.RULE_roleIdentifier:
            case MySQLParser.RULE_windowName:
            {
                if (!_objectNames.TryGetValue(context.RuleIndex, out var name))
                    name = "object";

                expectedText = $"a new {name} name";
                break;
            }

            case MySQLParser.RULE_columnRef:
            case MySQLParser.RULE_indexRef:
            case MySQLParser.RULE_schemaRef:
            case MySQLParser.RULE_procedureRef:
            case MySQLParser.RULE_functionRef:
            case MySQLParser.RULE_triggerRef:
            case MySQLParser.RULE_viewRef:
            case MySQLParser.RULE_tablespaceRef:
            case MySQLParser.RULE_logfileGroupRef:
            case MySQLParser.RULE_eventRef:
            case MySQLParser.RULE_serverRef:
            case MySQLParser.RULE_engineRef:
            case MySQLParser.RULE_tableRef:
            case MySQLParser.RULE_filterTableRef:
            case MySQLParser.RULE_tableRefWithWildcard:
            case MySQLParser.RULE_labelRef:
            case MySQLParser.RULE_roleRef:
            case MySQLParser.RULE_pluginRef:
            case MySQLParser.RULE_componentRef:
            case MySQLParser.RULE_resourceGroupRef:
            {
                if (!_objectNames.TryGetValue(context.RuleIndex, out var name))
                    name = "object";

                expectedText = $"the name of an existing {name}";
                break;
            }

            case MySQLParser.RULE_columnInternalRef:
                expectedText = "a column name from this table";
                break;

            default:
            {
                expectedText = expected.Contains(MySQLLexer.IDENTIFIER) ?
                    "an identifier" :
                    IntervalToString(expected, 6, parser.Vocabulary);

                break;
            }
        }

        if (wrongText[0] is not '"' and not '\'' and not '`')
            wrongText = $"\"{wrongText}\"";

        switch (e)
        {
            case null when msg.Contains("missing"):
            {
                if (expected.Count == 1)
                {
                    msg = $"Missing {expectedText}";
                }

                break;
            }

            case null:
                msg = $"Extraneous input {wrongText} found, expecting {expectedText}";
                break;

            case InputMismatchException:
                msg = isEof ?
                    "Statement is incomplete" :
                    $"{wrongText} is not valid at this position";

                if (!string.IsNullOrEmpty(expectedText))
                    msg += $", expecting {expectedText}";

                break;

            case FailedPredicateException fpe:
                const string prefix = "predicate failed: ";

                var condition = fpe
                    .Predicate[prefix.Length..]
                    .Replace("serverVersion", "server version")
                    .Replace("&&", "and");

                msg = $"{wrongText} is valid only for {condition}";

                break;

            case NoViableAltException:
                if (isEof)
                {
                    msg = "Statement is incomplete";
                }
                else
                {
                    msg = $"{wrongText} is not valid at this position";

                    if (invalidForVersion)
                        msg += " for this server version";
                }

                if (!string.IsNullOrEmpty(expectedText))
                    msg += $", expecting {expectedText}";

                break;
        }

        throw new QsiSyntaxErrorException(line, column, msg);
    }

    private string IntervalToString(IntervalSet set, int maxCount, IVocabulary vocabulary)
    {
        IList<int> symbols = set.ToList();

        if (symbols.Count == 0)
            return string.Empty;

        var buffer = new StringBuilder();
        maxCount = Math.Min(maxCount, symbols.Count);

        for (int i = 0; i < maxCount; i++)
        {
            if (i > 0)
                buffer.Append(", ");

            int symbol = symbols[i];

            if (symbol < 0)
            {
                buffer.Append("EOF");
                continue;
            }

            var name = vocabulary.GetDisplayName(symbol);

            if (name.EndsWith("_SYMBOL"))
            {
                name = name[..^7];
            }
            else if (name.EndsWith("_OPERATOR"))
            {
                name = name[..^9];
            }
            else if (name.EndsWith("_NUMBER"))
            {
                name = $"{name[..^7]} number";
            }
            else
            {
                switch (name)
                {
                    case "BACK_TICK_QUOTED_ID":
                        name = "`text`";
                        break;

                    case "DOUBLE_QUOTED_TEXT":
                        name = "\"text\"";
                        break;

                    case "SINGLE_QUOTED_TEXT":
                        name = "'text'";
                        break;
                }
            }

            buffer.Append(name);
        }

        if (maxCount < symbols.Count)
            buffer.Append(", ...");

        return buffer.ToString();
    }
}