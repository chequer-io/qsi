using System;
using System.Threading;
using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Hana.Internal;
using Qsi.Hana.Tree.Visitors;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Hana.Internal.HanaParserInternal;

namespace Qsi.Hana
{
    public sealed class HanaParser : IQsiTreeParser
    {
        public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
        {
            var stream = new AntlrInputStream(script.Script);
            var lexer = new HanaLexerInternal(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new HanaParserInternal(tokens);
            parser.AddErrorListener(new ErrorListener());

            var statement = parser.hanaStatement();

            switch (statement.children[0])
            {
                case DataManipulationStatementContext dataManipulationStatement:
                    return ParseDataManipulationStatement(dataManipulationStatement);

                case DataDefinitionStatementContext dataDefinitionStatement:
                    return ParseDataDefinitionStatementStatement(dataDefinitionStatement);

                case SessionManagementStatementContext sessionManagementStatement:
                    return ParseSessionManagementStatement(sessionManagementStatement);

                default:
                    throw TreeHelper.NotSupportedTree(statement.children[0]);
            }
        }

        private IQsiTreeNode ParseDataManipulationStatement(DataManipulationStatementContext statement)
        {
            switch (statement.children[0])
            {
                case SelectStatementContext selectStatement:
                    return TableVisitor.VisitSelectStatement(selectStatement);

                case SelectIntoStatementContext selectIntoStatement:
                    return ActionVisitor.VisitSelectIntoStatement(selectIntoStatement);

                case DeleteStatementContext deleteStatement:
                    return ActionVisitor.VisitDeleteStatement(deleteStatement);

                case InsertStatementContext insertStatement:
                    return ActionVisitor.VisitInsertStatement(insertStatement);

                case ReplaceStatementContext replaceStatement:
                    return ActionVisitor.VisitReplaceStatement(replaceStatement);

                case UpdateStatementContext updateStatement:
                    return ActionVisitor.VisitUpdateStatement(updateStatement);

                case MergeDeltaParameterContext mergeDeltaParameter:
                    throw new NotImplementedException();

                case MergeIntoStatementContext mergeIntoStatement:
                    throw new NotImplementedException();

                default:
                    throw TreeHelper.NotSupportedTree(statement.children[0]);
            }
        }

        private IQsiDefinitionNode ParseDataDefinitionStatementStatement(DataDefinitionStatementContext statement)
        {
            switch (statement.children[0])
            {
                case CreateViewStatementContext createViewStatement:
                    return DefinitionVisitor.VisitCreateViewStatement(createViewStatement);

                default:
                    throw TreeHelper.NotSupportedTree(statement.children[0]);
            }
        }

        private IQsiActionNode ParseSessionManagementStatement(SessionManagementStatementContext statement)
        {
            switch (statement.children[0])
            {
                case SetSchemaStatementContext setSchemaStatement:
                    return ActionVisitor.VisitSetSchemaStatement(setSchemaStatement);

                default:
                    throw TreeHelper.NotSupportedTree(statement.children[0]);
            }
        }
    }
}
