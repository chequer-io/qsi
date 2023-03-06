using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Tree.Immutable;
using Qsi.Utilities;

namespace Qsi.Analyzers.Table;

public class QsiTableAnalyzer : QsiAnalyzerBase
{
    private const string scopeFieldList = "field list";

    public QsiTableAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    #region Execute
    public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
    {
        return
            tree is IQsiTableNode &&
            (script.ScriptType is QsiScriptType.With or QsiScriptType.Select);
    }

    protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
    {
        if (context.Tree is not IQsiTableNode tableNode)
            throw new InvalidOperationException();

        using var scope = new TableCompileContext(context);
        var table = await BuildTableStructure(scope, tableNode);

        return new QsiTableResult(table).ToSingleArray();
    }
    #endregion

    #region Table
    public virtual async ValueTask<QsiTableStructure> BuildTableStructure(TableCompileContext context, IQsiTableNode table)
    {
        context.ThrowIfCancellationRequested();

        switch (table)
        {
            case IQsiTableReferenceNode tableReference:
                return await BuildTableReferenceStructure(context, tableReference);

            case IQsiTableFunctionNode tableFunction:
                return await BuildTableFunctionStructure(context, tableFunction);

            case IQsiDerivedTableNode derivedTable:
                return await BuildDerivedTableStructure(context, derivedTable);

            case IQsiInlineDerivedTableNode inlineDerivedTableNode:
                return await BuildInlineDerivedTableStructure(context, inlineDerivedTableNode);

            case IQsiJoinedTableNode joinedTable:
                return await BuildJoinedTableStructure(context, joinedTable);

            case IQsiCompositeTableNode compositeTable:
                return await BuildCompositeTableStructure(context, compositeTable);
        }

        throw new InvalidOperationException();
    }

    protected virtual async ValueTask<QsiTableStructure> BuildTableReferenceStructure(TableCompileContext context, IQsiTableReferenceNode table)
    {
        context.ThrowIfCancellationRequested();

        var lookup = ResolveTableStructure(context, table.Identifier);

        // view
        if (context.AnalyzerOptions.UseViewTracing &&
            !lookup.IsSystem &&
            lookup.Type is QsiTableType.View or QsiTableType.MaterializedView)
        {
            var script = context.Engine.RepositoryProvider.LookupDefinition(lookup.Identifier, lookup.Type) ??
                         throw new QsiException(QsiError.UnableResolveDefinition, lookup.Identifier);

            var viewNode = context.Engine.TreeParser.Parse(script);

            using var viewCompileContext = new TableCompileContext(context);

            if (lookup.Identifier.Level > 1)
                viewCompileContext.PushIdentifierScope(lookup.Identifier.SubIdentifier(..^1));

            QsiTableStructure viewStructure;

            switch (viewNode)
            {
                case IQsiTableNode viewTableNode:
                {
                    var viewTableStructure = await BuildTableStructure(viewCompileContext, viewTableNode);
                    viewTableStructure.Identifier = ResolveQualifiedIdentifier(context, viewTableStructure.Identifier);
                    viewStructure = viewTableStructure;
                    break;
                }

                case IQsiDefinitionNode definitionNode:
                {
                    viewStructure = await BuildDefinitionStructure(viewCompileContext, definitionNode);
                    break;
                }

                default:
                    throw TreeHelper.NotSupportedTree(viewNode);
            }

            if (viewStructure.Columns.Count != lookup.Columns.Count)
                throw new QsiException(QsiError.DifferentColumnsCount, "View definition");

            lookup.References.Add(viewStructure);

            for (int i = 0; i < viewStructure.Columns.Count; i++)
                lookup.Columns[i].References.Add(viewStructure.Columns[i]);
        }

        return lookup;
    }

    protected virtual Task<QsiTableStructure> BuildTableFunctionStructure(TableCompileContext context, IQsiTableFunctionNode table)
    {
        throw TreeHelper.NotSupportedFeature("Table Function");
    }

    protected virtual async ValueTask<QsiTableStructure> BuildDerivedTableStructure(TableCompileContext context, IQsiDerivedTableNode table)
    {
        context.ThrowIfCancellationRequested();

        using var scopedContext = new TableCompileContext(context);

        // Directives

        if (table.Directives?.Tables?.Length > 0)
        {
            await BuildDirectives(scopedContext, table.Directives);
        }

        // Table Source

        var alias = table.Alias?.Name;

        if (alias == null &&
            table.Parent is IQsiDerivedColumnNode &&
            !context.AnalyzerOptions.AllowNoAliasInDerivedTable)
        {
            throw new QsiException(QsiError.NoAlias);
        }

        if (table.Source is IQsiJoinedTableNode joinedTableNode)
        {
            scopedContext.SourceTable = await BuildJoinedTableStructure(scopedContext, joinedTableNode);
        }
        else if (table.Source != null)
        {
            using var sourceContext = new TableCompileContext(scopedContext);
            scopedContext.SourceTable = await BuildTableStructure(scopedContext, table.Source);
        }

        var declaredTable = new QsiTableStructure
        {
            Type = QsiTableType.Derived,
            Identifier = alias == null ? null : new QsiQualifiedIdentifier(alias)
        };

        if (scopedContext.SourceTable != null)
            declaredTable.References.Add(scopedContext.SourceTable);

        var columns = table.Columns;

        if (columns == null || columns.Count == 0)
        {
            if (!context.AnalyzerOptions.AllowEmptyColumnsInSelect)
            {
                throw new QsiException(QsiError.Syntax);
            }
        }
        else if (columns.TryCast(out IQsiSequentialColumnNode[] sequentialColumns))
        {
            // Sequential columns definition

            if (scopedContext.SourceTable == null)
                throw new QsiException(QsiError.NoTablesUsed);

            var columnType = sequentialColumns[0].ColumnType;
            QsiTableColumn[] allColumns = scopedContext.SourceTable.VisibleColumns.ToArray();
            int columnLength = allColumns.Length;

            if (sequentialColumns.Length > allColumns.Length)
            {
                throw new QsiException(QsiError.SpecifiesMoreColumnNames);
            }

            if (columnType == QsiSequentialColumnType.Default)
            {
                if (sequentialColumns.Length != allColumns.Length)
                    throw new QsiException(QsiError.DifferentColumnsCount);

                columnLength = sequentialColumns.Length;
            }

            for (int i = 0; i < columnLength; i++)
            {
                var column = allColumns[i];
                var declaredColumn = declaredTable.NewColumn();

                declaredColumn.Name = i < sequentialColumns.Length ? sequentialColumns[i].Alias?.Name : column.Name;
                declaredColumn.References.Add(column);
            }

            if (context.AnalyzerOptions.IncludeInvisibleColumnsInAlias)
            {
                foreach (var invisibleColumn in scopedContext.SourceTable.Columns.Where(c => !c.IsVisible))
                {
                    var declaredColumn = declaredTable.NewColumn();

                    declaredColumn.Name = invisibleColumn.Name;
                    declaredColumn.IsVisible = false;
                    declaredColumn.References.Add(invisibleColumn);
                }
            }
        }
        else
        {
            // Compound columns definition

            foreach (var column in columns)
            {
                IEnumerable<QsiTableColumn> resolvedColumns = ResolveColumns(scopedContext, column, out var implicitTableWildcard);
                bool keepVisible = column is IQsiAllColumnNode;

                switch (column)
                {
                    case IQsiDerivedColumnNode derivedColum:
                    {
                        var declaredColumn = declaredTable.NewColumn();

                        declaredColumn.Name = ResolveDerivedColumnName(context, table, derivedColum);
                        declaredColumn.IsExpression = derivedColum.IsExpression;
                        declaredColumn.References.AddRange(resolvedColumns);
                        break;
                    }

                    default:
                    {
                        var allColumnNode = column as IQsiAllColumnNode;
                        var aliasedAllColumn = !ListUtility.IsNullOrEmpty(allColumnNode?.SequentialColumns);
                        int i = 0;

                        foreach (var c in resolvedColumns)
                        {
                            if (aliasedAllColumn && allColumnNode.SequentialColumns.Length < i + 1)
                                throw new QsiException(QsiError.DifferentColumnsCount);

                            var seqentialColumn = aliasedAllColumn ? allColumnNode?.SequentialColumns[i++] : null;
                            var declaredColumn = declaredTable.NewColumn();

                            declaredColumn.Name = seqentialColumn is null
                                ? ResolveCompoundColumnName(context, table, column, c)
                                : seqentialColumn.Alias.Name;

                            declaredColumn.References.Add(c);
                            declaredColumn.ImplicitTableWildcardTarget = implicitTableWildcard;

                            if (keepVisible)
                                declaredColumn.IsVisible = c.IsVisible;
                        }

                        if (aliasedAllColumn && allColumnNode.SequentialColumns.Length != i)
                            throw new QsiException(QsiError.DifferentColumnsCount);

                        break;
                    }
                }
            }
        }

        return declaredTable;
    }

    protected virtual QsiIdentifier ResolveCompoundColumnName(TableCompileContext context, IQsiDerivedTableNode table, IQsiColumnNode column, QsiTableColumn refColumn)
    {
        return refColumn.Name;
    }

    protected virtual QsiIdentifier ResolveDerivedColumnName(TableCompileContext context, IQsiDerivedTableNode table, IQsiDerivedColumnNode column)
    {
        return column.Alias?.Name ?? column.InferredName;
    }

    protected virtual ValueTask<QsiTableStructure> BuildInlineDerivedTableStructure(TableCompileContext context, IQsiInlineDerivedTableNode table)
    {
        context.ThrowIfCancellationRequested();

        var alias = table.Alias?.Name;

        if (alias == null &&
            table.Parent is IQsiDerivedColumnNode &&
            !context.AnalyzerOptions.AllowNoAliasInDerivedTable)
        {
            throw new QsiException(QsiError.NoAlias);
        }

        var declaredTable = new QsiTableStructure
        {
            Type = QsiTableType.Inline,
            Identifier = alias == null ? null : new QsiQualifiedIdentifier(alias)
        };

        int? columnCount = null;

        switch (table.Columns)
        {
            case null:
            case var cd when cd.All(c => c is IQsiAllColumnNode { Path: null }):
                // Skip
                break;

            case var cd when cd.TryCast(out IQsiSequentialColumnNode[] sequentialColumns):
                foreach (var column in sequentialColumns)
                {
                    var c = declaredTable.NewColumn();
                    c.Name = column.Alias.Name;
                }

                columnCount = sequentialColumns.Length;
                break;

            default:
                throw new NotSupportedException("Not supported columns in inline derived table.");
        }

        // Skip trace columns in expression.
        // Because don't know the possibility of declaring a referenceable column in the expression.
        // ISSUE: row.ColumnValues
        foreach (var row in table.Rows ?? Enumerable.Empty<IQsiRowValueExpressionNode>())
        {
            if (!columnCount.HasValue)
            {
                columnCount = row.ColumnValues.Length;
            }
            else if (columnCount != row.ColumnValues.Length)
            {
                throw new QsiException(QsiError.DifferentColumnsCount);
            }
        }

        if ((columnCount ?? 0) == 0)
        {
            if (!context.AnalyzerOptions.AllowEmptyColumnsInInline)
                throw new QsiException(QsiError.NoColumnsSpecified, alias);

            columnCount = 0;
        }

        if (declaredTable.Columns.Count != columnCount)
        {
            for (int i = 0; i < columnCount; i++)
            {
                declaredTable.NewColumn();
            }
        }

        return new ValueTask<QsiTableStructure>(declaredTable);
    }

    protected virtual async ValueTask<QsiTableStructure> BuildRecursiveCompositeTableStructure(TableCompileContext context, IQsiDerivedTableNode table, IQsiCompositeTableNode source)
    {
        context.ThrowIfCancellationRequested();

        var declaredTable = new QsiTableStructure
        {
            Type = QsiTableType.Derived,
            Identifier = new QsiQualifiedIdentifier(table.Alias.Name)
        };

        int sourceOffset = 0;
        var structures = new List<QsiTableStructure>(source.Sources.Length);

        if (table.Columns.Any(c => c is not IQsiAllColumnNode))
        {
            foreach (var columnNode in table.Columns.Cast<IQsiSequentialColumnNode>())
            {
                var column = declaredTable.NewColumn();
                column.Name = columnNode.Alias.Name;
            }
        }
        else
        {
            using var anchorContext = new TableCompileContext(context);
            var anchor = await BuildTableStructure(anchorContext, source.Sources[0]);

            foreach (var anchorColumn in anchor.Columns)
            {
                var column = declaredTable.NewColumn();
                column.Name = anchorColumn.Name;
            }

            sourceOffset++;
            structures.Add(anchor);
        }

        for (int i = sourceOffset; i < source.Sources.Length; i++)
        {
            using var tempContext = new TableCompileContext(context);
            tempContext.AddDirective(declaredTable);

            structures.Add(await BuildTableStructure(tempContext, source.Sources[i]));
        }

        foreach (var structure in structures)
        {
            if (declaredTable.Columns.Count != structure.Columns.Count)
                throw new QsiException(QsiError.DifferentColumnsCount);

            for (int i = 0; i < structure.Columns.Count; i++)
            {
                declaredTable.Columns[i].References.Add(structure.Columns[i]);
            }
        }

        return declaredTable;
    }

    internal async ValueTask BuildDirectives(TableCompileContext context, IQsiTableDirectivesNode directivesNode)
    {
        context.ThrowIfCancellationRequested();

        foreach (var directiveTable in directivesNode.Tables)
        {
            var cteName = directiveTable.Alias.Name;

            if (directivesNode.IsRecursive && ContainsRecursiveQuery(directiveTable.Source, cteName))
            {
                if (directiveTable.Source is not IQsiCompositeTableNode compositeTableNode)
                    throw new QsiException(QsiError.NoTopLevelUnionInRecursiveQuery, cteName);

                if (ContainsRecursiveQuery(compositeTableNode.Sources[0], cteName))
                {
                    if (!context.AnalyzerOptions.UseAutoFixRecursiveQuery || compositeTableNode.Sources.Length < 2)
                        throw new QsiException(QsiError.NoAnchorInRecursiveQuery, cteName);

                    var fixedSources = compositeTableNode.Sources
                        .Select(s => new
                        {
                            Source = s,
                            Recursive = ContainsRecursiveQuery(s, cteName)
                        })
                        .OrderBy(x => x.Recursive ? 1 : 0)
                        .ToArray();

                    if (fixedSources[0].Recursive)
                        throw new QsiException(QsiError.NoAnchorInRecursiveQuery, cteName);

                    compositeTableNode = new ImmutableCompositeTableNode(
                        compositeTableNode.Parent,
                        fixedSources
                            .Select(s => s.Source)
                            .ToArray(),
                        compositeTableNode.Order,
                        compositeTableNode.Limit,
                        compositeTableNode.CompositeType,
                        compositeTableNode.UserData);
                }

                using var directiveContext = new TableCompileContext(context);
                var directiveTableStructure = await BuildRecursiveCompositeTableStructure(context, directiveTable, compositeTableNode);
                context.AddDirective(directiveTableStructure);
            }
            else
            {
                using var directiveContext = new TableCompileContext(context);
                var directiveTableStructure = await BuildTableStructure(directiveContext, directiveTable);
                context.AddDirective(directiveTableStructure);
            }
        }
    }

    protected virtual bool ContainsRecursiveQuery(IQsiTableNode table, QsiIdentifier identifier)
    {
        foreach (var tableReference in table.FindAscendants<IQsiTableReferenceNode>())
        {
            if (tableReference.Identifier.Level == 1 && Match(tableReference.Identifier[0], identifier))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual async ValueTask<QsiTableStructure> BuildJoinedTableStructure(TableCompileContext context, IQsiJoinedTableNode table)
    {
        context.ThrowIfCancellationRequested();

        if (table.Left == null || table.Right == null)
            throw new QsiException(QsiError.Syntax);

        // priority
        // using > left > right

        var joinedTable = new QsiTableStructure
        {
            Type = QsiTableType.Join
        };

        QsiTableStructure left;
        QsiTableStructure right;

        if (table.Left is IQsiJoinedTableNode leftNode)
        {
            left = await BuildJoinedTableStructure(context, leftNode);
        }
        else
        {
            using var leftContext = new TableCompileContext(context);
            left = await BuildTableStructure(leftContext, table.Left);
            context.JoinedSouceTables.Add(left);
        }

        if (table.Right is IQsiJoinedTableNode rightNode)
        {
            right = await BuildJoinedTableStructure(context, rightNode);
        }
        else
        {
            using var rightContext = new TableCompileContext(context);
            right = await BuildTableStructure(rightContext, table.Right);
            context.JoinedSouceTables.Add(right);
        }

        joinedTable.References.Add(left);
        joinedTable.References.Add(right);

        IQsiColumnReferenceNode[] pivots = table.PivotColumns?
            .Cast<IQsiColumnReferenceNode>()
            .ToArray();

        HashSet<QsiTableColumn> leftColumns = left.VisibleColumns.ToHashSet();
        HashSet<QsiTableColumn> rightColumns = right.VisibleColumns.ToHashSet();
        var pivotColumns = new List<PivotColumnPair>();

        if (table.IsNatural)
        {
            QsiIdentifier[] leftColumnNames = left.VisibleColumns
                .Where(c => c.Name != null)
                .Select(c => c.Name)
                .ToArray();

            QsiIdentifier[] rightColumnNames = right.VisibleColumns
                .Where(c => c.Name != null)
                .Select(c => c.Name)
                .ToArray();

            QsiIdentifier[] minColumnNames = leftColumnNames.Length > rightColumnNames.Length ?
                rightColumnNames :
                leftColumnNames;

            pivots = minColumnNames
                .Select(n => (IQsiColumnReferenceNode)new ImmutableColumnReferenceNode(null, new QsiQualifiedIdentifier(n), null))
                .ToArray();
        }

        foreach (var pivot in pivots ?? Enumerable.Empty<IQsiColumnReferenceNode>())
        {
            var pivotColumnName = pivot.Name[^1];
            var leftColumnIndexes = left.Columns.AllIndexOf(c => Match(c.Name, pivotColumnName)).Take(2).ToArray();
            var rightColumnIndexes = right.Columns.AllIndexOf(c => Match(c.Name, pivotColumnName)).Take(2).ToArray();

            if (leftColumnIndexes.Length == 0 || rightColumnIndexes.Length == 0)
            {
                if (table.IsNatural)
                    continue;

                throw new QsiException(QsiError.UnableResolveColumn, pivot.Name);
            }

            if (leftColumnIndexes.Length == 2 || rightColumnIndexes.Length == 2)
                throw new QsiException(QsiError.DuplicateColumnName, pivot.Name);

            pivotColumns.Add(new PivotColumnPair(
                leftColumnIndexes[0],
                left.Columns[leftColumnIndexes[0]],
                right.Columns[rightColumnIndexes[0]]
            ));
        }

        foreach (var (_, leftColumn, rightColumn) in pivotColumns.OrderBy(p => p.Order))
        {
            var column = joinedTable.NewColumn();
            column.Name = leftColumn.Name;
            column.References.Add(leftColumn);
            column.References.Add(rightColumn);

            leftColumns.Remove(leftColumn);
            rightColumns.Remove(rightColumn);
        }

        foreach (var leftColumn in leftColumns)
        {
            var column = joinedTable.NewColumn();
            column.Name = leftColumn.Name;
            column.References.Add(leftColumn);
        }

        foreach (var rightColumn in rightColumns)
        {
            var column = joinedTable.NewColumn();
            column.Name = rightColumn.Name;
            column.References.Add(rightColumn);
        }

        return joinedTable;
    }

    protected virtual async ValueTask<QsiTableStructure> BuildCompositeTableStructure(TableCompileContext context, IQsiCompositeTableNode table)
    {
        context.ThrowIfCancellationRequested();

        if (table.Sources == null || table.Sources.Length == 0)
            throw new QsiException(QsiError.Syntax);

        var sources = new QsiTableStructure[table.Sources.Length];

        for (int i = 0; i < sources.Length; i++)
        {
            using var tempContext = new TableCompileContext(context);
            sources[i] = await BuildTableStructure(tempContext, table.Sources[i]);
        }

        QsiTableColumn[] baseVisibleColumns = sources[0].VisibleColumns.ToArray();

        if (sources.Skip(1).Any(s => s.VisibleColumns.Count() != baseVisibleColumns.Length))
            throw new QsiException(QsiError.DifferentColumnsCount);

        var compositeSource = new QsiTableStructure
        {
            Type = QsiTableType.Union
        };

        compositeSource.References.AddRange(sources);

        for (int i = 0; i < baseVisibleColumns.Length; i++)
        {
            var baseColumn = baseVisibleColumns[i];
            var compositeColumn = compositeSource.NewColumn();

            compositeColumn.Name = baseColumn.Name;

            compositeColumn.References.AddRange(
                sources.Select(s => s.VisibleColumns.ElementAt(i))
            );
        }

        return compositeSource;
    }
    #endregion

    #region Definition
    protected virtual async ValueTask<QsiTableStructure> BuildDefinitionStructure(TableCompileContext context, IQsiDefinitionNode definition)
    {
        context.ThrowIfCancellationRequested();

        switch (definition)
        {
            case IQsiViewDefinitionNode viewDefinition:
                return await BuildViewDefinitionStructure(context, viewDefinition);

            default:
                throw TreeHelper.NotSupportedTree(definition);
        }
    }

    protected internal virtual async ValueTask<QsiTableStructure> BuildViewDefinitionStructure(TableCompileContext context, IQsiViewDefinitionNode viewDefinition)
    {
        context.ThrowIfCancellationRequested();

        using var scopedContext = new TableCompileContext(context);

        // Directives

        if (viewDefinition.Directives?.Tables?.Length > 0)
            await BuildDirectives(scopedContext, viewDefinition.Directives);

        // Source

        var sourceNode = viewDefinition.Source;

        if (viewDefinition.Columns != null && !viewDefinition.Columns.IsAllColumnNode())
        {
            sourceNode = new ImmutableDerivedTableNode(
                null, null,
                viewDefinition.Columns.ToImmutable(),
                viewDefinition.Source,
                null, null, null, null, null, null
            );
        }

        // Inject identifier

        var structure = await BuildTableStructure(scopedContext, sourceNode);
        structure.Identifier = ResolveQualifiedIdentifier(scopedContext, viewDefinition.Identifier);

        return structure;
    }
    #endregion

    #region Column
    protected QsiTableColumn[] ResolveColumns(TableCompileContext context, IQsiColumnNode column, /* TODO: remove */ out QsiQualifiedIdentifier implicitTableWildcardTarget)
    {
        context.ThrowIfCancellationRequested();
        implicitTableWildcardTarget = default;

        switch (column)
        {
            case IQsiAllColumnNode allColumn:
                return ResolveAllColumns(context, allColumn, false).ToArray();

            case IQsiColumnReferenceNode columnReference:
                return ResolveColumnReference(context, columnReference, out implicitTableWildcardTarget);

            case IQsiDerivedColumnNode derivedColumn:
                return ResolveDerivedColumns(context, derivedColumn).ToArray();

            case IQsiSequentialColumnNode _:
                throw new QsiException(QsiError.SyntaxError, "You cannot define sequential column in a compound column definition.");
        }

        throw new InvalidOperationException();
    }

    protected virtual IEnumerable<QsiTableColumn> ResolveAllColumns(TableCompileContext context, IQsiAllColumnNode column, bool includeInvisible)
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

    protected virtual QsiTableColumn[] ResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column, out QsiQualifiedIdentifier implicitTableWildcardTarget)
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
                .SelectMany(s => s.Columns.Where(c => Match(c.Name, lastName) && c.IsVisible))
                .Take(2)
                .ToArray();

            // If visible column is not exists, get invisible columns
            if (columns.Length is 0)
            {
                columns = candidateSourceTables
                    .SelectMany(s => s.Columns.Where(c => Match(c.Name, lastName) && !c.IsVisible))
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

    private QsiTableColumn[] ImplicitlyResolveColumnReference(TableCompileContext context, IQsiColumnReferenceNode column, out QsiQualifiedIdentifier implicitTableWildcardTarget)
    {
        QsiTableStructure[] implicitSources = LookupDataTableStructuresInExpression(context, column.Name).ToArray();

        if (implicitSources.Length != 1)
            throw new QsiException(QsiError.UnknownColumnIn, column.Name[^1], scopeFieldList);

        implicitTableWildcardTarget = column.Name;
        return implicitSources[0].Columns.ToArray();
    }

    protected virtual IEnumerable<QsiTableColumn> ResolveDerivedColumns(TableCompileContext context, IQsiDerivedColumnNode column)
    {
        context.ThrowIfCancellationRequested();

        if (column.IsExpression)
            return ResolveColumnsInExpression(context, column.Expression);

        return ResolveColumns(context, column.Column, out _);
    }

    protected virtual IEnumerable<QsiTableColumn> ResolveColumnsInExpression(TableCompileContext context, IQsiExpressionNode expression)
    {
        context.ThrowIfCancellationRequested();

        if (expression == null)
            yield break;

        switch (expression)
        {
            case QsiExpressionFragmentNode:
                break;

            case IQsiSetColumnExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Value))
                    yield return c;

                break;
            }

            case IQsiSetVariableExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Value))
                    yield return c;

                break;
            }

            case IQsiInvokeExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Member))
                    yield return c;

                foreach (var c in ResolveColumnsInExpression(context, e.Parameters))
                    yield return c;

                break;
            }

            case IQsiLiteralExpressionNode e:
            {
                break;
            }

            case IQsiBinaryExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Left))
                    yield return c;

                foreach (var c in ResolveColumnsInExpression(context, e.Right))
                    yield return c;

                break;
            }

            case IQsiParametersExpressionNode e:
            {
                foreach (var c in e.Expressions.SelectMany(x => ResolveColumnsInExpression(context, x)))
                    yield return c;

                break;
            }

            case IQsiMultipleExpressionNode e:
            {
                foreach (var c in e.Elements.SelectMany(x => ResolveColumnsInExpression(context, x)))
                    yield return c;

                break;
            }

            case IQsiSwitchExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Value))
                    yield return c;

                foreach (var c in e.Cases.SelectMany(c => ResolveColumnsInExpression(context, c)))
                    yield return c;

                break;
            }

            case IQsiSwitchCaseExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Condition))
                    yield return c;

                foreach (var c in ResolveColumnsInExpression(context, e.Consequent))
                    yield return c;

                break;
            }

            case IQsiTableExpressionNode e:
            {
                using var scopedContext = new TableCompileContext(context);
                var structure = BuildTableStructure(scopedContext, e.Table).Result;

                foreach (var c in structure.Columns)
                    yield return c;

                break;
            }

            case IQsiUnaryExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Expression))
                    yield return c;

                break;
            }

            case IQsiColumnExpressionNode e:
            {
                switch (e.Column)
                {
                    case IQsiAllColumnNode _ when
                        e.FindDescendant<IQsiParametersExpressionNode, IQsiInvokeExpressionNode>(out _, out var i) &&
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

                break;
            }

            case IQsiMemberAccessExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Target))
                    yield return c;

                foreach (var c in ResolveColumnsInExpression(context, e.Member))
                    yield return c;

                break;
            }

            case IQsiOrderExpressionNode e:
            {
                foreach (var c in ResolveColumnsInExpression(context, e.Expression))
                    yield return c;

                break;
            }

            case IQsiVariableExpressionNode e:
            {
                // TODO: Analyze variable
                break;
            }

            case IQsiFunctionExpressionNode e:
            {
                // TODO: Analyze function
                break;
            }

            case IQsiMemberExpressionNode _:
            {
                // Skip unknown member access
                break;
            }

            case IQsiBindParameterExpressionNode:
                break;

            default:
                throw new InvalidOperationException();
        }
    }
    #endregion

    #region Table Lookup
    protected QsiQualifiedIdentifier ResolveQualifiedIdentifier(TableCompileContext context, QsiQualifiedIdentifier identifier)
    {
        context.ThrowIfCancellationRequested();

        // Qualified identifier without table
        var identifierScope = context.PeekIdentifierScope();

        if (identifierScope != null && identifier.Level <= identifierScope.Level)
        {
            int offset = identifierScope.Level - identifier.Level + 1;
            Span<QsiIdentifier> identifiers = new QsiIdentifier[identifierScope.Level + 1].AsSpan();

            identifierScope._identifiers[..offset].CopyTo(identifiers);
            identifier._identifiers.AsSpan().CopyTo(identifiers[offset..]);

            identifier = new QsiQualifiedIdentifier(identifiers.ToArray());
        }

        return context.Engine.RepositoryProvider.ResolveQualifiedIdentifier(identifier, context.ExecuteOptions);
    }

    private QsiTableStructure ResolveTableStructure(TableCompileContext context, QsiQualifiedIdentifier identifier)
    {
        context.ThrowIfCancellationRequested();
        return LookupTableStructure(context, identifier) ?? throw new QsiException(QsiError.UnableResolveTable, identifier);
    }

    private QsiTableStructure LookupTableStructure(TableCompileContext context, QsiQualifiedIdentifier identifier)
    {
        context.ThrowIfCancellationRequested();

        return
            context.Directives.FirstOrDefault(d => Match(d.Identifier, identifier)) ??
            context.Engine.RepositoryProvider.LookupTable(ResolveQualifiedIdentifier(context, identifier))?.Clone();
    }

    private IEnumerable<QsiTableStructure> LookupDataTableStructuresInExpression(TableCompileContext context, QsiQualifiedIdentifier identifier)
    {
        context.ThrowIfCancellationRequested();
        return context.GetAllSourceTables().Where(x => Match(context, x, identifier));
    }
    #endregion

    private record PivotColumnPair(int Order, QsiTableColumn Left, QsiTableColumn Right);
}
