using System;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.SqlServer
{
    internal class SqlServerRepositoryProvider : VendorRepositoryProvider
    {
        protected override Task<QsiDataRowCollection> GetDataRows(QsiScript script)
        {
            throw new NotImplementedException();
        }

        protected override Task<QsiDataRowCollection> GetDataRows(string fromClause, string whereClause)
        {
            throw new NotImplementedException();
        }

        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            identifier = identifier.Level switch
            {
                1 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("master", false),
                    new QsiIdentifier("chequer", false),
                    identifier[0]
                ),
                2 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("master", false),
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
                case "actor":
                    var actor = CreateTable("master", "chequer", "actor");
                    AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");
                    return actor;

                case "actor_view":
                    var actorView = CreateTable("master", "chequer", "actor_view");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "actor_id", "first_name", "last_name", "last_update", "first_name + last_name");
                    return actorView;

                case "actor_view2":
                    var actorView2 = CreateTable("master", "chequer", "actor_view2");
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
                case "actor_view":
                    return new QsiScript("CREATE VIEW [chequer].[actor_view] (actor_id, first_name, last_name, last_update, [first_name + last_name]) AS SELECT *, first_name + last_name FROM [chequer].[actor]", QsiScriptType.Create);

                case "actor_view2":
                    return new QsiScript("CREATE VIEW [chequer].[actor_view2] AS SELECT * FROM [chequer].[actor]", QsiScriptType.Create);
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }
    }
}
