using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qsi.Analyzers;
using Qsi.Analyzers.Action;
using Qsi.Analyzers.Context;
using Qsi.Analyzers.Table;
using Qsi.Analyzers.Table.Context;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Oracle.Collections;
using Qsi.Oracle.Tree;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Analyzers
{
    public sealed class OracleActionAnalyzer : QsiActionAnalyzer
    {
        public OracleActionAnalyzer(QsiEngine engine) : base(engine)
        {
        }

        protected override async ValueTask<IQsiAnalysisResult[]> ExecuteDataUpdateAction(IAnalyzerContext context, IQsiDataUpdateActionNode action)
        {
            if (action is not OracleDataUpdateActionNode oracleAction)
                return await base.ExecuteDataUpdateAction(context, action);

            // table structure
            var tableAnalyzer = context.Engine.GetAnalyzer<QsiTableAnalyzer>();
            using var tableContext = new TableCompileContext(context);

            var sourceTable = await tableAnalyzer.BuildTableStructure(tableContext, action.Target);

            // update data (rows)
            var commonTableNode = ReassembleCommonTableNode(action.Target);
            var dataTable = await GetDataTableByCommonTableNode(context, commonTableNode);

            if (dataTable.Table.Columns.Count != sourceTable.Columns.Count)
                throw new QsiException(QsiError.DifferentColumnsCount);

            if (dataTable.Rows.Count == 0)
                return Array.Empty<IQsiAnalysisResult>();

            // values
            var values = new QsiDataValue[sourceTable.Columns.Count];
            var affectedColumnMap = new bool[sourceTable.Columns.Count];
            var affectedColumnSet = new HashSet<QsiQualifiedIdentifier>(QualifiedIdentifierComparer);

            if (oracleAction.SetValueExpressions.Count == 0)
                throw new QsiException(QsiError.Syntax);

            foreach (var setValueExpression in oracleAction.SetValueExpressions)
            {
                if (!setValueExpression.SetValue.IsEmpty)
                {
                    var setValue = setValueExpression.SetValue.Value;

                    if (!affectedColumnSet.Add(setValue.Target))
                        throw new QsiException(QsiError.DuplicateColumnName, setValue.Target);

                    int sourceIndex = FindColumnIndex(context, sourceTable, setValue.Target);

                    if (sourceIndex == -1)
                        throw new QsiException(QsiError.UnknownColumn, setValue.Target);

                    values[sourceIndex] = ResolveColumnValue(context, setValue.Value.Value);
                    affectedColumnMap[sourceIndex] = true;
                }

                if (!setValueExpression.SetValueFromTable.IsEmpty)
                {
                    var setValueFromTable = setValueExpression.SetValueFromTable.Value;

                    foreach (var target in setValueFromTable.Targets)
                    {
                        if (!affectedColumnSet.Add(target))
                            throw new QsiException(QsiError.DuplicateColumnName, target);
                    }

                    var valueTable = await GetDataTableByCommonTableNode(context, setValueFromTable.Value.Value);

                    if (valueTable.Rows.Count != 1)
                        throw new QsiException(QsiError.Internal, "Single-row subquery returns more than one row.");

                    for (int targetIndex = 0; targetIndex < setValueFromTable.Targets.Length; targetIndex++)
                    {
                        var target = setValueFromTable.Targets[targetIndex];
                        int sourceIndex = FindColumnIndex(context, sourceTable, target);

                        if (sourceIndex == -1)
                            throw new QsiException(QsiError.UnknownColumn, target);

                        values[sourceIndex] = valueTable.Rows[0].Items[targetIndex];
                        affectedColumnMap[sourceIndex] = true;
                    }
                }
            }

            return ResolveDataManipulationTargets(sourceTable)
                .Select(target =>
                {
                    foreach (var row in dataTable.Rows)
                    {
                        var oldRow = target.UpdateBeforeRows.NewRow();
                        var newRow = target.UpdateAfterRows.NewRow();

                        foreach (var pivot in target.ColumnPivots)
                        {
                            if (pivot.DeclaredColumn != null)
                            {
                                var value = row.Items[pivot.DeclaredOrder];

                                oldRow.Items[pivot.TargetOrder] = value;
                                newRow.Items[pivot.TargetOrder] = values[pivot.DeclaredOrder] ?? value;
                            }
                            else
                            {
                                oldRow.Items[pivot.TargetOrder] = QsiDataValue.Unknown;
                                newRow.Items[pivot.TargetOrder] = QsiDataValue.Unknown;
                            }
                        }
                    }

                    QsiTableColumn[] affectedColumns = target.ColumnPivots
                        .Where(p => p.DeclaredColumn != null && affectedColumnMap[p.DeclaredOrder])
                        .Select(p => p.DeclaredColumn)
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
        
        
    }
}
