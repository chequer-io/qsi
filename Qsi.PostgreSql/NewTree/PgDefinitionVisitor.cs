using System;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Internal;
using Qsi.PostgreSql.NewTree.Nodes;
using Qsi.PostgreSql.Tree.PG10.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.NewTree;

internal static partial class PgNodeVisitor
{
    // TODO: not all implemented yet (feature/pg-official-parser)
    public static IQsiTreeNode Visit(CreateStmt node)
    {
        var createTable = new PgTableDefinitionNode
        {
            IsCreateTableAs = false,
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

    private static IQsiTreeNode Visit(VariableSetStmt node)
    {
        switch (node.Kind)
        {
            case VariableSetKind.VarSetValue:
            {
                switch (node.Name)
                {
                    case "search_path":
                    {
                        // TODO: IsLocal not used, is it necessary? (feature/pg-official-parser)
                        return new QsiChangeSearchPathActionNode
                        {
                            Identifiers = new[]
                            {
                                new QsiQualifiedIdentifier(node.Args
                                    .Select(x => new QsiIdentifier(x.AConst.Sval.Sval, false)))
                            }
                        };
                    }
                }

                break;
            }
        }

        throw TreeHelper.NotSupportedTree(node);
    }

    private static PgViewDefinitionNode Visit(ViewStmt node)
    {
        var name = node.View;

        var def = new PgViewDefinitionNode
        {
            Identifier = CreateQualifiedIdentifier(name.Catalogname, name.Schemaname, name.Relname),
            Source = { Value = Visit<QsiTableNode>(node.Query) },
            CheckOption = node.WithCheckOption,
            Relpersistence = name.Relpersistence.ToRelpersistence(),
            Options = { node.Options.Select(Visit<PgDefinitionElementNode>) }
        };

        if (node.Replace)
            def.ConflictBehavior = QsiDefinitionConflictBehavior.Replace;

        def.Columns.Value = node.Aliases.Count > 0
            ? CreateSequentialColumnsDeclaration(node.Aliases)
            : TreeHelper.CreateAllColumnsDeclaration();

        return def;
    }
}
