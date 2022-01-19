using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Npgsql;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Tests.Utilities;

namespace Qsi.Tests.Driver;

public class PostgreSqlRepositoryProvider : RepositoryProviderDriverBase
{
    protected static readonly string[] defautlSchemas = { "public", "pg_catalog" };

    public PostgreSqlRepositoryProvider(DbConnection connection) : base(connection)
    {
    }

    protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
    {
        if (identifier.Level == 3 || string.IsNullOrEmpty(Connection.Database))
            return identifier;

        var catalog = new QsiIdentifier(Connection.Database, false);

        switch (identifier.Level)
        {
            case 1:
                var schema = new QsiIdentifier("public", false);
                return new QsiQualifiedIdentifier(catalog, schema, identifier[0]);

            case 2:
                return new QsiQualifiedIdentifier(catalog, identifier[0], identifier[1]);

            default:
                throw new InvalidOperationException();
        }
    }

    protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
    {
        try
        {
            using var command = Connection.CreateCommand();

            command.CommandText = @$"
SELECT
    (SELECT nspname FROM pg_namespace WHERE oid = relnamespace) as column_name,
    CASE relkind
        WHEN 'r' THEN 'Table'
        WHEN 'v' THEN 'View'
        WHEN 'm' THEN 'MaterializedView'
        ELSE 'UNKNOWN'
    END as column_type,
    null as is_system
FROM
    pg_class
WHERE
    oid = $1::regclass
UNION ALL
(
    SELECT
        attname,
        atttypid::regtype::text,
        attnum < 1
    FROM
        pg_attribute
    WHERE
        attrelid = $1::regclass
        AND NOT attisdropped
    ORDER BY
        attnum
)";

            command.Parameters.Add(new NpgsqlParameter { Value = identifier.ToString() });

            using var reader = command.ExecuteReader();
            IReadOnlyDictionary<string, object>[] rows = reader.ToDictionary();

            if (rows.Length <= 1)
                return null;

            if (!Enum.TryParse((string)rows[0]["column_type"], out QsiTableType relType))
                return null;

            var schema = (string)rows[0]["column_name"];

            var result = new QsiTableStructure
            {
                Identifier = new QsiQualifiedIdentifier(
                    new QsiIdentifier(Connection.Database, false),
                    new QsiIdentifier(schema, false),
                    identifier[^1]
                ),
                Type = relType
            };

            foreach (IReadOnlyDictionary<string, object> column in rows.Skip(1))
            {
                var c = result.NewColumn();
                c.Name = new QsiIdentifier((string)column["column_name"], false);
                c.IsVisible = !(bool)column["is_system"];
            }

            return result;
        }
        catch (PostgresException e)
        {
            if (e.SqlState is PostgresErrorCodes.UndefinedTable or PostgresErrorCodes.InvalidSchemaName)
            {
                return null;
            }

            throw;
        }
    }

    protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
    {
        string sql;

        switch (type)
        {
            case QsiTableType.View:
                sql = @$"
SELECT
   'CREATE OR REPLACE VIEW {identifier[^2]}.{identifier[^1]} AS ' || definition
FROM
    pg_views
WHERE
    schemaname = $1
    AND viewname = $2";

                break;

            case QsiTableType.MaterializedView:
                sql = @$"
SELECT
   'CREATE MATERIALIZED VIEW {identifier[^2]}.{identifier[^1]} AS ' || definition
FROM
    pg_matviews
WHERE
    schemaname = $1
    AND matviewname = $2";

                break;

            default:
                throw new NotSupportedException(type.ToString());
        }

        using var command = Connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter { Value = identifier[^2].ToString() });
        command.Parameters.Add(new NpgsqlParameter { Value = identifier[^1].ToString() });

        var definition =(string)command.ExecuteScalar();

        return new QsiScript(definition, QsiScriptType.Create);
    }

    protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
    {
        throw new System.NotImplementedException();
    }

    protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
    {
        throw new System.NotImplementedException();
    }
}
