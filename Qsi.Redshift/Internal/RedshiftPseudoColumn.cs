using System;
using Qsi.Data;

namespace Qsi.Redshift.Internal;

internal static class RedshiftPseudoColumn
{
    private static QsiTableStructure _pseudoTable;

    private static readonly string[] _names =
    {
        // built-in function
        "LOCALTIME",
        "LOCALTIMESTAMP",
        "SYSDATE",
        "CURRENT_DATE",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "USER",
        "CURRENT_USER_ID",
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
