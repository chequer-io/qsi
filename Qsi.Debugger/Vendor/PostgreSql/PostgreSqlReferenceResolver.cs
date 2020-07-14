using Qsi.Data;
using Qsi.Services;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlReferenceResolver : QsiReferenceResolverBase
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiDataTable LookupTable(QsiQualifiedIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiDataTableType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
