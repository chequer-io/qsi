using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qsi.Analyzers.Action.Context;
using Qsi.Analyzers.Action.Models;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Extensions;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Tree.Data;
using Qsi.Tree.Immutable;
using Qsi.Utilities;

namespace Qsi.Analyzers.Action;

public class QsiActionAnalyzer : QsiAnalyzerBase
{
    protected delegate QsiDataValue DataValueSelector(DataManipulationTargetDataPivot pivot);

    public QsiActionAnalyzer(QsiEngine engine) : base(engine)
    {
    }

    public override bool CanExecute(QsiScript script, IQsiTreeNode tree)
    {
        return tree is IQsiActionNode;
    }

    protected override async ValueTask<IQsiAnalysisResult[]> OnExecute(IAnalyzerContext context)
    {
        switch (context.Tree)
        {
            case IQsiPrepareActionNode prepareActionNode:
                return await ExecutePrepareAction(context, prepareActionNode);

            case IQsiDropPrepareActionNode dropPrepareAction:
                return await ExecuteDropPrepareAction(context, dropPrepareAction);

            case IQsiExecutePrepareActionNode executeAction:
                return await ExecuteExecutePrepareAction(context, executeAction);

            case IQsiDataInsertActionNode dataInsertAction:
                return await ExecuteDataInsertAction(context, dataInsertAction);

            case IQsiDataDeleteActionNode dataDeleteAction:
                return await ExecuteDataDeleteAction(context, dataDeleteAction);

            case IQsiDataUpdateActionNode dataUpdateAction:
                return await ExecuteDataUpdateAction(context, dataUpdateAction);

            case IQsiChangeSearchPathActionNode searchPathAction:
                return await ExecuteSearchPathAction(context, searchPathAction);

            case IQsiCreateUserActionNode createUserAction:
                return await ExecuteCreateUserAction(context, createUserAction);

            case IQsiAlterUserActionNode alterUserAction:
                return await ExecuteAlterUserAction(context, alterUserAction);

            case IQsiGrantUserActionNode grantUserAction:
                return await ExecuteGrantUserAction(context, grantUserAction);

            case IQsiVariableSetActionNode setAction:
                return await ExecuteVariableSetAction(context, setAction);

            default:
                throw TreeHelper.NotSupportedTree(context.Tree);
        }
    }

    #region Prepared
    protected virtual ValueTask<IQsiAnalysisResult[]> ExecutePrepareAction(IAnalyzerContext context, IQsiPrepareActionNode action)
    {
        string query;

        switch (action.Query)
        {
            case IQsiLiteralExpressionNode { Type: QsiDataType.String } literal:
                query = literal.Value?.ToString();
                break;

            case IQsiVariableExpressionNode variableAccess:
                var variable = context.Engine.RepositoryProvider.LookupVariable(variableAccess.Identifier) ??
                               throw new QsiException(QsiError.UnknownVariable, variableAccess.Identifier);

                if (variable.Type != QsiDataType.String)
                    throw new InvalidOperationException();

                query = variable.Value.ToString();
                break;

            default:
                throw new InvalidOperationException();
        }

        var scriptType = context.Engine.ScriptParser.GetSuitableType(query);

        var refAction = new QsiReferenceAction
        {
            Type = QsiReferenceType.Prepared,
            Operation = QsiReferenceOperation.Create,
            IsolationLevel = QsiReferenceIsolationLevel.Session,
            Target = action.Identifier,
            Definition = new QsiScript(query, scriptType)
        };

        return refAction.ToSingleArray().AsValueTask();
    }

    protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteDropPrepareAction(IAnalyzerContext context, IQsiDropPrepareActionNode action)
    {
        var refAction = new QsiReferenceAction
        {
            Type = QsiReferenceType.Prepared,
            Operation = QsiReferenceOperation.Delete,
            IsolationLevel = QsiReferenceIsolationLevel.Session,
            Target = action.Identifier
        };

        return refAction.ToSingleArray().AsValueTask();
    }

    protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteExecutePrepareAction(IAnalyzerContext context, IQsiExecutePrepareActionNode action)
    {
        var definition = context.Engine.RepositoryProvider.LookupDefinition(action.Identifier, QsiTableType.Prepared) ??
                         throw new QsiException(QsiError.UnableResolveDefinition, action.Identifier);

        return context.Engine.Execute(definition, null, context.ExecuteOptions);
    }
    #endregion

    #region Table Data
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected virtual QsiDataValue ResolveDefaultColumnValue(DataManipulationTargetDataPivot pivot)
    {
        if (pivot.DestinationColumn.Default != null)
            return new QsiDataValue(pivot.DestinationColumn.Default, QsiDataType.Raw);

        return QsiDataValue.Default;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected virtual QsiDataValue ResolveColumnValue(IAnalyzerContext context, IQsiExpressionNode expression)
    {
        switch (expression)
        {
            case IQsiLiteralExpressionNode literal:
                return new QsiDataValue(literal.Value, literal.Type);

            case IQsiBindParameterExpressionNode bindParameter:
            {
                if (!context.Parameters.TryGetValue(bindParameter, out var parameter))
                {
                    var parameterName = context.Engine.TreeDeparser.Deparse(bindParameter, context.Script);
                    throw new QsiException(QsiError.ParameterNotFound, parameterName);
                }

                return new QsiDataValue(parameter.Value, QsiDataType.Object);
            }

            default:
                return ResolveRawColumnValue(context, expression);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected virtual QsiDataValue ResolveRawColumnValue(IAnalyzerContext context, IQsiExpressionNode expression)
    {
        IQsiTableExpressionNode[] subqueries = CollectSubqueries(expression).ToArray();

        try
        {
            if (subqueries.Length > 0)
                return ResolveSubqueryColumnValue(context, expression, subqueries);
        }
        catch
        {
            // The subquery does not yet support outer data source access.
            // So when an error occurs, it ignores the exception.
        }

        var value = context.Engine.TreeDeparser.Deparse(expression, context.Script);
        return new QsiDataValue(value, QsiDataType.Raw);

        IEnumerable<IQsiTableExpressionNode> CollectSubqueries(IQsiExpressionNode node)
        {
            var queue = new Queue<IQsiTreeNode>();
            queue.Enqueue(node);

            while (queue.TryDequeue(out var item))
            {
                if (item is IQsiTableExpressionNode tNode)
                {
                    yield return tNode;
                }
                else
                {
                    foreach (var child in item.Children ?? Enumerable.Empty<IQsiTreeNode>())
                        queue.Enqueue(child);
                }
            }
        }
    }

    private QsiDataValue ResolveSubqueryColumnValue(IAnalyzerContext context, IQsiExpressionNode expression, IEnumerable<IQsiTableExpressionNode> subqueries)
    {
        // ex) PostgreSql
        if (expression.UserData?.GetData(QsiNodeProperties.Span) is not { } span)
            throw TreeHelper.NotSupportedFeature("subqueries value");

        var subqueriesWithSpan = subqueries
            .Select(x => new { Node = x, Span = x.UserData?.GetData(QsiNodeProperties.Span) })
            .OrderBy(x => x.Span is { } s ? s.Start.GetOffset(context.Script.Script.Length) : -1)
            .ToArray();

        var builder = new StringBuilder(context.Script.Script[span]);
        var baseOffset = span.Start.GetOffset(context.Script.Script.Length);
        var offset = 0;

        foreach (var x in subqueriesWithSpan)
        {
            if (x.Span is null)
                continue;

            var (sOffset, sLength) = x.Span.Value.GetOffsetAndLength(context.Script.Script.Length);
            var index = sOffset - baseOffset + offset;
            string computedValue = ComputeSubqueryValue(context, x.Node);

            builder.Remove(index, sLength);
            builder.Insert(index, computedValue);

            offset += computedValue.Length - sLength;
        }

        return new QsiDataValue(builder.ToString(), QsiDataType.Raw);
    }

    private string ComputeSubqueryValue(IAnalyzerContext context, IQsiTableExpressionNode node)
    {
        var query = context.Engine.TreeDeparser.Deparse(node.Table, context.Script);

        // TODO: bind parameters in directives
        var directivesNode = context.Tree switch
        {
            IQsiDataUpdateActionNode { Target: IQsiDerivedTableNode updateDerivedTableNode } => updateDerivedTableNode.Directives,
            IQsiDataDeleteActionNode { Target: IQsiDerivedTableNode deleteDerivedTableNode } => deleteDerivedTableNode.Directives,
            IQsiDataInsertActionNode { Directives: { } insertDirectives } => insertDirectives,
            _ => null
        };

        var parameters = new List<QsiParameter>();

        if (directivesNode is not null)
        {
            query = $"{context.Engine.TreeDeparser.Deparse(directivesNode, context.Script)}\n{query}";
            parameters.AddRange(ArrangeBindParameters(context, directivesNode));
        }

        var scriptType = context.Engine.ScriptParser.GetSuitableType(query);
        var script = new QsiScript(query, scriptType);
        parameters.AddRange(ArrangeBindParameters(context, node.Table));

        using var reader = context.Engine.RepositoryProvider.GetDataReaderAsync(
            script,
            parameters.ToArray(),
            context.ExecuteOptions,
            default
        ).Result;

        if (!reader.Read())
            return "null";

        var builder = new StringBuilder();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (i > 0)
                builder.Append(',');

            builder.Append(FormatValue(reader.GetValue(i)));
        }

        if (reader.Read())
            throw new QsiException(QsiError.SubqueryReturnsMoreThanRow, 1);

        // 1, 2 -> (1, 2)
        if (reader.FieldCount > 1)
        {
            builder.Insert(0, '(');
            builder.Append(')');
        }

        return builder.ToString();

        static string FormatValue(object value)
        {
            return value switch
            {
                null => "null",
                string str => str,
                _ => value.ToString()
            };
        }
    }

    protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargets(IAnalyzerContext context, QsiTableStructure table)
    {
        return ResolveDataManipulationTargets(context, ResolveColumnTargetsFromTable(context, table));
    }

    protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargets(IAnalyzerContext context, IEnumerable<ColumnTarget> columnTargets)
    {
        IEnumerable<DataManipulationTargetDataPivot> rawPivots = columnTargets
            .Select(ResolveDataManipulationTargetColumn);

        return ResolveDataManipulationTargetsCore(context, rawPivots);
    }

    protected virtual IEnumerable<DataManipulationTarget> ResolveDataManipulationTargetsCore(IAnalyzerContext context, IEnumerable<DataManipulationTargetDataPivot> rawPivots)
    {
        return rawPivots
            .GroupBy(c => c.DestinationColumn.Parent)
            .Select(g =>
            {
                var buffer = new DataManipulationTargetDataPivot[g.Key.Columns.Count];

                foreach (var pivot in g)
                    buffer[pivot.DestinationOrder] = pivot;

                for (int i = 0; i < buffer.Length; i++)
                {
                    ref var dataPivot = ref buffer[i];
                    dataPivot ??= new DataManipulationTargetDataPivot(null, i, g.Key.Columns[i], -1, null);
                }

                return new DataManipulationTarget(g.Key, buffer, context.Engine.CacheProviderFactory);
            });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected virtual DataManipulationTargetDataPivot ResolveDataManipulationTargetColumn(ColumnTarget columnTarget)
    {
        if (columnTarget.SourceColumn.IsAnonymous)
            throw new QsiException(QsiError.NotUpdatableColumn, "anonymous");

        if (columnTarget.SourceColumn.IsBinding || columnTarget.SourceColumn.IsExpression || !columnTarget.SourceColumn.IsVisible)
            throw new QsiException(QsiError.NotUpdatableColumn, columnTarget.SourceColumn.Name);

        return new DataManipulationTargetDataPivot(
            columnTarget,
            columnTarget.AffectedColumn.Parent.Columns.IndexOf(columnTarget.AffectedColumn),
            columnTarget.AffectedColumn,
            columnTarget.SourceColumn.Parent.Columns.IndexOf(columnTarget.SourceColumn),
            columnTarget.SourceColumn
        );
    }

    protected virtual QsiTableColumn[] GetAffectedColumns(DataManipulationTarget target)
    {
        return target.DataPivots
            .Where(p => p.SourceColumn != null)
            .Select(p => p.DestinationColumn)
            .ToArray();
    }

    protected IEnumerable<QsiTableColumn> GetAffectedColumns(QsiTableColumn column)
    {
        return QsiUtility.FlattenColumns(column)
            .Where(x =>
            {
                if (x.Parent is not { } parent)
                    return false;

                return parent.Type == QsiTableType.Table;
            });
    }

    protected virtual IQsiTableNode ReassembleCommonTableNode(IQsiTableNode node)
    {
        switch (node)
        {
            case IQsiDerivedTableNode derivedTable:
                return new ImmutableDerivedTableNode(
                    derivedTable.Parent,
                    derivedTable.Directives,
                    derivedTable.Columns,
                    derivedTable.Source,
                    null,
                    derivedTable.Where,
                    derivedTable.Grouping,
                    derivedTable.Order,
                    derivedTable.Limit,
                    null);

            case IQsiCompositeTableNode compositeTable:
                return compositeTable.ToImmutable(true);

            case IQsiTableFunctionNode tableFunction:
                return tableFunction.ToImmutable(true);

            default:
                return new ImmutableDerivedTableNode(
                    node.Parent,
                    null,
                    TreeHelper.CreateAllColumnsDeclaration(),
                    node,
                    null, null, null, null, null, null);
        }
    }

    protected virtual QsiParameter[] ArrangeBindParameters(IAnalyzerContext context, IQsiTreeNode node)
    {
        IQsiBindParameterExpressionNode[] parameterNodes = node
            .FindAscendants<IQsiBindParameterExpressionNode>()
            .ToArray();

        var result = new QsiParameter[parameterNodes.Length];
        var index = 0;

        foreach (var parameterNode in parameterNodes)
        {
            if (!context.Parameters.TryGetValue(parameterNode, out var parameter))
            {
                var parameterName = context.Engine.TreeDeparser.Deparse(parameterNode, context.Script);
                throw new QsiException(QsiError.ParameterNotFound, parameterName);
            }

            result[index++] = parameter;
        }

        return result;
    }

    protected virtual async ValueTask<QsiDataTable> GetDataTableByCommonTableNode(IAnalyzerContext context, IQsiTableNode commonTableNode)
    {
        var query = context.Engine.TreeDeparser.Deparse(commonTableNode, context.Script);
        var scriptType = context.Engine.ScriptParser.GetSuitableType(query);
        var script = new QsiScript(query, scriptType);
        QsiParameter[] parameters = ArrangeBindParameters(context, commonTableNode);

        return (await context.Engine.RepositoryProvider.GetDataTable(
            script,
            parameters,
            context.ExecuteOptions,
            context.CancellationToken
        )).CloneVisibleOnly();
    }
    #endregion

    #region Insert, Replace
    protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteDataInsertAction(IAnalyzerContext context, IQsiDataInsertActionNode action)
    {
        var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
        QsiTableStructure table;

        using (var tableContext = new TableCompileContext(context))
        {
            table = (await tableAnalyzer.BuildTableStructure(tableContext, action.Target)).CloneVisibleOnly();
        }

        ColumnTarget[] columnTargets = await ResolveColumnTargetsFromDataInsertActionAsync(context, table, action);
        var columnWithInvalidDefault = ResolveNotNullableColumnWithInvalidDefault(table.Columns, columnTargets);

        if (columnWithInvalidDefault is not null)
            throw new QsiException(QsiError.NotNullConstraints, columnWithInvalidDefault.Name.Value);

        var dataContext = new TableDataInsertContext(context, table)
        {
            ColumnTargets = columnTargets,
            Targets = ResolveDataManipulationTargets(context, columnTargets).ToArray()
        };

        if (action.ValueTable != null)
        {
            await ProcessQueryValues(dataContext, action.Directives, action.ValueTable);
        }
        else if (!ListUtility.IsNullOrEmpty(action.Values))
        {
            ProcessValues(dataContext, action.Values);
        }
        else if (!ListUtility.IsNullOrEmpty(action.SetValues))
        {
            ProcessSetValues(dataContext, action.SetValues);
        }
        else
        {
            throw new QsiException(QsiError.Syntax);
        }

        if (action.ConflictAction != null && action.ConflictBehavior != QsiDataConflictBehavior.Ignore)
        {
            ProcessConflict(dataContext, action.ConflictAction, action.ConflictBehavior);
        }

        return dataContext.Targets
            .Select(t => new QsiDataManipulationResult
            {
                Table = t.Table,
                AffectedColumns = GetAffectedColumns(t),
                InsertRows = t.InsertRows.ToNullIfEmpty(),
                DuplicateRows = t.DuplicateRows.ToNullIfEmpty()
            })
            .ToArray<IQsiAnalysisResult>();
    }

    protected virtual ValueTask<ColumnTarget[]> ResolveColumnTargetsFromDataInsertActionAsync(IAnalyzerContext context, QsiTableStructure table, IQsiDataInsertActionNode action)
    {
        ColumnTarget[] columnTargets;

        switch (action)
        {
            case { Columns.Length: > 0 }:
                columnTargets = ResolveColumnTargetsFromIdentifiers(context, table, action.Columns);
                break;

            case { SetValues.Length: > 0 }:
                columnTargets = ResolveSetColumnTargets(context, table, action.SetValues).ToArray<ColumnTarget>();
                break;

            default:
                columnTargets = ResolveColumnTargetsFromTable(context, table);
                break;
        }

        return ValueTask.FromResult(columnTargets);
    }

    protected virtual ColumnTarget[] ResolveColumnTargetsFromIdentifiers(IAnalyzerContext context, QsiTableStructure table, IEnumerable<QsiQualifiedIdentifier> identifiers)
    {
        return identifiers
            .Select((x, i) =>
            {
                var index = FindColumnIndex(context, table, x);

                if (index == -1)
                    throw new QsiException(QsiError.UnknownColumn, x);

                var targetColumn = table.Columns[index];

                QsiTableColumn[] affectedColumns = GetAffectedColumns(targetColumn)
                    .Take(2)
                    .ToArray();

                // TODO: Add QsiAnalyzerOptions
                if (affectedColumns.Length != 1)
                    throw new QsiException(QsiError.NotUpdatableColumn, x);

                return new ColumnTarget(i, x, targetColumn, affectedColumns[0]);
            })
            .ToArray();
    }

    protected virtual ColumnTarget[] ResolveColumnTargetsFromTable(IAnalyzerContext context, QsiTableStructure table)
    {
        return table.Columns
            .Select((x, i) =>
            {
                QsiTableColumn[] affectedColumns = GetAffectedColumns(x)
                    .Take(2)
                    .ToArray();

                // TODO: Add QsiAnalyzerOptions
                if (affectedColumns.Length != 1)
                    throw new QsiException(QsiError.NotUpdatableColumn, x.Name);

                return new ColumnTarget(i, new QsiQualifiedIdentifier(x.Name), x, affectedColumns[0]);
            })
            .ToArray();
    }

    protected virtual SetColumnTarget[] ResolveSetColumnTargets(IAnalyzerContext context, QsiTableStructure table, IEnumerable<IQsiSetColumnExpressionNode> declaredNodes)
    {
        SetColumnTarget[] pivots = declaredNodes
            .Select((x, i) => ResolveSetColumnTarget(context, table, i, x))
            .ToArray();

        IEnumerable<int> multipleDeclaration = pivots
            .GroupBy(x => x.AffectedColumn)
            .Select(x => x.Count());

        if (multipleDeclaration.Any(x => x > 1))
            throw new QsiException(QsiError.NotSupportedFeature, "Multiple set column");

        return pivots;
    }

    protected virtual SetColumnTarget ResolveSetColumnTarget(
        IAnalyzerContext context,
        QsiTableStructure table,
        int declaredOrder,
        IQsiSetColumnExpressionNode declaredNode)
    {
        var index = FindColumnIndex(context, table, declaredNode.Target);

        if (index == -1)
            throw new QsiException(QsiError.UnknownColumn, declaredNode.Target);

        var targetColumn = table.Columns[index];

        QsiTableColumn[] affectedColumns = GetAffectedColumns(targetColumn)
            .Take(2)
            .ToArray();

        // TODO: Add QsiAnalyzerOptions
        if (affectedColumns.Length != 1)
            throw new QsiException(QsiError.NotUpdatableColumn, declaredNode.Target);

        return new SetColumnTarget(
            declaredOrder,
            declaredNode.Target,
            targetColumn,
            affectedColumns[0],
            declaredNode.Value
        );
    }

    protected virtual QsiTableColumn ResolveNotNullableColumnWithInvalidDefault(IEnumerable<QsiTableColumn> columns, IEnumerable<ColumnTarget> columnTargets)
    {
        HashSet<string> targetNames = columnTargets.Select(ct => ct.DeclaredName.SubIdentifier(0).ToString()).ToHashSet();

        return columns
            .FirstOrDefault(x => !targetNames.Contains(x.Name.Value) && !x.IsNullable && x.Default is null);
    }

    private async ValueTask ProcessQueryValues(TableDataInsertContext context, IQsiTableDirectivesNode directives, IQsiTableNode valueTable)
    {
        var engine = context.Engine;
        var script = engine.TreeDeparser.Deparse(valueTable, context.Script);

        // TODO: bind parameters in directives
        if (directives != null)
            script = $"{engine.TreeDeparser.Deparse(directives, context.Script)}\n{script}";

        var scriptType = engine.ScriptParser.GetSuitableType(script);
        QsiParameter[] parameters = ArrangeBindParameters(context, valueTable);

        var dataTable = (await engine.RepositoryProvider.GetDataTable(
            new QsiScript(script, scriptType),
            parameters,
            context.ExecuteOptions,
            context.CancellationToken
        )).CloneVisibleOnly();

        if (dataTable.Rows.ColumnCount != context.ColumnTargets.Length)
            throw new QsiException(QsiError.DifferentColumnsCount);

        // Old cache hitted
        if (dataTable.Table.Columns.Count != context.ColumnTargets.Length)
            throw new QsiException(QsiError.DifferentColumnsCount);

        // Update source pivot
        foreach (DataManipulationTargetDataPivot[] dataPivots in context.Targets.Select(x => x.DataPivots))
        {
            for (int i = 0; i < dataPivots.Length; i++)
            {
                var dataPivot = dataPivots[i];

                if (dataPivot.DeclaredColumnTarget is not { } declaredColumnTarget)
                    continue;

                var sourceColumn = dataTable.Table.Columns[declaredColumnTarget.DeclaredOrder];

                dataPivots[i] = new DataManipulationTargetDataPivot(
                    dataPivot.DeclaredColumnTarget,
                    dataPivot.DestinationOrder,
                    dataPivot.DestinationColumn,
                    declaredColumnTarget.DeclaredOrder,
                    sourceColumn
                );
            }
        }

        foreach (var row in dataTable.Rows)
        {
            PopulateInsertRow(context, pivot => row.Items[pivot.SourceOrder]);
        }
    }

    private void ProcessValues(TableDataInsertContext context, IQsiRowValueExpressionNode[] rows)
    {
        int columnCount = context.ColumnTargets.Length;

        for (int i = 0; i < rows.Length; i++)
        {
            var row = rows[i];

            if (columnCount != row.ColumnValues.Length)
                throw new QsiException(QsiError.DifferentColumnValueCount, i + 1);

            PopulateInsertRow(context, pivot => ResolveColumnValue(context, row.ColumnValues[pivot.DeclaredColumnTarget.DeclaredOrder]));
        }
    }

    private void ProcessSetValues(TableDataInsertContext context, IQsiSetColumnExpressionNode[] setValues)
    {
        PopulateInsertRow(context, pivot =>
        {
            var declaredColumnTarget = (SetColumnTarget)pivot.DeclaredColumnTarget;
            return ResolveColumnValue(context, declaredColumnTarget.ValueNode);
        });
    }

    // TODO: action.Target, action.Condition
    private void ProcessConflict(TableDataInsertContext context, IQsiDataConflictActionNode action, QsiDataConflictBehavior behavior)
    {
        if (behavior == QsiDataConflictBehavior.None)
        {
            // TODO: throw in duplicate rows
            return;
        }

        if (action.SetValues == null)
            throw new QsiException(QsiError.Syntax);

        SetColumnTarget[] setColumnTargets = ResolveSetColumnTargets(context, context.Table, action.SetValues);

        foreach (var updateTarget in ResolveDataManipulationTargets(context, setColumnTargets))
        {
            var target = context.Targets.FirstOrDefault(t => t.Table == updateTarget.Table);

            if (target == null)
                continue;

            var targetRow = new QsiDataRow(target.DuplicateRows.ColumnCount);

            foreach (var pivot in updateTarget.DataPivots)
            {
                ref var item = ref targetRow.Items[pivot.DestinationOrder];

                item = pivot.SourceColumn is not null
                    ? ResolveColumnValue(context, ((SetColumnTarget)pivot.DeclaredColumnTarget).ValueNode)
                    : QsiDataValue.Unset;
            }

            target.DuplicateRows.Add(targetRow);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PopulateInsertRow(TableDataInsertContext context, DataValueSelector valueSelector)
    {
        foreach (var target in context.Targets)
        {
            var targetRow = new QsiDataRow(target.InsertRows.ColumnCount);

            foreach (var pivot in target.DataPivots)
            {
                ref var item = ref targetRow.Items[pivot.DestinationOrder];

                item = pivot.SourceColumn is not null
                    ? valueSelector(pivot)
                    : ResolveDefaultColumnValue(pivot);

                if (item.Value is null && target.Table.Columns[pivot.DestinationOrder].IsNullable == false)
                    throw new QsiException(QsiError.NotNullConstraints, pivot.DestinationColumn.Name.Value);
            }

            target.InsertRows.Add(targetRow);
        }
    }
    #endregion

    #region Delete
    protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteDataDeleteAction(IAnalyzerContext context, IQsiDataDeleteActionNode action)
    {
        var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
        QsiTableStructure table;
        int tableColumnCount;

        var actionTarget = action.Target;

        var options = context.AnalyzerOptions with
        {
            UseImplicitTableWildcardInSelect = actionTarget is IQsiDerivedTableNode
        };

        using (var tableContext = new TableCompileContext(context, options))
        {
            table = (await tableAnalyzer.BuildTableStructure(tableContext, actionTarget)).CloneVisibleOnly();
            tableColumnCount = table.Columns.Count;

            // TODO: How to compile without IsImplicitTableWildcard options
            HashSet<QsiQualifiedIdentifier> implicitTableWildcardSources = table.Columns
                .Select(x => x.ImplicitTableWildcardTarget)
                .Where(x => x is not null)
                .ToHashSet(QualifiedIdentifierComparer);

            if (implicitTableWildcardSources.Count > 0)
            {
                var derived = (IQsiDerivedTableNode)actionTarget;
                IQsiColumnNode[] columnNodes = derived.Columns.Columns.ToArray();

                for (int i = 0; i < columnNodes.Length; i++)
                {
                    if (columnNodes[i] is IQsiColumnReferenceNode referenceNode &&
                        implicitTableWildcardSources.Contains(referenceNode.Name))
                    {
                        columnNodes[i] = new QsiAllColumnNode { Path = referenceNode.Name };
                    }
                }

                actionTarget = new ImmutableDerivedTableNode(
                    derived.Parent,
                    derived.Directives,
                    new ImmutableColumnsDeclarationNode(
                        derived.Columns.Parent,
                        columnNodes,
                        null
                    ),
                    derived.Source,
                    null,
                    derived.Where,
                    derived.Grouping,
                    derived.Order,
                    derived.Limit,
                    null
                );
            }
        }

        var commonTableNode = ReassembleCommonTableNode(actionTarget);
        var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

        if (ListUtility.IsNullOrEmpty(action.Columns) && dataTable.Table.Columns.Count != tableColumnCount)
            throw new QsiException(QsiError.Internal, "Query results do not match target table structure");

        QsiQualifiedIdentifier[] columnNames = action.Columns;

        if (columnNames == null)
        {
            columnNames = new QsiQualifiedIdentifier[tableColumnCount];

            for (int i = 0; i < tableColumnCount; i++)
            {
                columnNames[i] = new QsiQualifiedIdentifier(dataTable.Table.Columns[i].Name);
                table.Columns[i].Name = columnNames[i][^1];
            }
        }

        ColumnTarget[] columnTargets = ResolveColumnTargetsFromIdentifiers(context, table, columnNames);

        return ResolveDataManipulationTargets(context, columnTargets)
            .Select(target =>
            {
                foreach (var row in dataTable.Rows)
                {
                    var targetRow = new QsiDataRow(target.DeleteRows.ColumnCount);

                    foreach (var pivot in target.DataPivots)
                    {
                        ref var item = ref targetRow.Items[pivot.DestinationOrder];

                        item = pivot.SourceColumn is not null
                            ? row.Items[pivot.SourceOrder]
                            : QsiDataValue.Unset;
                    }

                    target.DeleteRows.Add(targetRow);
                }

                return new QsiDataManipulationResult
                {
                    Table = target.Table,
                    AffectedColumns = GetAffectedColumns(target),
                    DeleteRows = target.DeleteRows.ToNullIfEmpty()
                };
            })
            .ToArray<IQsiAnalysisResult>();
    }
    #endregion

    #region Update
    protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteDataUpdateAction(IAnalyzerContext context, IQsiDataUpdateActionNode action)
    {
        // table structure
        var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
        using var tableContext = new TableCompileContext(context);

        var sourceTable = (await tableAnalyzer.BuildTableStructure(tableContext, action.Target)).CloneVisibleOnly();

        // update data (rows)
        var commonTableNode = ReassembleCommonTableNode(action.Target);
        var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

        if (dataTable.Table.Columns.Count != sourceTable.Columns.Count)
            throw new QsiException(QsiError.DifferentColumnsCount);

        var values = new QsiDataValue[sourceTable.Columns.Count];
        var affectedColumnMap = new bool[sourceTable.Columns.Count];
        ColumnTarget[] columnTargets;

        if (!ListUtility.IsNullOrEmpty(action.SetValues))
        {
            SetColumnTarget[] setColumnTargets = ResolveSetColumnTargets(context, sourceTable, action.SetValues);
            columnTargets = setColumnTargets.ToArray<ColumnTarget>();

            foreach (var columnTarget in setColumnTargets)
            {
                var targetIndex = columnTarget.SourceColumn.Parent.Columns.IndexOf(columnTarget.SourceColumn);
                values[targetIndex] = ResolveColumnValue(context, columnTarget.ValueNode);
                affectedColumnMap[targetIndex] = true;
            }
        }
        else if (action.Value != null)
        {
            columnTargets = ResolveColumnTargetsFromTable(context, sourceTable);

            if (action.Value.ColumnValues.Length != values.Length)
                throw new QsiException(QsiError.DifferentColumnValueCount, 0);

            for (int i = 0; i < values.Length; i++)
                values[i] = ResolveColumnValue(context, action.Value.ColumnValues[i]);

            affectedColumnMap.AsSpan().Fill(true);
        }
        else
        {
            throw new QsiException(QsiError.Syntax);
        }

        return ResolveDataManipulationTargets(context, columnTargets)
            .Select(target =>
            {
                foreach (var row in dataTable.Rows)
                {
                    var oldRow = new QsiDataRow(target.UpdateBeforeRows.ColumnCount);
                    var newRow = new QsiDataRow(target.UpdateAfterRows.ColumnCount);

                    foreach (var pivot in target.DataPivots)
                    {
                        ref var oldItem = ref oldRow.Items[pivot.DestinationOrder];
                        ref var newItem = ref newRow.Items[pivot.DestinationOrder];

                        if (pivot.SourceColumn != null)
                        {
                            var value = row.Items[pivot.SourceOrder];

                            oldItem = value;
                            newItem = values[pivot.SourceOrder] ?? value;
                        }
                        else
                        {
                            oldItem = QsiDataValue.Unknown;
                            newItem = QsiDataValue.Unset;
                        }
                    }

                    target.UpdateBeforeRows.Add(oldRow);
                    target.UpdateAfterRows.Add(newRow);
                }

                QsiTableColumn[] affectedColumns = target.DataPivots
                    .Where(p => p.SourceColumn != null && affectedColumnMap[p.SourceOrder])
                    .Select(p => p.SourceColumn)
                    .ToArray();

                return new QsiDataManipulationResult
                {
                    Table = target.Table,
                    AffectedColumns = affectedColumns,
                    UpdateBeforeRows = target.UpdateBeforeRows.ToNullIfEmpty(),
                    UpdateAfterRows = target.UpdateAfterRows.ToNullIfEmpty()
                };
            })
            .ToArray<IQsiAnalysisResult>();
    }

    protected int FindColumnIndex(IAnalyzerContext context, QsiTableStructure table, QsiQualifiedIdentifier identifier)
    {
        var alias = new QsiQualifiedIdentifier(identifier[..^1]);
        var name = identifier[^1];

        var queue = new Queue<QsiTableColumn>();
        var recursiveTracker = new HashSet<QsiTableColumn>();

        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (!IdentifierComparer.Equals(table.Columns[i].Name, name))
                continue;

            if (identifier.Level == 1)
                return i;

            queue.Clear();
            recursiveTracker.Clear();

            queue.Enqueue(table.Columns[i]);

            while (queue.TryDequeue(out var column))
            {
                recursiveTracker.Add(column);

                if (Match(context, column.Parent, alias))
                    return i;

                foreach (var refColumn in column.References.Where(refColumn => !recursiveTracker.Contains(refColumn)))
                {
                    queue.Enqueue(refColumn);
                }
            }
        }

        return -1;
    }
    #endregion

    #region SearchPath
    protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteSearchPathAction(IAnalyzerContext context, IQsiChangeSearchPathActionNode action)
    {
        var fakeRefIdentifier = new QsiIdentifier(string.Empty, false);

        QsiQualifiedIdentifier[] identifiers = action.Identifiers
            .Select(identifier =>
            {
                var qualifiedIdentifier = new QsiQualifiedIdentifier(identifier.Append(fakeRefIdentifier));
                qualifiedIdentifier = context.Engine.RepositoryProvider.ResolveQualifiedIdentifier(qualifiedIdentifier, context.ExecuteOptions);
                return new QsiQualifiedIdentifier(qualifiedIdentifier[..^1]);
            })
            .ToArray();

        return new QsiChangeSearchPathAction(identifiers).ToSingleArray().AsValueTask();
    }
    #endregion

    #region CreateUser
    protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteCreateUserAction(IAnalyzerContext context, IQsiCreateUserActionNode node)
    {
        var result = new QsiUserActionResult();

        result.UserInfos = node.Users
            .Select(user => ResolveUser(context, user, result.SensitiveDataCollection))
            .ToArray();

        return result
            .ToSingleArray()
            .AsValueTask();
    }
    #endregion

    #region AlterUser
    protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteAlterUserAction(IAnalyzerContext context, IQsiAlterUserActionNode node)
    {
        var result = new QsiUserActionResult();

        result.UserInfos = node.Users
            .Select(user => ResolveUser(context, user, result.SensitiveDataCollection))
            .ToArray();

        return result
            .ToSingleArray()
            .AsValueTask();
    }
    #endregion

    #region GrantUser
    protected virtual ValueTask<IQsiAnalysisResult[]> ExecuteGrantUserAction(IAnalyzerContext context, IQsiGrantUserActionNode node)
    {
        var result = new QsiGrantUserActionResult
        {
            Roles = node.Roles?.ToArray() ?? Array.Empty<string>()
        };

        result.TargetUsers = node.Users
            .Select(user => ResolveUser(context, user, result.SensitiveDataCollection))
            .ToArray();

        return result
            .ToSingleArray()
            .AsValueTask();
    }
    #endregion

    #region VariableSet
    protected virtual async ValueTask<IQsiAnalysisResult[]> ExecuteVariableSetAction(IAnalyzerContext context, IQsiVariableSetActionNode node)
    {
        var results = new List<IQsiAnalysisResult>();

        results.AddRange(node.SetItems.Select(setItem => ResolveVariableSet(context, setItem)));

        if (node.Target is not null)
        {
            results.AddRange(
                await context.Engine.Execute(
                    context.Script,
                    context.Parameters.Values.ToArray(),
                    node.Target,
                    context.AnalyzerOptions,
                    context.ExecuteOptions,
                    context.CancellationToken
                )
            );
        }

        return results.ToArray();
    }

    protected virtual QsiVariableSetActionResult ResolveVariableSet(IAnalyzerContext context, IQsiVariableSetItemNode node)
    {
        // node.Expression ignored
        return new QsiVariableSetActionResult
        {
            Name = node.Name
        };
    }
    #endregion

    protected virtual QsiUserInfo ResolveUser(IAnalyzerContext context, IQsiUserNode node, QsiSensitiveDataCollection dataCollection)
    {
        return new QsiUserInfo
        {
            Username = node.Username
        };
    }

    protected virtual QsiSensitiveData CreateSensitiveData(QsiSensitiveDataType dataType, IQsiTreeNode node)
    {
        throw new NotSupportedException();
    }
}
