using Qsi.Data;
using Qsi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qsi.Debugger.Vendor.MySql
{
    internal class MySqlReferenceResolver : IQsiReferenceResolver
    {
        private readonly Dictionary<int, QsiDataTable> _fakeTables;
        private readonly Dictionary<int, QsiScript> _fakeDefinitions;

        #region Initialize
        public MySqlReferenceResolver()
        {
            _fakeTables = new Dictionary<int, QsiDataTable>();
            _fakeDefinitions = new Dictionary<int, QsiScript>();

            var actor = CreateTable("sakila", "actor");
            AddColumns(actor, "actor_id", "first_name", "last_name", "last_update");

            var address = CreateTable("sakila", "address");
            AddColumns(address, "address_id", "address", "address2", "district", "city_id", "postal_code", "phone", "location", "last_update");

            var city = CreateTable("sakila", "`test 1`");
            AddColumns(city, "`c 1`", "`c 2`");
        }

        private QsiQualifiedIdentifier CreateIdentifier(params string[] path)
        {
            return new QsiQualifiedIdentifier(path.Select(p => new QsiIdentifier(p, p[0] == '`')));
        }

        private QsiDataTable CreateTable(params string[] path)
        {
            var t = new QsiDataTable();
            t.Type = QsiDataTableType.Table;
            t.Identifier = CreateIdentifier(path);
            _fakeTables[t.Identifier.GetHashCode()] = t;
            return t;
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

        public QsiDataTable LookupTable(QsiQualifiedIdentifier identifier)
        {
            if (_fakeTables.TryGetValue(identifier.GetHashCode(), out var table))
                return table;

            return null;
        }

        public QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiDataTableType type)
        {
            return null;
        }

        public QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
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
