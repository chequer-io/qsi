using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers.Extensions;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Extensions;
using Qsi.Services;
using Qsi.Shared.Extensions;
using Qsi.Tree;

namespace Qsi.Analyzers.Expression;

public class ColumnResolver : ExpressionResolver<QsiTableColumn>
{
    public delegate ValueTask<QsiTableStructure> BuildTableStructureDelegate(TableCompileContext context, IQsiTableNode table);

    private const string scopeFieldList = "field list";

    protected readonly IQsiLanguageService _languageService;
    protected readonly BuildTableStructureDelegate _buildTableStructureDelegate;

    public ColumnResolver(IQsiLanguageService languageService, BuildTableStructureDelegate buildTableStructureDelegate)
    {
        _languageService = languageService;
        _buildTableStructureDelegate = buildTableStructureDelegate;
    }

    protected override IEnumerable<QsiTableColumn> ResolveTableExpression(TableCompileContext context, IQsiTableExpressionNode expression)
    {
        using var scopedContext = new TableCompileContext(context);
        var structure = _buildTableStructureDelegate(scopedContext, expression.Table).Result;

        foreach (var c in structure.Columns)
            yield return c;
    }

    protected override IEnumerable<QsiTableColumn> ResolveColumnExpression(TableCompileContext context, IQsiColumnExpressionNode expression)
    {
        switch (expression.Column)
        {
            case IQsiAllColumnNode _ when
                expression.FindDescendant<IQsiParametersExpressionNode, IQsiInvokeExpressionNode>(out _, out var i) &&
                i.Member != null &&
                i.Member.Identifier.Level == 1 &&
                i.Member.Identifier[0].Value.EqualsIgnoreCase("COUNT"):
                yield break;

            case IQsiAllColumnNode allColumnNode:
            {
                foreach (var column in ResolveAllColumns(context, allColumnNode, true))
                    yield return column;

                break;
            }

            case IQsiColumnReferenceNode columnReferenceNode:
                foreach (var column in ResolveColumnReference(context, columnReferenceNode, out _))
                    yield return column;

                break;

            default:
                throw new InvalidOperationException();
        }
    }

    private IEnumerable<QsiTableColumn> ResolveAllColumns(TableCompileContext context, IQsiAllColumnNode column, bool includeInvisible)
    {
        context.ThrowIfCancellationRequested();

        includeInvisible |= column.IncludeInvisibleColumns;

        // *
        if (column.Path == null)
        {
            if (context.SourceTable == null)
                throw new QsiException(QsiError.NoTablesUsed);

            return includeInvisible ?
                context.SourceTable.Columns :
                context.SourceTable.VisibleColumns;
        }

        // path.or.alias.*

        IEnumerable<QsiTableStructure> tables = LookupDataTableStructuresInExpression(context, column.Path);

        if (!tables.Any())
            throw new QsiException(QsiError.UnknownTable, column.Path);

        return tables.SelectMany(t => includeInvisible ? t.Columns : t.VisibleColumns);
    }

    private IEnumerable<QsiTableColumn> ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column, out QsiQualifiedIdentifier implicitTableWildcardTarget)
    {
        context.ThrowIfCancellationRequested();

        if (column.Name.Level == 0)
            throw new QsiException(QsiError.UnknownColumnIn, "null", scopeFieldList);

        IEnumerable<TableCompileContext> candidateContexts =
            context.AnalyzerOptions.UseOuterQueryColumn ? context.AncestorsAndSelf() : new[] { context };

        var lastName = column.Name[^1];

        foreach (var candidateContext in candidateContexts)
        {
            if (candidateContext.SourceTable == null)
                continue;

            IEnumerable<QsiTableStructure> candidateSourceTables;

            if (column.Name.Level > 1)
            {
                var identifier = new QsiQualifiedIdentifier(column.Name[..^1]);

                candidateSourceTables = LookupDataTableStructuresInExpression(candidateContext, identifier);

                if (!candidateSourceTables.Any())
                    continue;
            }
            else
            {
                candidateSourceTables = new[] { candidateContext.SourceTable };
            }

            QsiTableColumn[] columns = candidateSourceTables
                .SelectMany(s => s.Columns.Where(c => _languageService.MatchIdentifier(c.Name, lastName) && c.IsVisible))
                .Take(2)
                .ToArray();

            // If visible column is not exists, get invisible columns
            if (columns.Length is 0)
            {
                columns = candidateSourceTables
                    .SelectMany(s => s.Columns.Where(c => _languageService.MatchIdentifier(c.Name, lastName) && !c.IsVisible))
                    .Take(2)
                    .ToArray();
            }

            switch (columns.Length)
            {
                case 0:
                    break;

                case > 1:
                    throw new QsiException(QsiError.AmbiguousColumnIn, column.Name, scopeFieldList);

                default:
                    implicitTableWildcardTarget = default;
                    return new[] { columns[0] };
            }
        }

        // NOTE: 'SELECT actor FROM actor' same as
        //       'SELECT actor.* FROM actor'
        if (context.AnalyzerOptions.UseImplicitTableWildcardInSelect)
            return ImplicitlyResolveColumnReference(context, column, out implicitTableWildcardTarget);

        throw new QsiException(QsiError.UnknownColumnIn, lastName.Value, scopeFieldList);
    }

    private IEnumerable<QsiTableStructure> LookupDataTableStructuresInExpression(TableCompileContext context, QsiQualifiedIdentifier identifier)
    {
        context.ThrowIfCancellationRequested();
        return context.GetAllSourceTables().Where(x => _languageService.MatchIdentifier(context, x, identifier));
    }

    private QsiTableColumn[] ImplicitlyResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column, out QsiQualifiedIdentifier implicitTableWildcardTarget)
    {
        QsiTableStructure[] implicitSources = LookupDataTableStructuresInExpression(context, column.Name).ToArray();

        if (implicitSources.Length != 1)
            throw new QsiException(QsiError.UnknownColumnIn, column.Name[^1], scopeFieldList);

        implicitTableWildcardTarget = column.Name;
        return implicitSources[0].Columns.ToArray();
    }
}
