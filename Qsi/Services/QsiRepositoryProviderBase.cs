using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Services
{
    public abstract class QsiRepositoryProviderBase : IQsiRepositoryProvider
    {
        public IQsiCacheRepository CacheRepository { get; set; }

        protected abstract QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        protected abstract QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);

        protected abstract QsiVariable LookupVariable(QsiQualifiedIdentifier identifier);

        protected abstract QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        protected abstract Task<QsiDataRowCollection> GetDataRows(QsiScript script);

        protected abstract Task<QsiDataRowCollection> GetDataRows(QsiQualifiedIdentifier identifier, string whereClause);

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

        Task<QsiDataRowCollection> IQsiRepositoryProvider.GetDataRows(QsiScript script)
        {
            return GetDataRows(script);
        }

        Task<QsiDataRowCollection> IQsiRepositoryProvider.GetDataRows(QsiQualifiedIdentifier identifier, string whereClause)
        {
            return GetDataRows(identifier, whereClause);
        }
        #endregion
    }
}
