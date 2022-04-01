using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Qsi.Tests.Models;

public sealed class ReferenceViewManager<TView> where TView : ReferenceView
{
    public IEnumerable<TView> Views => _table.Values.Select(x => x.Value).OrderBy(x => x.RefId);

    private readonly Dictionary<object, DeferredReferenceView<TView>> _table = new();

    private int _id;
    private readonly string _prefix;
    private readonly string _suffix;

    public ReferenceViewManager(string prefix, string suffix)
    {
        _prefix = prefix;
        _suffix = suffix;
    }

    public DeferredReferenceView<TView> GetOrCreateView(object value, Func<string, TView> factory)
    {
        if (!_table.TryGetValue(value, out DeferredReferenceView<TView> view))
        {
            var id = $"{_prefix}{Interlocked.Increment(ref _id)}{_suffix}";
            view = new DeferredReferenceView<TView>(id, factory);
            _table[value] = view;
        }

        return view;
    }

    public void Freeze()
    {
        while (_id > 0)
        {
            var version = _id;
            using var valueEnumerator = _table.Values.GetEnumerator();

            while (version == _id && valueEnumerator.MoveNext())
            {
                if (!valueEnumerator.Current!.LazyValue.IsValueCreated)
                {
                    var _ = valueEnumerator.Current.LazyValue.Value;
                }
            }

            if (version == _id)
                break;
        }
    }
}

public sealed class DeferredReferenceView<TView> where TView : ReferenceView
{
    public string RefId { get; }

    public TView Value => LazyValue.Value;

    public Lazy<TView> LazyValue { get; }

    public DeferredReferenceView(string refId, Func<string, TView> factory)
    {
        RefId = refId;
        LazyValue = new Lazy<TView>(() => factory(refId));
    }
}
