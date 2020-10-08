using Qsi.Data;

namespace Qsi.Services
{
    public abstract class QsiRepositoryProviderBase : IQsiRepositoryProvider
    {
        public IQsiCacheRepository CacheRepository { get; set; }

        protected abstract QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        protected abstract QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);

        protected abstract QsiVariable LookupVariable(QsiQualifiedIdentifier identifier);

        protected abstract QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        protected abstract QsiDataRowCollection GetDataRows(QsiScript script);

        #region IQsiRepositoryProvider
        QsiTableStructure IQsiRepositoryProvider.LookupTable(QsiQualifiedIdentifier identifier)
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

        QsiScript IQsiRepositoryProvider.LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
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

        QsiVariable IQsiRepositoryProvider.LookupVariable(QsiQualifiedIdentifier identifier)
        {
            return LookupVariable(identifier);
        }

        QsiQualifiedIdentifier IQsiRepositoryProvider.ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            return ResolveQualifiedIdentifier(identifier);
        }

        QsiDataRowCollection IQsiRepositoryProvider.GetDataRows(QsiScript script)
        {
            return GetDataRows(script);
        }
        #endregion
    }
}
