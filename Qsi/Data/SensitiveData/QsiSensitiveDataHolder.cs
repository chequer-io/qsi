using System.Collections;
using System.Collections.Generic;

namespace Qsi.Data;

public class QsiSensitiveDataHolder : IEnumerable<QsiSensitiveData>
{
    public QsiSensitiveData this[int index] => _sensitiveDataList[index];

    public int Count => _sensitiveDataList.Count;

    private readonly List<QsiSensitiveData> _sensitiveDataList = new();

    public IEnumerator<QsiSensitiveData> GetEnumerator()
    {
        return _sensitiveDataList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(QsiSensitiveData item)
    {
        _sensitiveDataList.Add(item);
    }

    public bool Contains(QsiSensitiveData item)
    {
        return _sensitiveDataList.Contains(item);
    }
}
