using System;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Debugger.Vendor.Oracle
{
    internal sealed class OracleReferenceResolver : VendorReferenceResolver
    {
        protected override QsiDataTable LookupTable(QsiQualifiedIdentifier identifier)
        {
            var tableName = GetName(identifier[^1]);

            switch (tableName)
            {
                case "DUAL":
                    var dual = CreateTable("PUBLIC", "DUAL");
                    AddColumns(dual, "DUMMY");
                    return dual;

                case "ACTOR":
                    var actor = CreateTable("PUBLIC", "ACTOR");
                    AddColumns(actor, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actor;

                case "ACTOR_VIEW_1":
                    var actorView1 = CreateTable("PUBLIC", "ACTOR_VIEW_1");
                    actorView1.Type = QsiDataTableType.View;
                    AddColumns(actorView1, "ACTOR_ID", "FIRST_NAME", "LAST_NAME", "LAST_UPDATE");
                    return actorView1;

                case "ACTOR_VIEW_2":
                    var actorView2 = CreateTable("PUBLIC", "ACTOR_VIEW_2");
                    actorView2.Type = QsiDataTableType.View;
                    AddColumns(actorView2, "A", "B", "C", "D");
                    return actorView2;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiDataTableType type)
        {
            var name = GetName(identifier[^1]);

            switch (name)
            {
                case "ACTOR_VIEW_1":
                    return new QsiScript("CREATE VIEW \"CHEQUER\".\"ACTOR_VIEW_1\" (\"ACTOR_ID\", \"FIRST_NAME\", \"LAST_NAME\", \"LAST_UPDATE\") AS select \"ACTOR_ID\",\"FIRST_NAME\",\"LAST_NAME\",\"LAST_UPDATE\" from actor", QsiScriptType.CreateView);

                case "ACTOR_VIEW_2":
                    return new QsiScript("CREATE VIEW \"CHEQUER\".\"ACTOR_VIEW_2\" (\"A\", \"B\", \"C\", \"D\") AS select \"ACTOR_ID\",\"FIRST_NAME\",\"LAST_NAME\",\"LAST_UPDATE\" from actor", QsiScriptType.CreateView);
            }

            return null;
        }

        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            if (identifier.Level == 1)
            {
                var sys = new QsiIdentifier("PUBLIC", false);
                identifier = new QsiQualifiedIdentifier(sys, identifier[0]);
            }

            if (identifier.Level != 2)
                throw new InvalidOperationException();

            return identifier;
        }

        private static string GetName(QsiIdentifier identifier)
        {
            if (identifier.IsEscaped)
                return IdentifierUtility.Unescape(identifier.Value);

            return identifier.Value;
        }
    }
}
