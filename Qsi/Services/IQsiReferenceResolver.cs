using Qsi.Data;

namespace Qsi.Services
{
    public interface IQsiReferenceResolver
    {
        QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);
    }
}
