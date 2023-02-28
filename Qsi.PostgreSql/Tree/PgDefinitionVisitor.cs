using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.PostgreSql.Extensions;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;

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
            TableElts = { node.TableElts.Select(VisitExpression).WhereNotNull() },
            InheritRelations = { node.InhRelations.Select(VisitExpression).WhereNotNull() },
            Constraints = { node.Constraints.Select(VisitExpression).WhereNotNull() },
            Options = { node.Options.Select(VisitExpression).WhereNotNull() },
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
            IsInherit = node.Into.Rel.Inh,
            Identifier = CreateQualifiedIdentifier(node.Into.Rel)
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
                        return new QsiChangeSearchPathActionNode
                        {
                            Identifiers = node.Args.Select(x =>
                                new QsiQualifiedIdentifier(new QsiIdentifier(x.AConst.Sval.Sval, false))
                            ).ToArray()
                        };
                    }
                }

                break;
            }
        }

        return new PgVariableSetActionNode
        {
            Name = new QsiIdentifier(node.Name, false),
            Arguments = { node.Args.Select(VisitExpression).WhereNotNull() },
            IsLocal = node.IsLocal,
            Kind = node.Kind
        };
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

    public static PgFunctionDefinitionNode Visit(CreateFunctionStmt node)
    {
        return new PgFunctionDefinitionNode(CreateQualifiedIdentifier(node.Funcname))
        {
            Name = CreateQualifiedIdentifier(node.Funcname),
            Replace = node.Replace,
            IsProcedure = node.IsProcedure,
            ReturnType = { Value = Visit(node.ReturnType) },
            Parameters = { node.Parameters.Select(Visit<PgFunctionParameterExpressionNode>).WhereNotNull() },
            Options = { node.Options.Select(Visit<PgDefinitionElementNode>).WhereNotNull() },
            SqlBody = { Value = Visit<QsiTreeNode>(node.SqlBody) }
        };
    }

    public static PgColumnDefinitionNode Visit(ColumnDef node)
    {
        return new PgColumnDefinitionNode
        {
            Name = new QsiIdentifier(node.Colname, false),
            TypeName = { Value = node.TypeName is null ? null : Visit(node.TypeName) },
            RawDefault = { Value = VisitExpression(node.RawDefault) },
            CollClause = { Value = node.CollClause is null ? null : Visit(node.CollClause) },
            Constraints = { node.Constraints.Select(VisitExpression).WhereNotNull() },
            FdwOptions = { node.Fdwoptions.Select(VisitExpression).WhereNotNull() }
        };
    }
}
