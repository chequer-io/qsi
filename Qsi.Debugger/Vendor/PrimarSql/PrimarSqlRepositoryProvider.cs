using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Engines;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.PrimarSql
{
    internal class PrimarSqlRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier, ExecuteOption executeOption)
        {
            return identifier;
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (tableName)
            {
                case "actor":
                    var actor = CreateTable("actor");
                    AddColumns(actor, "actor_id", "first_name");
                    return actor;

                case "address":
                    var address = CreateTable("address");
                    AddColumns(address, "address_id", "address");
                    return address;

                case "city":
                    var city = CreateTable("city");
                    AddColumns(city, "city_id", "city");
                    return city;

                case "test 1":
                    var test1 = CreateTable("`test 1`");
                    AddColumns(test1, "`c 1`", "`c 2`");
                    return test1;

                case "cs_memo":
                    var csMemo = CreateTable("cs_memo");
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

        protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            return null;
        }
    }
}
