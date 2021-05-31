using System;
using System.Threading.Tasks;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.PostgreSql
{
    internal class PostgreSqlRepositoryProvider : VendorRepositoryProvider
    {
        protected override Task<QsiDataTable> GetDataTable(QsiScript script, QsiParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (tableName)
            {
                case "actor":
                    var actor = CreateTable("postgres", "public", "actor");
                    AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");
                    return actor;

                case "actor_view":
                    var actorView = CreateTable("postgres", "public", "actor_view");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "actor_id", "first_name", "last_name", "last_update");
                    return actorView;

                case "actor_mat_view":
                    var actorMatView = CreateTable("postgres", "public", "actor_mat_view");
                    actorMatView.Type = QsiTableType.View;
                    AddColumns(actorMatView, "actor_id", "first_name", "last_name", "last_update");
                    return actorMatView;

                case "address":
                    var address = CreateTable("postgres", "public", "address");
                    AddColumns(address, "address_id", "address", "address2", "district", "city_id", "postal_code", "phone", "location", "last_update");
                    return address;

                case "city":
                    var city = CreateTable("postgres", "public", "city");
                    AddColumns(city, "city_id", "city", "country_id", "last_update", "test");
                    return city;

                case "test 1":
                    var test1 = CreateTable("postgres", "public", "\"test 1\"");
                    AddColumns(test1, "`c 1`", "`c 2`");
                    return test1;

                case "cs_memo":
                    var csMemo = CreateTable("postgres", "public", "cs_memo");
                    AddColumns(csMemo, "id", "memo");
                    return csMemo;
            }

            return null;
        }

        protected override QsiTableStructure CreateTable(params string[] path)
        {
            var hiddenColumns = new[]
            {
                "oid",
                "tableoid",
                "xmin",
                "cmin",
                "xmax",
                "cmax",
                "ctid"
            };

            var table = base.CreateTable(path);

            foreach (var hiddenColumn in hiddenColumns)
            {
                var column = table.NewColumn();
                column.Name = new QsiIdentifier(hiddenColumn, false);
                column.IsVisible = false;
            }

            return table;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "actor_view":
                    return new QsiScript("CREATE OR REPLACE VIEW public.actor_view AS SELECT actor.actor_id, actor.first_name, actor.last_name, actor.last_update FROM actor;", QsiScriptType.Create);

                case "actor_mat_view":
                    return new QsiScript("CREATE MATERIALIZED VIEW public.actor_view AS SELECT actor.actor_id, actor.first_name, actor.last_name, actor.last_update FROM actor;", QsiScriptType.Create);
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            throw new NotImplementedException();
        }

        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            identifier = identifier.Level switch
            {
                1 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("postgres", false),
                    new QsiIdentifier("public", false),
                    identifier[0]
                ),
                2 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("postgres", false),
                    identifier[0],
                    identifier[1]
                ),
                _ => identifier
            };

            if (identifier.Level != 3)
                throw new InvalidOperationException();

            return identifier;
        }
    }
}
