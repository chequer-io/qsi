using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Org.BouncyCastle.Asn1.Ocsp;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Parsing.Common;
using Qsi.Utilities;

namespace Qsi.Tests.Vendor.PostgreSQL.Driver;

public class PostgreSqlRepositoryProvider : RepositoryProviderDriverBase
{
    private const string SchemaName = "public";
    
    public PostgreSqlRepositoryProvider(DbConnection connection) : base(connection)
    {
    }

    protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
    {
        if (identifier.Level == 2 || string.IsNullOrEmpty(Connection.Database))
        {
            return identifier;
        }

        if (identifier.Level != 1)
        {
            throw new InvalidOperationException($"Identifier level is invalid: {identifier.Level}");
        }

        var id = new QsiIdentifier(SchemaName, false);
        return new QsiQualifiedIdentifier(id, identifier[0]);
    }

    protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
    {
        var names = identifier
            .Select(x => x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value)
            .ToArray();

        var table = new QsiTableStructure();

        var sql = $@"SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE from information_schema.TABLES where TABLE_SCHEMA = '{names[0]}' and TABLE_NAME = '{names[1]}' limit 1";

        using (var reader = GetDataReaderCoreAsync(new QsiScript(sql, default), null, default).Result)
        {
            if (!reader.Read())
            {
                return null;
            }

            var id0 = new QsiIdentifier(reader.GetString(0), false);
            var id1 = new QsiIdentifier(reader.GetString(1), false);
            table.Identifier = new QsiQualifiedIdentifier(id0, id1);

            table.Type = reader.GetString(2).ToUpper() switch
            {
                "BASE TABLE" => QsiTableType.Table,
                "SYSTEM VIEW" => QsiTableType.Table,
                "VIEW" => QsiTableType.View,
                _ => throw new Exception()
            };
        }

        sql = $@"select COLUMN_NAME from information_schema.COLUMNS where TABLE_SCHEMA = '{names[0]}' and TABLE_NAME = '{names[1]}' order by ORDINAL_POSITION";

        using (var reader = GetDataReaderCoreAsync(new QsiScript(sql, default), null, default).Result)
        {
            while (reader.Read())
            {
                var column = table.NewColumn();
                column.Name = new QsiIdentifier(reader.GetString(0), false);
            }
        }

        return table;
    }

    protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
    {
        var oidSql = $@"select oid from pg_catalog.pg_class where relname = '{identifier[^1]}'";

        string oid;
        
        using (var oidReader = GetDataReaderCoreAsync(new QsiScript(oidSql, default), null, default).Result)
        {
            if (!oidReader.Read())
                return null;

            oid = oidReader.GetString(0);
        }
        
        var viewSql = $@"SELECT pg_catalog.pg_get_viewdef({oid})";

        using var reader = GetDataReaderCoreAsync(new QsiScript(viewSql, QsiScriptType.Show), null, default).Result;

        if (!reader.Read())
            return null;

        var selectSql = reader.GetString(0).TrimEnd(';');

        Console.WriteLine(selectSql);
        
        var createSql = $@"CREATE VIEW {identifier} AS ({selectSql})";

        return new QsiScript(createSql, QsiScriptType.Create);
    }

    protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
    {
        throw new NotImplementedException();
    }

    protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
    {
        throw new NotImplementedException();
    }

    protected override void AddParameterValue(DbCommand command, QsiParameter parameter)
    {
        var pgCommand = (NpgsqlCommand)command;
        // pgCommand.AllResultTypesAreUnknown = true;
        pgCommand.Parameters.AddWithValue(parameter.Name, parameter.Value);
    }
}
