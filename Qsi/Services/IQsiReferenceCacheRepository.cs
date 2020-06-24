using Qsi.Data;

namespace Qsi.Services
{
    public interface IQsiReferenceCacheRepository
    {
        bool TryGetTable(QsiQualifiedIdentifier identifier, out QsiDataTable dataTable);

        void SetTable(QsiQualifiedIdentifier identifier, QsiDataTable dataTable);

        bool TryGetDefinition(QsiQualifiedIdentifier identifier, out QsiScript script);

        void SetDefinition(QsiQualifiedIdentifier identifier, QsiScript script);
    }
}
