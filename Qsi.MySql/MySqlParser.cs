using Antlr4.Runtime;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.MySql.Tree;
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
            return ParseInternal(script, parser) ?? throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
        }

        private IQsiTreeNode ParseInternal(QsiScript script, Parser parser)
        {
            var mySqlParser = (Internal.MySqlParser)parser;

            switch (script.ScriptType)
            {
                case QsiScriptType.With:
                case QsiScriptType.Select:
                    return TableVisitor.VisitSelectStatement(mySqlParser.selectStatement());

                case QsiScriptType.Create:
                    return TableVisitor.VisitDdlStatement(mySqlParser.ddlStatement());

                case QsiScriptType.Prepare:
                case QsiScriptType.Execute:
                case QsiScriptType.DropPrepare:
                    return ActionVisitor.VisitPreparedStatement(mySqlParser.preparedStatement());

                case QsiScriptType.Insert:
                    return ActionVisitor.VisitInsertStatement(mySqlParser.insertStatement());

                case QsiScriptType.Replace:
                    return ActionVisitor.VisitReplaceStatement(mySqlParser.replaceStatement());

                default:
                    return null;
            }
        }
    }
}
