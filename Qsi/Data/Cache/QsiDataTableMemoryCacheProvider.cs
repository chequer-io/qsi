using System.Collections.Generic;

namespace Qsi.Data.Cache;

public class QsiDataTableMemoryCacheProvider : IQsiDataTableCacheProvider
{
    public int Count => _rows.Count;

    private List<QsiDataRow> _rows;

    public QsiDataTableMemoryCacheProvider()
    {
        _rows = new List<QsiDataRow>();
    }

    public QsiDataRow Get(int row)
    {
        return _rows[row];
    }

    public IEnumerable<QsiDataRow> Get(int row, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return _rows[row + i];
        }
    }

    public void Add(QsiDataRow row)
    {
        _rows.Add(row);
    }

    public void Flush()
    {
    }

    public void Clear()
    {
        _rows.Clear();
    }
        
    public void Dispose()
    {
        _rows.Clear();
        _rows = null;
    }
}