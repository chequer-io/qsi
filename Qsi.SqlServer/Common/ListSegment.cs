using System;
using System.Collections.Generic;
using System.Linq;

public readonly ref struct ListSegment<T>
{
    public T this[int index]
    {
        get
        {
            if (index >= Count)
                throw new IndexOutOfRangeException();

            return _list[Offset + index];
        }
    }

    public int Offset { get; }

    public int Count { get; }

    private readonly IList<T> _list;

    public ListSegment(IList<T> list, int offset, int count)
    {
        _list = list;
        Offset = offset;
        Count = count;
    }

    public bool All(Predicate<T> predicate)
    {
        var end = Offset + Count;

        for (var i = Offset; i < end; i++)
        {
            if (!predicate(_list[i]))
                return false;
        }

        return true;
    }

    public IEnumerable<T> AsEnumerable()
    {
        return _list.Take(Offset..(Offset + Count));
    }
}
