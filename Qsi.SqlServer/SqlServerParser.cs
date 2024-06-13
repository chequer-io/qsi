using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.SqlServer.Common;
using Qsi.SqlServer.Internal;
using Qsi.SqlServer.Tree;
using Qsi.Tree;
using ManagementTransactSqlVersion = Microsoft.SqlServer.Management.SqlParser.Common.TransactSqlVersion;

namespace Qsi.SqlServer;

public sealed class SqlServerParser : IQsiTreeParser, IVisitorContext
{
    #region IContext
    TableVisitor IVisitorContext.TableVisitor => _tableVisitor;

    ExpressionVisitor IVisitorContext.ExpressionVisitor => _expressionVisitor;

    IdentifierVisitor IVisitorContext.IdentifierVisitor => _identifierVisitor;

    ActionVisitor IVisitorContext.ActionVisitor => _actionVisitor;

    DefinitionVisitor IVisitorContext.DefinitionVisitor => _definitionVisitor;

    private readonly TableVisitor _tableVisitor;
    private readonly ExpressionVisitor _expressionVisitor;
    private readonly IdentifierVisitor _identifierVisitor;
    private readonly ActionVisitor _actionVisitor;
    private readonly DefinitionVisitor _definitionVisitor;
    #endregion

    private readonly TSqlParserInternal _parser;
    private readonly AlternativeParserInternal _alternativeParser;

    public SqlServerParser(TransactSqlVersion transactSqlVersion)
    {
        _parser = new TSqlParserInternal(transactSqlVersion, false);
        _alternativeParser = new AlternativeParserInternal(transactSqlVersion);

        _tableVisitor = CreateTableVisitor();
        _expressionVisitor = CreateExpressionVisitor();
        _identifierVisitor = CreateIdentifierVisitor();
        _actionVisitor = CreateActionVisitor();
        _definitionVisitor = CreateDefinitionVisitor();
    }

    private TableVisitor CreateTableVisitor()
    {
        return new(this);
    }

    private ExpressionVisitor CreateExpressionVisitor()
    {
        return new(this);
    }

    private IdentifierVisitor CreateIdentifierVisitor()
    {
        return new(this);
    }

    private ActionVisitor CreateActionVisitor()
    {
        return new(this);
    }

    private DefinitionVisitor CreateDefinitionVisitor()
    {
        return new(this);
    }

    private void PatchPhysloc(IEnumerable<SqlCodeObject> nodes, StringBuilder builder)
    {
        IEnumerable<Range> physlocRanges = nodes
            .OfType<SqlNullScalarExpression>()
            .Where(n =>
            {
                string sql = string.Concat(n.Sql.Where(c => !char.IsWhiteSpace(c)));

                return sql.Equals("%%physloc%%", StringComparison.InvariantCultureIgnoreCase);
            })
            .Select(n => new Range(n.StartLocation.Offset, n.EndLocation.Offset));

        _expressionVisitor.PhyslocRanges.AddRange(physlocRanges);

        foreach (var range in physlocRanges)
        {
            int offset = range.Start.Value;
            int length = range.End.Value - range.Start.Value;

            builder.Remove(offset, length);
            builder.Insert(offset, $@"""{new string(' ', length - 2)}""");
        }
    }

    public IQsiTreeNode Parse(QsiScript script, CancellationToken cancellationToken = default)
    {
        TSqlFragment result;

        _expressionVisitor.PhyslocRanges.Clear();

        try
        {
            result = _parser.Parse(script.Script);
        }
        catch (Exception)
        {
            // ISSUE: %%physloc%% cannot parsed in Microsoft.SqlServer.TransactSql.ScriptDom Parser
            if (!script.Script.Contains("physloc", StringComparison.InvariantCultureIgnoreCase))
                throw;

            if (!_alternativeParser.TryParse(script.Script, out var alternativeResult))
                throw;

            IEnumerable<SqlCodeObject> nodes = FlattenNode(alternativeResult);
            var builder = new StringBuilder(script.Script);

            PatchPhysloc(nodes, builder);

            result = _parser.Parse(builder.ToString());
        }

        if (result is TSqlScript sqlScript)
        {
            var batch = sqlScript.Batches.FirstOrDefault();

            var statement = batch?.Statements?.FirstOrDefault()
                            ?? throw new QsiException(QsiError.Syntax);

            switch (statement)
            {
                case UseStatement useStatement:
                    return _actionVisitor.VisitUseStatement(useStatement);

                case InsertStatement insertStatement:
                    return _actionVisitor.VisitInsertStatement(insertStatement);

                case DeleteStatement deleteStatement:
                    return _actionVisitor.VisitDeleteStatement(deleteStatement);

                case UpdateStatement updateStatement:
                    return _actionVisitor.VisitUpdateStatement(updateStatement);

                case MergeStatement mergeStatement:
                    return _actionVisitor.VisitMergeStatement(mergeStatement);

                case AlterUserStatement alterUserStatement:
                    return _actionVisitor.VisitAlterUser(alterUserStatement);

                case ViewStatementBody viewStatementBody:
                    return _definitionVisitor.VisitViewStatementBody(viewStatementBody);

                case SetVariableStatement setVariableStatement:
                    return _actionVisitor.VisitSetVariableStatement(setVariableStatement);

                default:
                    return _tableVisitor.Visit(statement);
            }
        }

        throw new InvalidOperationException();
    }

    private IEnumerable<SqlCodeObject> FlattenNode(SqlCodeObject rootNode)
    {
        var queue = new Queue<SqlCodeObject>();

        foreach (var child in rootNode.Children)
        {
            queue.Enqueue(child);

            while (queue.TryDequeue(out var node))
            {
                foreach (var childNode in node.Children)
                    queue.Enqueue(childNode);

                yield return node;
            }
        }
    }
}
