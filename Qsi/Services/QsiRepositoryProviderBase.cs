using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Engines;

namespace Qsi.Services
{
    public abstract class QsiRepositoryProviderBase : IQsiRepositoryProvider
    {
        public IQsiCacheRepository CacheRepository { get; set; }

        protected abstract QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        protected abstract QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);

        protected abstract QsiVariable LookupVariable(QsiQualifiedIdentifier identifier);

        protected abstract QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type);

        protected abstract QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier, ExecuteOptions options);

        protected abstract Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters, ExecuteOptions options, CancellationToken cancellationToken);

        protected abstract Task<IDataReader> GetDataReaderAsync(QsiScript script, QsiParameter[] parameters, ExecuteOptions options, CancellationToken cancellationToken);

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

        QsiObject IQsiRepositoryProvider.LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            return LookupObject(identifier, type);
        }

        QsiQualifiedIdentifier IQsiRepositoryProvider.ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier, ExecuteOptions executeOptions)
        {
            return ResolveQualifiedIdentifier(identifier, executeOptions);
        }

        Task<QsiDataTable> IQsiRepositoryProvider.GetDataTable(QsiScript script, QsiParameter[] parameters, ExecuteOptions executeOptions, CancellationToken cancellationToken)
        {
            return GetDataTable(script, parameters, executeOptions, cancellationToken);
        }

        Task<IDataReader> IQsiRepositoryProvider.GetDataReaderAsync(QsiScript script, QsiParameter[] parameters, ExecuteOptions executeOptions, CancellationToken cancellationToken)
        {
            return GetDataReaderAsync(script, parameters, executeOptions, cancellationToken);
        }
        #endregion
    }
}
