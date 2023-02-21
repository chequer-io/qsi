using System;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree;

internal static partial class PgNodeVisitor
{
    public static PgTableDefinitionNode Visit(CreateStmt node)
    {
        // partbound, partspec ignored
        return new PgTableDefinitionNode
        {
            Identifier = CreateQualifiedIdentifier(node.Relation),
            IsCreateTableAs = false,
            Relpersistence = node.Relation.Relpersistence.ToRelpersistence(),
            AccessMethod = node.AccessMethod,
            TableElts = { node.TableElts.Select(VisitExpression) },
            InheritRelations = { node.InhRelations.Select(VisitExpression) },
            Constraints = { node.Constraints.Select(VisitExpression) },
            Options = { node.Options.Select(VisitExpression) },
            OfType = { Value = node.OfTypename is null ? null : Visit(node.OfTypename) },
            ConflictBehavior = node.IfNotExists ? QsiDefinitionConflictBehavior.Ignore : QsiDefinitionConflictBehavior.None,
            OnCommit = node.Oncommit,
            TablespaceName = node.Tablespacename
        };
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

        if (node.Query is { } query)
            createTable.DataSource.Value = Visit<QsiTableNode>(query);

        if (node.IfNotExists)
            createTable.ConflictBehavior = QsiDefinitionConflictBehavior.Ignore;

        createTable.Columns.Value = CreateAliasedColumnsDeclaration(node.Into.ColNames);

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
                    case PgKnownVariable.SearchPath:
                    {
                        // TODO: IsLocal not used, is it necessary? (feature/pg-official-parser)
                        return new QsiChangeSearchPathActionNode
                        {
                            Identifiers = new[]
                            {
                                new QsiQualifiedIdentifier(
                                    node.Args
                                        .Select(x => new QsiIdentifier(x.AConst.Sval.Sval, false))
                                )
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
        var def = new PgViewDefinitionNode
        {
            Identifier = CreateQualifiedIdentifier(node.View),
            Source = { Value = Visit<QsiTableNode>(node.Query) },
            CheckOption = node.WithCheckOption,
            Relpersistence = node.View.Relpersistence.ToRelpersistence(),
            Options = { node.Options.Select(Visit<PgDefinitionElementNode>) }
        };

        if (node.Replace)
            def.ConflictBehavior = QsiDefinitionConflictBehavior.Replace;

        def.Columns.Value = CreateAliasedColumnsDeclaration(node.Aliases);

        return def;
    }
}
