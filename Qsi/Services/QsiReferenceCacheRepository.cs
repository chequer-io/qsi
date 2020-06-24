using System.Collections.Concurrent;
using System.Collections.Generic;
using Qsi.Data;

namespace Qsi.Services
{
    public class QsiReferenceCacheRepository : IQsiReferenceCacheRepository
    {
        private readonly ConcurrentDictionary<QsiQualifiedIdentifier, QsiDataTable> _lookupCache;
        private readonly ConcurrentDictionary<QsiQualifiedIdentifier, QsiScript> _lookupDefinitionCache;

        public QsiReferenceCacheRepository()
        {
            var comparer = new IdentifierEqualityComparer();
            _lookupCache = new ConcurrentDictionary<QsiQualifiedIdentifier, QsiDataTable>(comparer);
            _lookupDefinitionCache = new ConcurrentDictionary<QsiQualifiedIdentifier, QsiScript>(comparer);
        }

        public bool TryGetTable(in QsiQualifiedIdentifier identifier, out QsiDataTable dataTable)
        {
            return _lookupCache.TryGetValue(identifier, out dataTable);
        }

        public void SetTable(in QsiQualifiedIdentifier identifier, QsiDataTable dataTable)
        {
            _lookupCache[identifier] = dataTable;
        }

        public bool TryGetDefinition(in QsiQualifiedIdentifier identifier, out QsiScript script)
        {
            return _lookupDefinitionCache.TryGetValue(identifier, out script);
        }

        public void SetDefinition(in QsiQualifiedIdentifier identifier, in QsiScript script)
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

                if (x.Identifiers.Length != y.Identifiers.Length)
                    return false;

                for (int i = 0; i < x.Identifiers.Length; i++)
                {
                    var xIdentifier = x.Identifiers[i];
                    var yIdentifier = y.Identifiers[i];

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
