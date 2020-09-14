using System;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.MySql.Tree;
using Qsi.Parsing;
using Qsi.Parsing.Antlr;
using Qsi.Tree;

namespace Qsi.MySql
{
    public sealed class MySqlParser : AntlrParserBase
    {
        protected override Parser CreateParser(QsiScript script)
        {
            var stream = new AntlrUpperInputStream(script.Script);
            var lexer = new MySqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            return new Internal.MySqlParser(tokens);
        }

        protected override IQsiTreeNode Parse(QsiScript script, Parser parser)
        {
            return ParseInternal(script, parser) ?? throw new QsiException(QsiError.NotSupportedScript);
        }

        private IQsiTreeNode ParseInternal(QsiScript script, Parser parser)
        {
            var mySqlParser = (Internal.MySqlParser)parser;

            switch (script.ScriptType)
            {
                // ** DML **

                case QsiScriptType.DataManipulation:
                    return TableVisitor.VisitDmlStatement(mySqlParser.dmlStatement());

                case QsiScriptType.Select:
                    return TableVisitor.VisitSelectStatement(mySqlParser.selectStatement());

                // ** DDL **

                case QsiScriptType.DataDefinition:
                    return TableVisitor.VisitDdlStatement(mySqlParser.ddlStatement());

                case QsiScriptType.CreateView:
                    return TableVisitor.VisitCreateView(mySqlParser.createView());

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
