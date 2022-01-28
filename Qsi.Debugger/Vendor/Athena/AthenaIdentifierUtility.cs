using System;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.Athena;

public static class AthenaIdentifierUtility
{
    public static bool Compare(this QsiIdentifier identifier, string name)
    {
        var value = IdentifierUtility.Unescape(identifier.Value);

        var flag = identifier.IsEscaped
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase; 
        
        return value.Equals(name, flag);
    }

    public static bool Compare(this QsiQualifiedIdentifier identifier, params string[] names)
    {
        if (identifier.Level != names.Length) return false;

        for (int i = 0; i < identifier.Level; i++)
        {
            if (identifier[i].Compare(names[i])) continue;

            return false;
        }

        return true;
    }
}
