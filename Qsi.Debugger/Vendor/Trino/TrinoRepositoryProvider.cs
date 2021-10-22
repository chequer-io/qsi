using System;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.Trino
{
    internal class TrinoRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            identifier = identifier.Level switch
            {
                1 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("TRINO", false),
                    new QsiIdentifier("CHEQUER", false),
                    identifier[0]
                ),
                2 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("TRINO", false),
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
            var tableName = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (tableName)
            {
                case "ACTOR":
                    var actor = CreateTable("TRINO", "CHEQUER", "ACTOR");
                    AddColumns(actor, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actor;

                case "ACTOR_VIEW":
                    var actorView = CreateTable("TRINO", "CHEQUER", "ACTOR_VIEW");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE", "FIRST_NAME||LAST_NAME");
                    return actorView;

                case "ACTOR_VIEW2":
                    var actorView2 = CreateTable("TRINO", "CHEQUER", "ACTOR_VIEW2");
                    actorView2.Type = QsiTableType.View;
                    AddColumns(actorView2, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actorView2;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            throw new NotImplementedException();
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }
    }
}
