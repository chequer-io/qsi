using Qsi.Data;

namespace Qsi.Services
{
    public interface IQsiReferenceResolver
    {
        QsiQualifiedIdentifier ResolveQualifiedIdentifier(in QsiQualifiedIdentifier identifier);

        QsiDataTable LookupTable(in QsiQualifiedIdentifier identifier);
    }
}
