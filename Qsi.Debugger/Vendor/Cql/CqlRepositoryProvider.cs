using System.Threading.Tasks;
using Qsi.Data;

namespace Qsi.Debugger.Vendor.Cql
{
    internal class CqlRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<QsiDataTable> GetDataTable(QsiScript script)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }
    }
}
