using Qsi.Data;

namespace Qsi.Services
{
    public interface IQsiReferenceCacheRepository
    {
        bool TryGetTable(QsiQualifiedIdentifier identifier, out QsiTableStructure tableStructure);

        void SetTable(QsiQualifiedIdentifier identifier, QsiTableStructure tableStructure);

        bool TryGetDefinition(QsiQualifiedIdentifier identifier, out QsiScript script);

        void SetDefinition(QsiQualifiedIdentifier identifier, QsiScript script);
    }
}
