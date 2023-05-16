using System;
using System.Collections;
using System.Collections.Generic;

namespace Qsi.Extensions;

public static class IEnumerableExtensions
{
    public static IBufferedEnumerable<T> Buffer<T>(this IEnumerable<T> enumerable)
    {
        switch (enumerable)
        {
            case IBufferedEnumerable<T> bufferedEnumerable:
                return bufferedEnumerable;

            case IList<T>:
            case IReadOnlyList<T>:
            case ICollection<T>:
            case IReadOnlyCollection<T>:
                return new SizedEnumerator<T>(enumerable);
        }

        return new BufferedEnumerable<T>(enumerable);
    }
}

public interface IBufferedEnumerable<out T> : IEnumerable<T>, IDisposable
{
}

internal sealed class BufferedEnumerable<T> : IBufferedEnumerable<T>
{
    private readonly IEnumerator<T> _enumerator;
    private readonly List<T> _buffer;

    public BufferedEnumerable(IEnumerable<T> enumerable)
    {
        _enumerator = enumerable.GetEnumerator();
        _buffer = new List<T>();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new BufferedEnumerator<T>(_enumerator, _buffer);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        _enumerator.Dispose();
        _buffer.Clear();
    }
}

internal sealed class BufferedEnumerator<T> : IEnumerator<T>
{
    public T Current => _buffer[_index];

    object IEnumerator.Current => Current;

    private readonly IEnumerator<T> _enumerator;
    private readonly List<T> _buffer;
    private int _index = -1;

    public BufferedEnumerator(IEnumerator<T> enumerator, List<T> buffer)
    {
        _enumerator = enumerator;
        _buffer = buffer;
    }

    public bool MoveNext()
    {
        lock (_enumerator)
        {
            if (_index + 1 < _buffer.Count)
            {
                _index++;
                return true;
            }

            if (!_enumerator.MoveNext())
                return false;

            _index++;
            _buffer.Add(_enumerator.Current);
            return true;
        }
    }

    public void Reset()
    {
        _index = -1;
    }

    public void Dispose()
    {
    }
}

internal readonly struct SizedEnumerator<T> : IBufferedEnumerable<T>
{
    private readonly IEnumerable<T> _enumerable;

    public SizedEnumerator(IEnumerable<T> enumerable)
    {
        _enumerable = enumerable;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
    }
}
