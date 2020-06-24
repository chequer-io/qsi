using Qsi.Data;

namespace Qsi.Services
{
    public interface IQsiReferenceCacheRepository
    {
        bool TryGetTable(in QsiQualifiedIdentifier identifier, out QsiDataTable dataTable);

        void SetTable(in QsiQualifiedIdentifier identifier, QsiDataTable dataTable);

        bool TryGetDefinition(in QsiQualifiedIdentifier identifier, out QsiScript script);

        void SetDefinition(in QsiQualifiedIdentifier identifier, in QsiScript script);
    }
}
