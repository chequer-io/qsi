using System;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Data.Object.Sequence;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.Oracle
{
    internal class OracleRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            identifier = identifier.Level switch
            {
                1 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("xe", false),
                    new QsiIdentifier("SYSTEM", false),
                    identifier[0]
                ),
                2 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("xe", false),
                    identifier[0],
                    identifier[1]
                ),
                _ => identifier
            };

            if (identifier.Level != 3)
                throw new InvalidOperationException();

            return identifier;
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableIdentifier = identifier[^1];
            var schemaIdentifier = identifier.Level >= 2 ? identifier[^2] : null;
            var tableName = IdentifierUtility.Unescape(tableIdentifier.Value);

            if (schemaIdentifier is not null && schemaIdentifier.Value != "SYSTEM")
                return null;

            switch (tableName)
            {
                case "DUAL":
                    var dual = CreateTable("xe", "SYSTEM", "DUAL");
                    AddColumns(dual, "DUMMY");
                    return dual;

                case "ACTOR":
                    var actor = CreateTable("xe", "SYSTEM", "ACTOR");
                    AddColumns(actor, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actor;

                case "ACTOR_VIEW":
                    var actorView = CreateTable("xe", "SYSTEM", "ACTOR_VIEW");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE", "FIRST_NAME||LAST_NAME");
                    return actorView;

                case "ACTOR_VIEW2":
                    var actorView2 = CreateTable("xe", "SYSTEM", "ACTOR_VIEW2");
                    actorView2.Type = QsiTableType.View;
                    AddColumns(actorView2, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actorView2;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "ACTOR_VIEW":
                    return new QsiScript("CREATE VIEW actor_view AS (select actor_id, first_name, last_name, last_update, first_name || last_name from actor);", QsiScriptType.Create);

                case "ACTOR_VIEW2":
                    return new QsiScript("CREATE VIEW actor_view AS (select * from actor);", QsiScriptType.Create);
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (type)
            {
                case QsiObjectType.Sequence:
                    switch (name)
                    {
                        case "TEST_SEQUENCE":
                            return new QsiSequenceObject(CreateIdentifier("xe", "SYSTEM", "TEST_SEQUENCE"));
                    }

                    break;
            }

            return null;
        }
    }
}
