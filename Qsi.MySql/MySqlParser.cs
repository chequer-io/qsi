using System;
using System.Threading;
using Qsi.Data;
using Qsi.MySql.Internal;
using Qsi.MySql.Tree;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.MySql.Internal.MySqlParserInternal;

namespace Qsi.MySql
{
    public sealed class MySqlParser : IQsiTreeParser
    {
        private readonly int _version;

        public MySqlParser(Version version)
        {
            _version = MySQLUtility.VersionToInt(version);
        }

        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var parser = MySQLUtility.CreateParser(script.Script, _version);
            var query = parser.query();

            if (query.children[0] is not SimpleStatementContext simpleStatement)
                return null;

            switch (simpleStatement.children[0])
            {
                case SelectStatementContext selectStatement:
                    return TableVisitor.VisitSelectStatement(selectStatement);

                case CreateStatementContext createStatement:
                    return DefinitionVisitor.VisitCreateStatement(createStatement);

                case DeleteStatementContext deleteStatement:
                    return ActionVisitor.VisitDeleteStatement(deleteStatement);

                case ReplaceStatementContext replaceStatement:
                    return ActionVisitor.VisitReplaceStatement(replaceStatement);

                case UpdateStatementContext updateStatement:
                    return ActionVisitor.VisitUpdateStatement(updateStatement);

                case InsertStatementContext insertStatement:
                    return ActionVisitor.VisitInsertStatement(insertStatement);

                case UtilityStatementContext utilityStatement:
                    return ParseUtilityStatement(utilityStatement);

                default:
                    throw TreeHelper.NotSupportedTree(simpleStatement.children[0]);
            }
        }

        private IQsiTreeNode ParseUtilityStatement(UtilityStatementContext context)
        {
            switch (context.children[0])
            {
                case UseCommandContext useCommand:
                    return ActionVisitor.VisitUseCommand(useCommand);

                default:
                    throw TreeHelper.NotSupportedTree(context.children[0]);
            }
        }
    }
}
