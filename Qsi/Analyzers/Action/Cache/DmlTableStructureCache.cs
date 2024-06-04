using System.Collections.Generic;
using Qsi.Analyzers.Action.Models;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Cache;

public class DmlTableStructureCache : ITableStructureCache<DataManipulationTarget>
{
    private readonly Dictionary<DataManipulationTarget, List<QsiTableStructure>> _cache = new();

    public void Add(DataManipulationTarget key, QsiTableStructure table)
    {
        if (_cache.TryGetValue(key, out List<QsiTableStructure> value))
        {
            value.Add(table);
            return;
        }

        _cache[key] = new List<QsiTableStructure> { table };
    }

    public void AddRange(DataManipulationTarget key, IEnumerable<QsiTableStructure> tables)
    {
        if (_cache.TryGetValue(key, out List<QsiTableStructure> value))
        {
            value.AddRange(tables);
            return;
        }

        _cache[key] = new List<QsiTableStructure>(tables);
    }

    public ICollection<QsiTableStructure> Get(DataManipulationTarget key)
    {
        return _cache[key];
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
