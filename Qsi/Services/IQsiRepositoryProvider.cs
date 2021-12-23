using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Data.Object;

namespace Qsi.Services
{
    public interface IQsiRepositoryProvider
    {
        QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);

        QsiVariable LookupVariable(QsiQualifiedIdentifier identifier);

        QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type);

        [Obsolete("Use GetDataReader")]
        Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken);

        Task<IDataReader> GetDataReaderAsync(QsiScript script, QsiParameter[] parameters, CancellationToken cancellationToken);
    }
}
