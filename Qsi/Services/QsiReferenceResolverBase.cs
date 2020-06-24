using Qsi.Data;

namespace Qsi.Services
{
    public abstract class QsiReferenceResolverBase : IQsiReferenceResolver
    {
        public IQsiReferenceCacheRepository CacheRepository { get; set; }

        protected abstract QsiDataTable LookupTable(in QsiQualifiedIdentifier identifier);

        protected abstract QsiScript LookupDefinition(in QsiQualifiedIdentifier identifier, QsiDataTableType type);

        protected abstract QsiQualifiedIdentifier ResolveQualifiedIdentifier(in QsiQualifiedIdentifier identifier);

        #region IQsiReferenceResolver
        QsiDataTable IQsiReferenceResolver.LookupTable(in QsiQualifiedIdentifier identifier)
        {
            if (CacheRepository == null)
                return LookupTable(identifier);

            if (!CacheRepository.TryGetTable(identifier, out var table))
            {
                table = LookupTable(identifier);
                CacheRepository.SetTable(identifier, table);
            }

            return table;
        }

        QsiScript IQsiReferenceResolver.LookupDefinition(in QsiQualifiedIdentifier identifier, QsiDataTableType type)
        {
            if (CacheRepository == null)
                return LookupDefinition(identifier, type);

            if (!CacheRepository.TryGetDefinition(identifier, out var definition))
            {
                definition = LookupDefinition(identifier, type);
                CacheRepository.SetDefinition(identifier, definition);
            }

            return definition;
        }

        QsiQualifiedIdentifier IQsiReferenceResolver.ResolveQualifiedIdentifier(in QsiQualifiedIdentifier identifier)
        {
            return ResolveQualifiedIdentifier(identifier);
        }
        #endregion
    }
}
