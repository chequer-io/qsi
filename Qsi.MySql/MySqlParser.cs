using System;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.Parsing.Antlr;
using Qsi.Tree;

namespace Qsi.MySql
{
    public sealed class MySqlParser : AntlrParserBase
    {
        private readonly int _version;

        public MySqlParser(Version version)
        {
            _version = MySQLUtility.VersionToInt(version);
        }

        protected override Parser CreateParser(QsiScript script)
        {
            return MySQLUtility.CreateParser(script.Script, _version);
        }

        protected override IQsiTreeNode Parse(QsiScript script, Parser parser)
        {
            return ParseInternal(script, parser) ?? throw new QsiException(QsiError.NotSupportedScript, script.ScriptType);
        }

        private IQsiTreeNode ParseInternal(QsiScript script, Parser parser)
        {
            throw new System.NotImplementedException();

            // var mySqlParser = (Internal.MySqlParser)parser;
            //
            // switch (script.ScriptType)
            // {
            //     case QsiScriptType.With:
            //     case QsiScriptType.Select:
            //         return TableVisitor.VisitSelectStatement(mySqlParser.selectStatement());
            //
            //     case QsiScriptType.Create:
            //         return TableVisitor.VisitDdlStatement(mySqlParser.ddlStatement());
            //
            //     case QsiScriptType.Prepare:
            //     case QsiScriptType.Execute:
            //     case QsiScriptType.DropPrepare:
            //         return ActionVisitor.VisitPreparedStatement(mySqlParser.preparedStatement());
            //
            //     case QsiScriptType.Insert:
            //         return ActionVisitor.VisitInsertStatement(mySqlParser.insertStatement());
            //
            //     case QsiScriptType.Replace:
            //         return ActionVisitor.VisitReplaceStatement(mySqlParser.replaceStatement());
            //
            //     case QsiScriptType.Delete:
            //         return ActionVisitor.VisitDeleteStatement(mySqlParser.deleteStatement());
            //     
            //     case QsiScriptType.Update:
            //         return ActionVisitor.VisitUpdateStatement(mySqlParser.updateStatement());
            //
            //     case QsiScriptType.Use:
            //         return ActionVisitor.VisitUseStatement(mySqlParser.useStatement());
            //     
            //     default:
            //         return null;
            // }
        }
    }
}
