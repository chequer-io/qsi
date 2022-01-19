using System;
using System.Data.Common;
using System.Linq;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Utilities;

namespace Qsi.Tests.Vendor.MySql.Driver;

public class MySqlRepositoryProvider : RepositoryProviderDriverBase
{
    public MySqlRepositoryProvider(DbConnection connection) : base(connection)
    {
    }

    protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
    {
        if (identifier.Level == 2 || string.IsNullOrEmpty(Connection.Database))
            return identifier;

        if (identifier.Level != 1)
            throw new InvalidOperationException();

        return new QsiQualifiedIdentifier(new QsiIdentifier(Connection.Database, false), identifier[0]);
    }

    protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
    {
        var names = identifier
            .Select(x => x.IsEscaped ? IdentifierUtility.Unescape(x.Value) : x.Value)
            .ToArray();

        var table = new QsiTableStructure();

        var sql = @$"SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
from information_schema.TABLES
where TABLE_SCHEMA = '{names[0]}' and TABLE_NAME = '{names[1]}'
limit 1";

        using (var reader = GetDataReaderCoreAsync(new QsiScript(sql, default), null, default).Result)
        {
            if (!reader.Read())
                return null;

            table.Identifier = new QsiQualifiedIdentifier(
                new QsiIdentifier(reader.GetString(0), false),
                new QsiIdentifier(reader.GetString(1), false)
            );

            table.Type = reader.GetString(2).ToUpper() switch
            {
                "BASE TABLE" => QsiTableType.Table,
                "SYSTEM VIEW" => QsiTableType.Table,
                "VIEW" => QsiTableType.View,
                _ => throw new Exception()
            };
        }

        sql = @$"
select COLUMN_NAME 
from information_schema.COLUMNS
where TABLE_SCHEMA = '{names[0]}' and TABLE_NAME = '{names[1]}'
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
        using var reader = GetDataReaderCoreAsync(new QsiScript($"SHOW CREATE VIEW {identifier}", QsiScriptType.Show), null, default).Result;

        if (!reader.Read())
            return null;

        return new QsiScript(reader.GetString(1), QsiScriptType.Create);
    }

    protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
    {
        throw new NotImplementedException();
    }

    protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
    {
        throw new NotImplementedException();
    }
}
