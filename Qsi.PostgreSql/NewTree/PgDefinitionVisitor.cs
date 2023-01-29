using System;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.NewTree.Nodes;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree;

internal static partial class PgNodeVisitor
{
    public static IQsiTreeNode Visit(CreateStmt node)
    {
        var createTable = new PgTableDefinitionNode
        {
            IsCreateTableAs = true,
            Relpersistence = node.Relation.Relpersistence.ToRelpersistence(),
            AccessMethod = node.AccessMethod,
        };

        if (node.IfNotExists)
            createTable.ConflictBehavior = QsiDefinitionConflictBehavior.Ignore;

        if (node.OfTypename is { } ofTypeName)
        {
            // createTable.OfType = ofTypeName.Names
        }

        return createTable;
    }

    public static IQsiDefinitionNode Visit(CreateTableAsStmt node)
    {
        switch (node.Objtype)
        {
            // CREATE TABLE
            case ObjectType.ObjectTable:
                return VisitCreateObjectTable(node);

            // CREATE MATERIALIZED VIEW
            case ObjectType.ObjectMatview:
                throw new NotImplementedException();

            default:
                throw NotSupportedOption(node.Objtype);
        }
    }

    private static PgTableDefinitionNode VisitCreateObjectTable(CreateTableAsStmt node)
    {
        var createTable = new PgTableDefinitionNode
        {
            IsCreateTableAs = true,
            Relpersistence = node.Into.Rel.Relpersistence.ToRelpersistence(),
            AccessMethod = node.Into.AccessMethod,
            OnCommit = node.Into.OnCommit,
        };

        if (node.Query is { })
            createTable.DataSource.Value = Visit<QsiTableNode>(node.Query);

        if (node.IfNotExists)
            createTable.ConflictBehavior = QsiDefinitionConflictBehavior.Ignore;

        if (node.Into.ColNames is { Count: > 0 } colNames)
            createTable.Columns.Value = CreateSequentialColumnsDeclaration(colNames);

        return createTable;
    }
}
