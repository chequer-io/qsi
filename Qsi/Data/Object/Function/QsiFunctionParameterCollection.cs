using System;
using System.Collections;
using System.Collections.Generic;

namespace Qsi.Data.Object.Function;

public class QsiFunctionParameterCollection : IList<QsiFunctionParameter>
{
    public int Count => _list.Count;

    public QsiFunctionParameter this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    bool ICollection<QsiFunctionParameter>.IsReadOnly => false;

    private readonly List<QsiFunctionParameter> _list;
    private readonly QsiFunctionObject _parent;

    public QsiFunctionParameterCollection(QsiFunctionObject parent)
    {
        _list = new List<QsiFunctionParameter>();
        _parent = parent;
    }

    public void Add(QsiFunctionParameter item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (item.Parent == _parent)
            return;

        if (item.Parent != null)
            throw new ArgumentException(nameof(item));

        item.Parent = _parent;
        _list.Add(item);
    }

    public void Insert(int index, QsiFunctionParameter item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (item.Parent == _parent)
            return;

        if (item.Parent != null)
            throw new ArgumentException(nameof(item));

        item.Parent = _parent;
        _list.Insert(index, item);
    }

    public void Clear()
    {
        foreach (var column in _list)
            column.Parent = null;

        _list.Clear();
    }

    public bool Contains(QsiFunctionParameter item)
    {
        if (item == null || item.Parent != _parent)
            return false;

        return _list.Contains(item);
    }

    public int IndexOf(QsiFunctionParameter item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (item.Parent != _parent)
            return -1;

        return _list.IndexOf(item);
    }

    public bool Remove(QsiFunctionParameter item)
    {
        if (item == null)
            return false;

        if (item.Parent != _parent)
            throw new ArgumentException(nameof(item));

        if (!_list.Remove(item))
            return false;

        item.Parent = null;

        return true;
    }

    public void RemoveAt(int index)
    {
        var column = _list[index];

        _list.RemoveAt(index);
        column.Parent = null;
    }

    void ICollection<QsiFunctionParameter>.CopyTo(QsiFunctionParameter[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<QsiFunctionParameter> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}