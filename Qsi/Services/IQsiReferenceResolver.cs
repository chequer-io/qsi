using Qsi.Data;

namespace Qsi.Services
{
    public interface IQsiReferenceResolver
    {
        QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        QsiDataTable LookupTable(QsiQualifiedIdentifier identifier);

        QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiDataTableType type);
    }
}
