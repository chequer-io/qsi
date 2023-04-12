using Qsi.Data;
using System;
using Qsi.Data.Object;
using Qsi.Engines;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (tableName)
            {
                case "actor":
                    var actor = CreateTable("sakila", "actor");
                    AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");
                    return actor;

                case "actor_view":
                    var actorView = CreateTable("sakila", "actor_view");
                    actorView.Type = QsiTableType.View;
                    AddColumns(actorView, "actor_id", "first_name", "last_name", "last_update", "first_name || last_name");
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
                    csMemoView.Type = QsiTableType.View;
                    AddColumns(csMemoView, "a", "b");
                    return csMemoView;

                case "hidden_test":
                    var hiddenTest = CreateTable("sakila", "hidden_test");
                    AddColumns(hiddenTest, "col1", "hidden1", "col2", "hidden2");
                    hiddenTest.Columns[1].IsVisible = false;
                    hiddenTest.Columns[3].IsVisible = false;
                    return hiddenTest;

                case "hidden_test2":
                    var hiddenTest2 = CreateTable("sakila", "hidden_test2");
                    AddColumns(hiddenTest2, "col1", "col2", "hidden1", "hidden2");
                    hiddenTest2.Columns[2].IsVisible = false;
                    hiddenTest2.Columns[3].IsVisible = false;
                    return hiddenTest2;

                case "user":
                    var user = CreateTable("mysql", "user");

                    AddColumns(user, "Host", "User",
                        "Select_priv", "Insert_priv", "Update_priv", "Delete_priv",
                        "Create_priv", "Drop_priv", "Reload_priv", "Shutdown_priv",
                        "Process_priv", "File_priv", "Grant_priv", "References_priv",
                        "Index_priv", "Alter_priv", "Show_db_priv", "Super_priv",
                        "Create_tmp_table_priv", "Lock_tables_priv", "Execute_priv",
                        "Repl_slave_priv", "Repl_client_priv", "Create_view_priv",
                        "Show_view_priv", "Create_routine_priv", "Alter_routine_priv",
                        "Create_user_priv", "Event_priv", "Trigger_priv",
                        "Create_tablespace_priv", "ssl_type", "ssl_cipher",
                        "x509_issuer", "x509_subject", "max_questions", "max_updates",
                        "max_connections", "max_user_connections", "plugin",
                        "authentication_string", "password_expired",
                        "password_last_changed", "password_lifetime",
                        "account_locked", "Create_role_priv", "Drop_role_priv",
                        "Password_reuse_history", "Password_reuse_time",
                        "Password_require_current", "User_attributes", "password");

                    return user;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "cs_memo_view":
                    return new QsiScript("CREATE ALGORITHM=UNDEFINED DEFINER=`chequer`@`%` SQL SECURITY DEFINER VIEW `cs_memo_view` AS select `cs_memo`.`id` AS `a`,`cs_memo`.`memo` AS `b` from `cs_memo`", QsiScriptType.Create);

                case "actor_view":
                    return new QsiScript("CREATE VIEW `sakila`.`actor_view` (actor_id, first_name, last_name, last_update, `first_name || last_name`) AS SELECT *, first_name || last_name FROM `sakila`.`actor`", QsiScriptType.Create);

                case "stmt1" when type == QsiTableType.Prepared:
                    return new QsiScript("SELECT * FROM actor", QsiScriptType.Select);
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            var name = IdentifierUtility.Unescape(identifier[^1].Value);

            switch (name)
            {
                case "stmt1":
                    return new QsiVariable
                    {
                        Identifier = CreateIdentifier("stmt1"),
                        Type = QsiDataType.String,
                        Value = "SELECT * FROM actor"
                    };
            }

            return null;
        }

        protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            return null;
        }

        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier, ExecuteOptions executeOptions)
        {
            if (identifier.Level == 1)
            {
                var sakila = new QsiIdentifier("sakila", false);
                identifier = new QsiQualifiedIdentifier(sakila, identifier[0]);
            }

            if (identifier.Level != 2)
                throw new InvalidOperationException();

            return identifier;
        }
    }
}
