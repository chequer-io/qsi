using System;
using System.Collections;
using System.Collections.Generic;

namespace Qsi.Data;

public class QsiSensitiveDataCollection : ICollection<QsiSensitiveData>
{
    public static QsiSensitiveDataCollection Empty { get; } = new(true);

    private QsiSensitiveDataCollection(bool isReadOnly = false)
    {
        IsReadOnly = isReadOnly;
    }

    public QsiSensitiveDataCollection() : this(false)
    {
    }

    public QsiSensitiveData this[int index] => _sensitiveDataList[index];

    public int Count => _sensitiveDataList.Count;

    public bool IsReadOnly { get; }

    private readonly List<QsiSensitiveData> _sensitiveDataList = new();

    public void Add(QsiSensitiveData item)
    {
        ThrowIfReadOnly();
        _sensitiveDataList.Add(item);
    }

    public void AddRange(IEnumerable<QsiSensitiveData> item)
    {
        ThrowIfReadOnly();
        _sensitiveDataList.AddRange(item);
    }

    public bool Remove(QsiSensitiveData item)
    {
        ThrowIfReadOnly();
        return _sensitiveDataList.Remove(item);
    }

    public void Clear()
    {
        ThrowIfReadOnly();
        _sensitiveDataList.Clear();
    }

    public bool Contains(QsiSensitiveData item)
    {
        return _sensitiveDataList.Contains(item);
    }

    public void CopyTo(QsiSensitiveData[] array, int arrayIndex)
    {
        _sensitiveDataList.CopyTo(array, arrayIndex);
    }

    public IEnumerator<QsiSensitiveData> GetEnumerator()
    {
        return _sensitiveDataList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void ThrowIfReadOnly()
    {
        if (IsReadOnly)
            throw new NotSupportedException();
    }
}
