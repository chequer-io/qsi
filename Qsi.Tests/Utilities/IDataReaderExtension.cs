using System.Collections.Generic;
using System.Data;

namespace Qsi.Tests.Utilities;

public static class IDataReaderExtension
{
    public static IReadOnlyDictionary<string, object>[] ToDictionary(this IDataReader reader)
    {
        var list = new List<IReadOnlyDictionary<string, object>>();

        while (reader.Read())
        {
            var row = new Dictionary<string, object>();

            for (int i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = reader.GetValue(i);

            list.Add(row);
        }

        return list.ToArray();
    }
}
