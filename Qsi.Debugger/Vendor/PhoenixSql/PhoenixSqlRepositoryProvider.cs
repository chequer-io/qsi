using System;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.PhoenixSql
{
    internal class PhoenixSqlRepositoryProvider : VendorRepositoryProvider
    {
        protected override Task<QsiDataTable> GetDataTable(QsiScript script)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (tableName)
            {
                case "ACTOR":
                    var actor = CreateTable("SECURE", "ACTOR");
                    AddColumns(actor, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actor;

                case "ADDRESS":
                    var address = CreateTable("SECURE", "ADDRESS");
                    AddColumns(address, "ADDRESS_ID", "ADDRESS", "ADDRESS2", "DISTRICT", "CITY_ID", "POSTAL_CODE", "PHONE", "LOCATION", "LAST_UPDATE");
                    return address;

                case "CITY":
                    var city = CreateTable("SECURE", "CITY");
                    AddColumns(city, "CITY_ID", "CITY", "COUNTRY_ID", "LAST_UPDATE", "TEST");
                    return city;

                case "CS_MEMO":
                    var csMemo = CreateTable("SECURE", "CS_MEMO");
                    AddColumns(csMemo, "ID", "MEMO");
                    return csMemo;

                case "CS_MEMO_VIEW":
                    var csMemoView = CreateTable("SECURE", "CS_MEMO_VIEW");
                    csMemoView.Type = QsiTableType.View;
                    AddColumns(csMemoView, "ID", "MEMO");
                    return csMemoView;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            switch (identifier.ToString())
            {
                case "SECURE.CS_MEMO_VIEW":
                    return new QsiScript("CREATE VIEW SECURE.CS_MEMO_VIEW (N INTEGER PRIMARY KEY DESC, Q DECIMAL(3, 1) ARRAY[2], P VARCHAR DEFAULT 'querypie') AS SELECT * FROM SECURE.CS_MEMO WHERE ID = 1", QsiScriptType.Create);
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new System.NotImplementedException();
        }

        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            return identifier;
        }
    }
}
