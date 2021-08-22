using System;
using Qsi.Data;
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

            var tableName = tableIdentifier.IsEscaped ? IdentifierUtility.Unescape(tableIdentifier.Value) : tableIdentifier.Value.ToUpper();

            if (schemaIdentifier != null)
            {
                var schemaName = schemaIdentifier.IsEscaped ? IdentifierUtility.Unescape(schemaIdentifier.Value) : schemaIdentifier.Value.ToUpper();

                if (schemaName != "SYSTEM")
                    return null;
            }

            switch (tableName)
            {
                case "DUAL":
                    var dual = CreateTable("xe", "SYSTEM", "DUAL");
                    AddColumns(dual, "dummy");
                    return dual;

                case "ACTOR":
                    var actor = CreateTable("xe", "SYSTEM", "ACTOR");
                    AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");
                    return actor;

                case "ACTOR_VIEW":
                    var actorView = CreateTable("xe", "SYSTEM", "ACTOR_VIEW");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "actor_id", "first_name", "last_name", "last_update", "first_name||last_name");
                    return actorView;

                case "ACTOR_VIEW2":
                    var actorView2 = CreateTable("xe", "SYSTEM", "ACTOR_VIEW2");
                    actorView2.Type = QsiTableType.View;
                    AddColumns(actorView2, "actor_id", "first_name", "last_name", "last_update");
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
    }
}
