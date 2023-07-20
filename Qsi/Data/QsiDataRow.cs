using System;

namespace Qsi.Data;

public sealed class QsiDataRow
{
    public int Length { get; }

    public QsiDataValue[] Items
    {
        get => _items;
        set
        {
            if (value != null && value.Length != Length)
                throw new ArgumentException(nameof(value));

            _items = value;
        }
    }

    private QsiDataValue[] _items;

    public QsiDataRow(int length)
    {
        Length = length;
        _items = new QsiDataValue[length];
    }

    public QsiDataRow(QsiDataValue[] items)
    {
        Length = items.Length;
        _items = items;
    }
}