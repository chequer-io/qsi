using System;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Analyzers.Action
{
    public partial class QsiActionAnalyzer
    {
        protected sealed class DataInsertContext
        {
            public ExecutionContext ExecutionContext { get; }

            public IQsiDataInsertActionNode Action { get; }

            public QsiTableStructure Table { get; }

            public List<DataInsertTargetContext> Targets { get; }

            public QsiIdentifier[] ColumnsNames { get; set; }

            public int[] ColumnsIndices { get; set; }

            // Used in SetValues
            public int[] AffectedIndices { get; set; }

            public DataInsertContext(ExecutionContext context, IQsiDataInsertActionNode action, QsiTableStructure table)
            {
                ExecutionContext = context;
                Action = action;
                Table = table;
                Targets = new List<DataInsertTargetContext>();
            }
        }

        protected sealed class DataInsertColumnPivot
        {
            public QsiTableColumn TargetColumn { get; }

            public QsiTableColumn DeclaredColumn { get; }

            public int TargetOrder { get; }

            public int DeclaredOrder { get; }

            public DataInsertColumnPivot(int targetOrder, QsiTableColumn target, int declaredOrder, QsiTableColumn declared)
            {
                TargetOrder = targetOrder;
                TargetColumn = target;
                DeclaredOrder = declaredOrder;
                DeclaredColumn = declared;
            }
        }

        protected sealed class DataInsertTargetContext
        {
            public QsiTableStructure Table { get; }

            public DataInsertColumnPivot[] ColumnPivots { get; }

            public QsiDataRowCollection Rows { get; }

            public DataInsertTargetContext(QsiTableStructure table, DataInsertColumnPivot[] pivots)
            {
                if (table.Type != QsiTableType.Table)
                    throw new ArgumentException(nameof(table));

                if (pivots.Length != table.Columns.Count)
                    throw new ArgumentException(nameof(pivots));

                Table = table;
                ColumnPivots = pivots;
                Rows = new QsiDataRowCollection(table.Columns.Count);
            }
        }
    }
}
