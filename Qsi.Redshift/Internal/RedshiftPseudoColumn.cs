using System;
using Qsi.Data;

namespace Qsi.Redshift.Internal;

internal static class RedshiftPseudoColumn
{
    private static QsiTableStructure _anonymousTable;

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
        _anonymousTable ??= CreateAnonymousTable();
        int index = Array.FindIndex(_names, t => t.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        if (index != -1)
        {
            tableColumn = _anonymousTable.Columns[0];
            return true;
        }

        tableColumn = null;
        return false;
    }

    private static QsiTableStructure CreateAnonymousTable()
    {
        var table = new QsiTableStructure
        {
            Identifier = new QsiQualifiedIdentifier(new QsiIdentifier("AnonymousColumn", false))
        };

        var c = table.NewColumn();
        c.Name = null;

        return table;
    }
}
