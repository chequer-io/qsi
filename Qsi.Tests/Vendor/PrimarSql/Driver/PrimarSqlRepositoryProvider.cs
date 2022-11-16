using System;
using System.Data.Common;
using Qsi.Data;
using Qsi.Data.Object;
using Qsi.Engines;

namespace Qsi.Tests.Vendor.PostgreSQL.Driver;

public class PrimarSqlRepositoryProvider : RepositoryProviderDriverBase
{
    public PrimarSqlRepositoryProvider(DbConnection connection) : base(connection)
    {
    }

    protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
    {
        throw new NotSupportedException();
    }

    protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier, ExecuteOptions options)
    {
        return identifier;
    }

    protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
    {
        var sql = $"DESCRIBE TABLE {identifier}";

        var structure = new QsiTableStructure
        {
            IsSystem = false,
            Type = QsiTableType.Table,
            Identifier = identifier
        };

        using var reader = GetDataReaderCoreAsync(new QsiScript(sql, default), null, default).Result;

        if (!reader.Read())
            return null;

        var hashKeyName = reader["HashKeyName"].ToString();
        var sortKeyName = reader["SortKeyName"].ToString();

        var hashColumn = structure.NewColumn();
        hashColumn.Name = new QsiIdentifier(hashKeyName, false);

        if (!string.IsNullOrEmpty(sortKeyName))
        {
            var sortColumn = structure.NewColumn();
            sortColumn.Name = new QsiIdentifier(sortKeyName, false);
        }

        return structure;
    }

    protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
    {
        throw new NotSupportedException();
    }

    protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
    {
        throw new NotSupportedException();
    }

    protected override void AddParameterValue(DbCommand command, QsiParameter parameter)
    {
        throw new NotSupportedException();
    }
}
