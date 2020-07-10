using Qsi.Data;
using Qsi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlReferenceResolver : QsiReferenceResolverBase
    {
        #region Mock
        private QsiQualifiedIdentifier CreateIdentifier(params string[] path)
        {
            return new QsiQualifiedIdentifier(path.Select(p => new QsiIdentifier(p, p[0] == '`')));
        }

        private QsiDataTable CreateTable(params string[] path)
        {
            return new QsiDataTable
            {
                Type = QsiDataTableType.Table,
                Identifier = CreateIdentifier(path)
            };
        }

        private void AddColumns(QsiDataTable table, params string[] names)
        {
            foreach (var name in names)
            {
                var c = table.NewColumn();
                c.Name = new QsiIdentifier(name, name[0] == '`');
            }
        }
        #endregion

        protected override QsiDataTable LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = IdentifierUtility.Unescape(identifier.Identifiers[^1].Value);

            switch (tableName)
            {
                case "actor":
                    var actor = CreateTable("sakila", "actor");
                    AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");
                    return actor;

                case "actor_view":
                    var actorView = CreateTable("sakila", "actor_view");
                    actorView.Type = QsiDataTableType.View;
                    AddColumns(actorView, "actor_id", "first_name", "last_name", "last_update");
                    return actorView;

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
                    AddColumns(test1, "`c 1`", "`c 2`");
                    return test1;

                case "cs_memo":
                    var csMemo = CreateTable("sakila", "cs_memo");
                    AddColumns(csMemo, "id", "memo");
                    return csMemo;

                case "cs_memo_view":
                    var csMemoView = CreateTable("sakila", "cs_memo_view");
                    csMemoView.Type = QsiDataTableType.View;
                    AddColumns(csMemoView, "a", "b");
                    return csMemoView;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiDataTableType type)
        {
            var name = IdentifierUtility.Unescape(identifier.Identifiers[^1].Value);

            switch (name)
            {
                case "cs_memo_view":
                    return new QsiScript("CREATE ALGORITHM=UNDEFINED DEFINER=`chequer`@`%` SQL SECURITY DEFINER VIEW `cs_memo_view` AS select `cs_memo`.`id` AS `a`,`cs_memo`.`memo` AS `b` from `cs_memo`", QsiScriptType.CreateView);

                case "actor_view":
                    return new QsiScript("CREATE VIEW `sakila`.`actor_view` AS SELECT * FROM `sakila`.`actor`", QsiScriptType.CreateView);
            }

            return null;
        }

        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            if (identifier.Level == 1)
            {
                var sakila = new QsiIdentifier("sakila", false);
                identifier = new QsiQualifiedIdentifier(sakila, identifier.Identifiers[0]);
            }

            if (identifier.Level != 2)
                throw new InvalidOperationException();

            return identifier;
        }
    }
}
