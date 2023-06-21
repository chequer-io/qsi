using System;
using System.Threading;
using PgQuery;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PostgreSql.Tree;
using Qsi.Tree;

namespace Qsi.PostgreSql;

public class PostgreSqlParser : IQsiTreeParser
{
    public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
    {
        return PgNodeVisitor.Visit(ParseProtobuf(script).Stmt);
    }

    private RawStmt ParseProtobuf(QsiScript script)
    {
        ParseResult parseResult;

        try
        {
            parseResult = Parser.Parse(script.Script);
        }
        catch (Exception e) when (e is ArgumentNullException or DllNotFoundException or BadImageFormatException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new QsiException(QsiError.SyntaxError, e.Message);
        }

        if (parseResult.Stmts.Count == 0)
            throw new QsiException(QsiError.Syntax);

        return parseResult.Stmts[0];
    }
}