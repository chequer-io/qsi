using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.Hana
{
    internal class HanaRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            // TODO: has search path ?
            return identifier;
        }

        protected override Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var name = identifier[^1];
            var tableName = name.IsEscaped ? IdentifierUtility.Unescape(name.Value) : name.Value;

            switch (tableName)
            {
                case "actor":
                    var actor = CreateTable("sakila", "actor");
                    AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");
                    return actor;

                case "address":
                    var address = CreateTable("sakila", "address");
                    AddColumns(address, "address_id", "address", "address2", "district", "city_id", "postal_code", "phone", "location", "last_update");
                    return address;

                case "city":
                    var city = CreateTable("sakila", "city");
                    AddColumns(city, "city_id", "city", "country_id", "last_update", "test");
                    return city;

                case "test 1":
                    var test1 = CreateTable("sakila", "`test 1`");
                    AddColumns(test1, "\"c 1\"", "\"c 2\"");
                    return test1;

                case "cs_memo":
                    var csMemo = CreateTable("sakila", "cs_memo");
                    AddColumns(csMemo, "id", "memo");
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
