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
        var collector = new ReferenceViewCollector();

        foreach (var c in table.Columns.SelectMany(QsiUtility.FlattenColumns))
            Collect(c, collector);

        foreach (var t in QsiUtility.FlattenTables(table))
            Collect(t, collector);

        return collector
            .Collect<QsiTableStructureView>()
            .ToArray();
    }

    private static DeferredReferenceView Collect(QsiTableStructure table, ReferenceViewCollector collector)
    {
        if (table is null)
            return null;

        return collector.GetOrCreateView(
            table,
            seq => $"$tbl_{seq}",
            refId =>
            {
                return new QsiTableStructureView(
                    refId,
                    table.Type,
                    table.Identifier?.ToString(),
                    table.IsSystem,
                    table.References
                        .Select(x => Collect(x, collector)?.RefId)
                        .ToArray(),
                    table.Columns
                        .Select(x => (QsiTableColumnView)Collect(x, collector).Value)
                        .ToArray()
                );
            });
    }

    private static DeferredReferenceView Collect(QsiTableColumn column, ReferenceViewCollector collector)
    {
        return collector.GetOrCreateView(
            column,
            seq => $"$col_{seq}",
            refId =>
            {
                return new QsiTableColumnView(
                    refId,
                    Collect(column.Parent, collector)?.RefId,
                    column.Name?.ToString(),
                    column.References
                        .Select(x => Collect(x, collector)?.RefId)
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
