using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tests.Models;

public sealed class QsiTableStructureView : ReferenceView
{
    public QsiTableType Type { get; }

    public string Identifier { get; }

    public bool IsSystem { get; }

    public string[] References { get; }

    public QsiTableColumnView[] Columns { get; }

    public QsiTableStructureView(string refId, QsiTableType type, string identifier, bool isSystem, string[] references, QsiTableColumnView[] columns) : base(refId)
    {
        Type = type;
        Identifier = identifier;
        IsSystem = isSystem;
        References = references;
        Columns = columns;
    }

    public static QsiTableStructureView[] From(QsiTableStructure table)
    {
        var tableRefs = new ReferenceViewManager<QsiTableStructureView>("tbl_", null);
        var columnRefs = new ReferenceViewManager<QsiTableColumnView>("col_", null);

        foreach (var c in table.Columns.SelectMany(QsiUtility.FlattenColumns))
            Collect(c, tableRefs, columnRefs);

        foreach (var t in QsiUtility.FlattenTables(table))
            Collect(t, tableRefs, columnRefs);

        columnRefs.Freeze();
        tableRefs.Freeze();

        return tableRefs.Views.ToArray();
    }

    private static DeferredReferenceView<QsiTableStructureView> Collect(
        QsiTableStructure table,
        ReferenceViewManager<QsiTableStructureView> tableRefs,
        ReferenceViewManager<QsiTableColumnView> columnRefs)
    {
        if (table is null)
            return null;

        return tableRefs.GetOrCreateView(table, refId =>
        {
            return new QsiTableStructureView(
                refId,
                table.Type,
                table.Identifier?.ToString(),
                table.IsSystem,
                table.References
                    .Select(x => Collect(x, tableRefs, columnRefs)?.RefId)
                    .ToArray(),
                table.Columns
                    .Select(x => Collect(x, tableRefs, columnRefs).Value)
                    .ToArray()
            );
        });
    }

    private static DeferredReferenceView<QsiTableColumnView> Collect(
        QsiTableColumn column,
        ReferenceViewManager<QsiTableStructureView> tableRefs,
        ReferenceViewManager<QsiTableColumnView> columnRefs)
    {
        return columnRefs.GetOrCreateView(column, refId =>
        {
            return new QsiTableColumnView(
                refId,
                Collect(column.Parent, tableRefs, columnRefs)?.RefId,
                column.Name?.ToString(),
                column.References
                    .Select(x => Collect(x, tableRefs, columnRefs)?.RefId)
                    .ToArray(),
                column.ObjectReferences.ToArray(),
                column.IsVisible,
                column.IsBinding,
                column.IsDynamic,
                column.Default,
                column.IsExpression
            );
        });
    }
}
