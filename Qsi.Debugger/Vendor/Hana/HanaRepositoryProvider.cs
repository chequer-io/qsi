using System;
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

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "DUMMY":
                    var dummy = CreateTable("SYS", "DUMMY");
                    AddColumns(dummy, "DUMMY");
                    return dummy;

                case "ACTOR":
                    var actor = CreateTable("SAKILA", "ACTOR");
                    AddColumns(actor, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actor;

                case "ACTOR_VIEW":
                    var actorView = CreateTable("SAKILA", "ACTOR_VIEW");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE", "FIRST_NAME || LAST_NAME");
                    return actorView;

                case "ADDRESS":
                    var address = CreateTable("SAKILA", "ADDRESS");
                    AddColumns(address, "ADDRESS_ID", "ADDRESS", "ADDRESS2", "DISTRICT", "CITY_ID", "POSTAL_CODE", "PHONE", "LOCATION", "LAST_UPDATE");
                    return address;

                case "CITY":
                    var city = CreateTable("SAKILA", "CITY");
                    AddColumns(city, "CITY_ID", "CITY", "COUNTRY_ID", "LAST_UPDATE", "TEST");
                    return city;

                case "test 1":
                    var test1 = CreateTable("SAKILA", "\"test 1\"");
                    AddColumns(test1, "\"c 1\"", "\"c 2\"");
                    return test1;

                case "CS_MEMO":
                    var csMemo = CreateTable("SAKILA", "CS_MEMO");
                    AddColumns(csMemo, "ID", "MEMO");
                    return csMemo;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "ACTOR_VIEW":
                    return new QsiScript(
                        "CREATE VIEW ACTOR_VIEW AS SELECT ACTOR_ID, FIRST_NAME, LAST_NAME, FIRST_NAME || LAST_NAME FROM ACTOR FORCE WITH CHECK OPTION",
                        QsiScriptType.Create
                    );
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }
    }
}
