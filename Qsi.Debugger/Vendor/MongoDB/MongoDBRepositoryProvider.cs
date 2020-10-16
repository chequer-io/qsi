using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.MongoDB
{
    internal class MongoDBRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<QsiDataRowCollection> GetDataRows(QsiScript script)
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
