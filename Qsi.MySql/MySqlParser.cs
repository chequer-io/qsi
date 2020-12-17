using System;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.MySql.Tree;
using Qsi.Parsing.Antlr;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

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
            var query = ((MySqlParserInternal)parser).query();

            if (query.children[0] is not SimpleStatementContext simpleStatement)
                return null;

            switch (simpleStatement.children[0])
            {
                case SelectStatementContext selectStatement:
                    return TableVisitor.VisitSelectStatement(selectStatement);

                case DeleteStatementContext deleteStatement:
                    return ActionVisitor.VisitDeleteStatement(deleteStatement);

                case ReplaceStatementContext replaceStatement:
                    return ActionVisitor.VisitReplaceStatement(replaceStatement);

                case UpdateStatementContext updateStatement:
                    return ActionVisitor.VisitUpdateStatement(updateStatement);

                default:
                    throw TreeHelper.NotSupportedTree(simpleStatement.children[0]);
            }
        }
    }
}
