using System;
using Antlr4.Runtime;
using PrimarSql.Internal;
using Qsi.Data;
using Qsi.Parsing.Antlr;
using Qsi.PrimarSql.Tree;
using Qsi.Tree;

namespace Qsi.PrimarSql
{
    public class PrimarSqlParser : AntlrParserBase
    {
        protected override Parser CreateParser(QsiScript script)
        {
            var stream = new AntlrUpperInputStream(script.Script);
            var lexer = new PrimarSqlLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            return new global::PrimarSql.Internal.PrimarSqlParser(tokens);
        }
        
        protected override IQsiTreeNode Parse(QsiScript script, Parser parser)
        {
            return ParseInternal(script, parser) ?? throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
        }

        private IQsiTreeNode ParseInternal(QsiScript script, Parser parser)
        {
            var primarSqlParser = (global::PrimarSql.Internal.PrimarSqlParser)parser;

            switch (script.ScriptType)
            {
                case QsiScriptType.Select:
                    return TableVisitor.VisitSelectStatement(primarSqlParser.selectStatement());
                
                // case QsiScriptType.Insert:
                //     return ActionVisitor.VisitInsertStatement(primarSqlParser.insertStatement());
                //
                // case QsiScriptType.Replace:
                //     return ActionVisitor.VisitReplaceStatement(primarSqlParser.replaceStatement());
                //
                // case QsiScriptType.Delete:
                //     return ActionVisitor.VisitDeleteStatement(primarSqlParser.deleteStatement());
                //
                // case QsiScriptType.Update:
                //     return ActionVisitor.VisitUpdateStatement(primarSqlParser.updateStatement());
                default:
                    return null;
            }
        }
    }
}
