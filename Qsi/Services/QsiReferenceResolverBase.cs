using Qsi.Data;

namespace Qsi.Services
{
    public abstract class QsiReferenceResolverBase : IQsiReferenceResolver
    {
        public IQsiReferenceCacheRepository CacheRepository { get; set; }

        protected abstract QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        protected abstract QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);

        protected abstract QsiVariable LookupVariable(QsiQualifiedIdentifier identifier);

        protected abstract QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        #region IQsiReferenceResolver
        QsiTableStructure IQsiReferenceResolver.LookupTable(QsiQualifiedIdentifier identifier)
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

        QsiScript IQsiReferenceResolver.LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
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

        QsiVariable IQsiReferenceResolver.LookupVariable(QsiQualifiedIdentifier identifier)
        {
            return LookupVariable(identifier);
        }

        QsiQualifiedIdentifier IQsiReferenceResolver.ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            return ResolveQualifiedIdentifier(identifier);
        }
        #endregion
    }
}
