using System;
using Qsi.Data;

namespace Qsi.Oracle.Internal;

internal static class OraclePseudoColumn
{
    private static QsiTableStructure _pseudoTable;

    private static readonly string[] _names =
    {
        "ROWNUM",
        "ROWID",
        "ORA_ROWSCN",
        "COLUMN_VALUE",

        // Hierarchical Query
        "LEVEL",
        "CONNECT_BY_ISLEAF",
        "CONNECT_BY_ISCYCLE",

        // Version Query
        "VERSIONS_STARTTIME",
        "VERSIONS_STARTSCN",
        "VERSIONS_ENDTIME",
        "VERSIONS_ENDSCN",
        "VERSIONS_XID",
        "VERSIONS_OPERATION"
    };

    public static bool TryGetColumn(string name, out QsiTableColumn tableColumn)
    {
        _pseudoTable ??= CreatePseudoTable();
        int index = Array.FindIndex(_names, t => t.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        if (index != -1)
        {
            tableColumn = _pseudoTable.Columns[index];
            return true;
        }

        tableColumn = null;
        return false;
    }

    private static QsiTableStructure CreatePseudoTable()
    {
        var table = new QsiTableStructure
        {
            Identifier = new QsiQualifiedIdentifier(new QsiIdentifier("PseudoColumns", false))
        };

        foreach (var name in _names)
        {
            var c = table.NewColumn();
            c.Name = new QsiIdentifier(name, false);
        }

        return table;
    }
}