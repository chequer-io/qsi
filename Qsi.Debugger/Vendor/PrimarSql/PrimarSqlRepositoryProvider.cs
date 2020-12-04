using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.PrimarSql
{
    internal class PrimarSqlRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            return identifier;
        }

        protected override Task<QsiDataTable> GetDataTable(QsiScript script)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (tableName)
            {
                case "actor":
                    var actor = CreateTable("actor");
                    return actor;

                case "address":
                    var address = CreateTable("address");
                    return address;

                case "city":
                    var city = CreateTable("city");
                    return city;

                case "test 1":
                    var test1 = CreateTable("`test 1`");
                    return test1;

                case "cs_memo":
                    var csMemo = CreateTable("cs_memo");
                    return csMemo;
            }

            return null;
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
