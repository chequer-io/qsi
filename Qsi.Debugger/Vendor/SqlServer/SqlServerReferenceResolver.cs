using System;
using Qsi.Data;

namespace Qsi.Debugger.Vendor.SqlServer
{
    internal class SqlServerReferenceResolver : VendorReferenceResolver
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override QsiDataTable LookupTable(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiDataTableType type)
        {
            throw new NotImplementedException();
        }
    }
}
