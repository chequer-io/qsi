using System.Threading;
using Antlr4.Runtime;
using Qsi.Cql.Internal;
using Qsi.Cql.Tree;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Cql.Internal.CqlParserInternal;

namespace Qsi.Cql;

public sealed class CqlParser : IQsiTreeParser
{
    public static CqlParser Instance => _instance ??= new CqlParser();

    private static CqlParser _instance;

    private CqlParser()
    {
    }

    public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
    {
        var stream = new AntlrInputStream(script.Script);
        var lexer = new CqlLexerInternal(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new CqlParserInternal(tokens);
        parser.AddErrorListener(new ErrorListener());

        var statement = parser.cqlStatement().children[0];

        switch (statement)
        {
            case SelectStatementContext selectStatement:
                return TableVisitor.VisitSelectStatement(selectStatement);

            case CreateMaterializedViewStatementContext createMaterializedViewStatement:
                return DefinitionVisitor.VisitCreateMaterializedViewStatement(createMaterializedViewStatement);

            case UseStatementContext useStatement:
                return ActionVisitor.VisitUseStatement(useStatement);

            case InsertStatementContext insertStatement:
                return ActionVisitor.VisitInsertStatement(insertStatement);

            case UpdateStatementContext updateStatement:
                return ActionVisitor.VisitUpdateStatement(updateStatement);

            case DeleteStatementContext deleteStatement:
                return ActionVisitor.VisitDeleteStatement(deleteStatement);

            default:
                throw TreeHelper.NotSupportedTree(statement);
        }
    }
}