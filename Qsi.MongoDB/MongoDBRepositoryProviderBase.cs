using System;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.MongoDB.Lookup;
using Qsi.Services;

namespace Qsi.MongoDB
{
    public abstract class MongoDBRepositoryProviderBase : IQsiRepositoryProvider
    {
        public abstract MongoDBLookupResult LookupObject(string objectName);
        
        QsiQualifiedIdentifier IQsiRepositoryProvider.ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
            => throw new NotSupportedException();

        QsiTableStructure IQsiRepositoryProvider.LookupTable(QsiQualifiedIdentifier identifier)
            => throw new NotSupportedException();

        QsiScript IQsiRepositoryProvider.LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
            => throw new NotSupportedException();

        QsiVariable IQsiRepositoryProvider.LookupVariable(QsiQualifiedIdentifier identifier)
            => throw new NotSupportedException();

        Task<QsiDataRowCollection> IQsiRepositoryProvider.GetDataRows(QsiScript script)
            => throw new NotSupportedException();

        Task<QsiDataRowCollection> IQsiRepositoryProvider.GetDataRows(QsiQualifiedIdentifier identifier, string whereClause)
            => throw new NotSupportedException();
    }
}
