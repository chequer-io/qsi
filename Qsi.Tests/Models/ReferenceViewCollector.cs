using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Qsi.Tests.Models;

public sealed class ReferenceViewCollector
{
    private readonly Dictionary<object, DeferredReferenceView> _table = new();

    private int _id;

    public DeferredReferenceView GetOrCreateView(object value, Func<int, string> idFactory, Func<string, ReferenceView> factory)
    {
        if (!_table.TryGetValue(value, out var view))
        {
            var refId = idFactory(Interlocked.Increment(ref _id));
            view = new DeferredReferenceView(refId, factory);
            _table[value] = view;
        }

        return view;
    }

    public IEnumerable<TView> Collect<TView>() where TView : ReferenceView
    {
        while (_id > 0)
        {
            var version = _id;
            using var valueEnumerator = _table.Values.GetEnumerator();

            while (version == _id && valueEnumerator.MoveNext())
            {
                var view = valueEnumerator.Current!;

                if (!view.IsValueCreated)
                    view.Create();
            }

            if (version == _id)
                break;
        }

        return _table.Values
            .Select(x => x.Value)
            .OrderBy(x => x.RefId)
            .OfType<TView>();
    }
}

public sealed class DeferredReferenceView
{
    public string RefId { get; }

    public ReferenceView Value => _lazyValue.Value;

    public bool IsValueCreated => _lazyValue.IsValueCreated;

    private readonly Lazy<ReferenceView> _lazyValue;

    public DeferredReferenceView(string refId, Func<string, ReferenceView> factory)
    {
        RefId = refId;
        _lazyValue = new Lazy<ReferenceView>(() => factory(refId));
    }

    public void Create()
    {
        var _ = _lazyValue.Value;
    }
}
