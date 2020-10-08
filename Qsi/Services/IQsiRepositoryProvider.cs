using Qsi.Data;

namespace Qsi.Services
{
    public interface IQsiRepositoryProvider
    {
        QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);

        QsiVariable LookupVariable(QsiQualifiedIdentifier identifier);

        QsiDataRowCollection GetDataRows(QsiScript script);
    }
}
