using System.Collections.Concurrent;
using System.Collections.Generic;
using Qsi.Data;

namespace Qsi.Services
{
    public class QsiReferenceCacheRepository : IQsiReferenceCacheRepository
    {
        private readonly ConcurrentDictionary<QsiQualifiedIdentifier, QsiTableStructure> _lookupCache;
        private readonly ConcurrentDictionary<QsiQualifiedIdentifier, QsiScript> _lookupDefinitionCache;

        public QsiReferenceCacheRepository()
        {
            var comparer = new IdentifierEqualityComparer();
            _lookupCache = new ConcurrentDictionary<QsiQualifiedIdentifier, QsiTableStructure>(comparer);
            _lookupDefinitionCache = new ConcurrentDictionary<QsiQualifiedIdentifier, QsiScript>(comparer);
        }

        public bool TryGetTable(QsiQualifiedIdentifier identifier, out QsiTableStructure tableStructure)
        {
            return _lookupCache.TryGetValue(identifier, out tableStructure);
        }

        public void SetTable(QsiQualifiedIdentifier identifier, QsiTableStructure tableStructure)
        {
            _lookupCache[identifier] = tableStructure;
        }

        public bool TryGetDefinition(QsiQualifiedIdentifier identifier, out QsiScript script)
        {
            return _lookupDefinitionCache.TryGetValue(identifier, out script);
        }

        public void SetDefinition(QsiQualifiedIdentifier identifier, QsiScript script)
        {
            _lookupDefinitionCache[identifier] = script;
        }

        public void Clear()
        {
            _lookupCache.Clear();
            _lookupDefinitionCache.Clear();
            OnClear();
        }

        protected virtual void OnClear()
        {
        }

        public sealed class IdentifierEqualityComparer : IEqualityComparer<QsiQualifiedIdentifier>
        {
            public bool Equals(QsiQualifiedIdentifier x, QsiQualifiedIdentifier y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (x == null || y == null)
                    return false;

                if (x.Level != y.Level)
                    return false;

                for (int i = 0; i < x.Level; i++)
                {
                    var xIdentifier = x[i];
                    var yIdentifier = y[i];

                    if (xIdentifier.IsEscaped != yIdentifier.IsEscaped)
                        return false;

                    if (xIdentifier.Value != yIdentifier.Value)
                        return false;
                }

                return true;
            }

            public int GetHashCode(QsiQualifiedIdentifier obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
