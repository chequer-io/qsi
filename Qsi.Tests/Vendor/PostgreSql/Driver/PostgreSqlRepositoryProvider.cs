using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Npgsql;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Data.Object.Function;
using Qsi.Engines;
using Qsi.Utilities;

namespace Qsi.Tests.PostgreSql.Driver;

public class PostgreSqlRepositoryProvider : RepositoryProviderDriverBase
{
    public PostgreSqlRepositoryProvider(DbConnection connection) : base(connection)
    {
    }

    protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier, ExecuteOptions options)
    {
        if (identifier.Level == 3 || string.IsNullOrEmpty(Connection.Database))
            return identifier;

        var database = new QsiIdentifier(Connection.Database, false);

        if (identifier.Level == 2)
            return new QsiQualifiedIdentifier(database, identifier[0], identifier[1]);

        const string query = "select current_schema()";

        using var reader = GetDataReaderCoreAsync(new QsiScript(query, default), null, default).Result;

        if (!reader.Read())
            return null;

        var schema = new QsiIdentifier(reader.GetValue(0) as string ?? "public", false);

        if (identifier.Level == 1)
            return new QsiQualifiedIdentifier(database, schema, identifier[0]);

        throw new InvalidOperationException();
    }

    protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
    {
        var names = identifier
            .Select(x => x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value)
            .ToArray();

        var table = new QsiTableStructure();

        var sql = @$"
SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
from information_schema.TABLES
where TABLE_CATALOG = '{names[0]}' and TABLE_SCHEMA = '{names[1]}' and TABLE_NAME = '{names[2]}'
limit 1";

        using (var reader = GetDataReaderCoreAsync(new QsiScript(sql, default), null, default).Result)
        {
            if (!reader.Read())
                return null;

            table.Identifier = new QsiQualifiedIdentifier(
                new QsiIdentifier(reader.GetString(0), false),
                new QsiIdentifier(reader.GetString(1), false),
                new QsiIdentifier(reader.GetString(2), false));

            table.Type = reader.GetString(3).ToUpper() switch
            {
                "BASE TABLE" => QsiTableType.Table,
                "SYSTEM VIEW" => QsiTableType.Table,
                "VIEW" => QsiTableType.View,
                _ => throw new InvalidOperationException()
            };
        }

        sql = $@"
select COLUMN_NAME 
from information_schema.COLUMNS
where TABLE_CATALOG = '{names[0]}' and TABLE_SCHEMA = '{names[1]}' and TABLE_NAME = '{names[2]}'
order by ORDINAL_POSITION";

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
        var oidSql = $@"
SELECT oid 
FROM pg_catalog.pg_class
WHERE relname = '{identifier[^1]}'";

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
        throw new System.NotImplementedException();
    }

    protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
    {
        return type switch
        {
            QsiObjectType.Function => new QsiFunctionList(LookupFunction(identifier)),
            _ => null
        };
    }

    protected IEnumerable<QsiFunctionObject> LookupFunction(QsiQualifiedIdentifier identifier)
    {
        var funcInformation = @$"select oid, pronargs from ""{identifier[0]}"".pg_catalog.pg_proc
         where
             proname = '{identifier[2]}' and
             pronamespace = (select oid from pg_namespace where nspname = '{identifier[1]}' limit 1);";

        var funcDefinitions = new List<(string, int)>();

        using (var reader = GetDataReaderCoreAsync(new QsiScript(funcInformation, QsiScriptType.Select), null, default).Result)
        {
            while (reader.Read())
            {
                var oid = reader.GetString(0);
                var argsCount = reader.GetString(1);

                funcDefinitions.Add((oid, int.Parse(argsCount)));
            }
        }

        foreach (var (oid, argsCount) in funcDefinitions)
        {
            using var defReader = GetDataReaderCoreAsync(new QsiScript($@"SELECT pg_catalog.pg_get_functiondef({oid})", QsiScriptType.Select), null, default).Result;

            if (!defReader.Read())
                continue;

            yield return new QsiFunctionObject(identifier, defReader.GetString(0), argsCount);
        }
    }

    protected override void AddParameterValue(DbCommand command, QsiParameter parameter)
    {
        var pgCommand = (NpgsqlCommand)command;
        // pgCommand.AllResultTypesAreUnknown = true;
        pgCommand.Parameters.Add(new NpgsqlParameter { Value = parameter.Value });
        // pgCommand.Parameters.AddWithValue(parameter.Name, parameter.Value);
    }
}
