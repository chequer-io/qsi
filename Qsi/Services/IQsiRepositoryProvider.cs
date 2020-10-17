using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Services
{
    public interface IQsiRepositoryProvider
    {
        QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier);

        QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier);

        QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type);

        QsiVariable LookupVariable(QsiQualifiedIdentifier identifier);

        Task<QsiDataRowCollection> GetDataRows(QsiScript script);
    }
}
