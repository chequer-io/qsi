using System.Threading;
using Antlr4.Runtime;
using PrimarSql.Internal;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.Parsing.Antlr;
using Qsi.PrimarSql.Tree;
using Qsi.Tree;
using static PrimarSql.Internal.PrimarSqlParser;

namespace Qsi.PrimarSql;

public class PrimarSqlParser : AntlrParserBase
{
    public static PrimarSqlParser Instance => _instance ??= new PrimarSqlParser();

    private static PrimarSqlParser _instance;

    private PrimarSqlParser()
    {
    }

    public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
    {
        return ((IQsiTreeParser)this).Parse(script, cancellationToken);
    }

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
        var rootContext = primarSqlParser.root();

        if (rootContext.children[0] is not SqlStatementContext sqlStatement)
            return null;

        if (sqlStatement.children[0] is not DmlStatementContext dmlStatement)
            return null;

        switch (dmlStatement.children[0])
        {
            case SelectStatementContext selectStatement:
                return TableVisitor.VisitSelectStatement(selectStatement);

            case InsertStatementContext insertStatement:
                return ActionVisitor.VisitInsertStatement(insertStatement);

            case DeleteStatementContext deleteStatement:
                return ActionVisitor.VisitDeleteStatement(deleteStatement);

            case UpdateStatementContext updateStatement:
                return ActionVisitor.VisitUpdateStatement(updateStatement);

            default:
                return null;
        }
    }
}