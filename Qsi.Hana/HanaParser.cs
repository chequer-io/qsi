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
                    return DataDefinitionStatementStatement(dataDefinitionStatement);

                default:
                    throw TreeHelper.NotSupportedTree(statement.children[0]);
            }
        }

        private IQsiTableNode ParseDataManipulationStatement(DataManipulationStatementContext statement)
        {
            switch (statement.children[0])
            {
                case SelectStatementContext selectStatement:
                    return TableVisitor.VisitSelectStatement(selectStatement);

                case SelectIntoStatementContext selectIntoStatement:
                    throw new NotImplementedException();

                case DeleteStatementContext deleteStatement:
                    throw new NotImplementedException();

                case InsertStatementContext insertStatement:
                    throw new NotImplementedException();

                case ReplaceStatementContext replaceStatement:
                    throw new NotImplementedException();

                case UpdateStatementContext updateStatement:
                    throw new NotImplementedException();

                case MergeDeltaParameterContext mergeDeltaParameter:
                    throw new NotImplementedException();

                case MergeIntoStatementContext mergeIntoStatement:
                    throw new NotImplementedException();

                default:
                    throw TreeHelper.NotSupportedTree(statement.children[0]);
            }
        }

        private IQsiDefinitionNode DataDefinitionStatementStatement(DataDefinitionStatementContext statement)
        {
            switch (statement.children[0])
            {
                case CreateViewStatementContext createViewStatement:
                    return DefinitionVisitor.VisitCreateViewStatement(createViewStatement);

                default:
                    throw TreeHelper.NotSupportedTree(statement.children[0]);
            }
        }
    }
}
