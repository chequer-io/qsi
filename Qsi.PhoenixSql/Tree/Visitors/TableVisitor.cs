using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using PhoenixSql;
using PhoenixSql.Extensions;
using Qsi.Data;
using Qsi.Shared.Extensions;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PhoenixSql.Tree;

internal static class TableVisitor
{
    public static QsiTableNode VisitSelectStatement(SelectStatement statement)
    {
        var tableNode = new PDerivedTableNode();
        var columnsNode = new QsiColumnsDeclarationNode();

        tableNode.Hints = statement.Hint?.Hints;

        RepeatedField<AliasedNode> selects = statement.Select;

        if (statement.IsUnion && selects.Select(s => s.Node.Unwrap()).TryCast(out IEnumerable<ColumnParseNode> columns))
        {
            columnsNode.Columns.AddRange(CreateSequentialColumns(columns));
        }
        else
        {
            columnsNode.Columns.AddRange(selects.Select(VisitAliasedNode));
        }

        tableNode.Columns.SetValue(columnsNode);

        if (statement.IsUnion)
        {
            tableNode.Source.SetValue(VisitSelectStatements(statement.Selects));
        }
        else if (statement.From != null)
        {
            tableNode.Source.SetValue(VisitTableNode(statement.From));
        }

        if (statement.Where != null)
            tableNode.Where.SetValue(ExpressionVisitor.VisitWhere(statement.Where));

        if (statement.Limit != null || statement.Offset != null)
            tableNode.Limit.SetValue(ExpressionVisitor.VisitLimitOffset(statement.Limit, statement.Offset));

        if (statement.OrderBy.Any())
            tableNode.Order.SetValue(ExpressionVisitor.VisitOrderBy(statement.OrderBy));

        // statement.GroupBy
        // statement.Having

        PTree.RawNode[tableNode] = statement;

        return tableNode;
    }

    public static IEnumerable<QsiSequentialColumnNode> CreateSequentialColumns(IEnumerable<ColumnParseNode> columns)
    {
        return columns.Select(c =>
        {
            var node = new QsiSequentialColumnNode();

            node.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.Visit(c)[0]
            });

            PTree.RawNode[node] = c;

            return node;
        });
    }

    public static QsiTableNode VisitSelectStatements(IEnumerable<SelectStatement> statements)
    {
        return TreeHelper.Create<QsiCompositeTableNode>(n =>
        {
            n.Sources.AddRange(statements.Select(VisitSelectStatement));
            n.CompositeType = "UNION";
        });
    }

    public static QsiColumnNode VisitAliasedNode(AliasedNode node)
    {
        var childNode = node.Node.UnwrapAs<IParseNode>();
        bool hasAlias = !string.IsNullOrEmpty(node.Alias);
        QsiColumnNode columnNode = null;
        QsiExpressionNode expressionNode = null;
        string expressionInfferedName = null;

        switch (childNode.Unwrap())
        {
            case INamedParseNode namedNode:
                if (namedNode.UnwrapAs<INamedParseNode>() is BindParseNode bindParseNode)
                {
                    expressionNode = ExpressionVisitor.VisitBind(bindParseNode);
                    expressionInfferedName = bindParseNode.Name;
                }
                else
                {
                    columnNode = VisitNamedParseNode(namedNode);
                }

                break;

            case WildcardParseNode wildcardParseNode:
                columnNode = VisitWildcardParseNode(wildcardParseNode);
                break;

            default:
                expressionNode = ExpressionVisitor.Visit(childNode);
                break;
        }

        if (columnNode != null && !hasAlias)
            return columnNode;

        return TreeHelper.Create<QsiDerivedColumnNode>(n =>
        {
            if (hasAlias)
            {
                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = IdentifierVisitor.Visit(node)
                });
            }

            if (columnNode != null)
                n.Column.SetValue(columnNode);

            if (expressionNode != null)
                n.Expression.SetValue(expressionNode);

            n.InferredName = new QsiIdentifier(expressionInfferedName, false);

            PTree.RawNode[n] = node;
        });
    }

    public static QsiColumnNode VisitNamedParseNode(INamedParseNode node)
    {
        switch (node.UnwrapAs<INamedParseNode>())
        {
            case FamilyWildcardParseNode familyWildcardParseNode:
                return VisitFamilyWildcardParseNode(familyWildcardParseNode);

            // case BindParseNode bindParseNode:

            case ColumnParseNode columnParseNode:
                return VisitColumnParseNode(columnParseNode);

            case TableWildcardParseNode tableWildcardParseNode:
                return VisitTableWildcardParseNode(tableWildcardParseNode);

            default:
                throw TreeHelper.NotSupportedTree(node);
        }
    }

    public static QsiColumnNode VisitWildcardParseNode(WildcardParseNode node)
    {
        var columnNode = new QsiAllColumnNode();
        PTree.RawNode[columnNode] = node;
        return columnNode;
    }

    public static QsiColumnNode VisitFamilyWildcardParseNode(FamilyWildcardParseNode node)
    {
        var columnNode = new QsiAllColumnNode();

        if (!string.IsNullOrEmpty(node.Name))
            columnNode.Path = new QsiQualifiedIdentifier(IdentifierVisitor.Visit(node));

        PTree.RawNode[columnNode] = node;

        return columnNode;
    }

    public static QsiColumnNode VisitColumnParseNode(ColumnParseNode node)
    {
        var columnNode = new QsiColumnReferenceNode
        {
            Name = IdentifierVisitor.Visit(node)
        };

        PTree.RawNode[columnNode] = node;

        return columnNode;
    }

    public static QsiColumnNode VisitTableWildcardParseNode(TableWildcardParseNode node)
    {
        var columnNode = new QsiAllColumnNode();

        if (!string.IsNullOrEmpty(node.Name))
            columnNode.Path = new QsiQualifiedIdentifier(IdentifierVisitor.Visit(node));

        PTree.RawNode[columnNode] = node;

        return columnNode;
    }

    public static QsiTableNode VisitTableNode(ITableNode node)
    {
        switch (node.Unwrap())
        {
            case NamedTableNode namedTableNode:
                return VisitNamedTableNode(namedTableNode);

            case BindTableNode bindTableNode:
                return VisitBindTableNode(bindTableNode);

            case JoinTableNode joinTableNode:
                return VisitJoinTableNode(joinTableNode);

            case DerivedTableNode derivedTableNode:
                return VisitDerivedTableNode(derivedTableNode);

            default:
                throw TreeHelper.NotSupportedTree(node);
        }
    }

    public static QsiTableNode VisitNamedTableNode(NamedTableNode node)
    {
        var identifier = IdentifierVisitor.Visit(node.Name);

        QsiTableReferenceNode tableNode;

        if (node.DynamicColumns.Any())
        {
            tableNode = TreeHelper.Create<PDynamicTableReferenceNode>(n =>
            {
                n.Identifier = identifier;
                n.DynamicColumns = new QsiColumnsDeclarationNode();
                n.DynamicColumns.Columns.AddRange(node.DynamicColumns.Select(VisitDynamicColumn));
            });
        }
        else
        {
            tableNode = new QsiTableReferenceNode
            {
                Identifier = identifier
            };
        }

        if (string.IsNullOrEmpty(node.Alias))
        {
            PTree.RawNode[tableNode] = node;
            return tableNode;
        }

        return TreeHelper.Create<QsiDerivedTableNode>(n =>
        {
            n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
            n.Source.SetValue(tableNode);

            n.Alias.SetValue(new QsiAliasNode
            {
                Name = IdentifierVisitor.Visit(node)
            });

            PTree.RawNode[n] = node;
        });
    }

    public static PDynamicColumnReferenceNode VisitDynamicColumn(ColumnDef node)
    {
        return TreeHelper.Create<PDynamicColumnReferenceNode>(n =>
        {
            n.Name = IdentifierVisitor.Visit(node.ColumnDefName);
            PTree.RawNode[n] = node;
        });
    }

    public static QsiTableNode VisitBindTableNode(BindTableNode node)
    {
        throw TreeHelper.NotSupportedFeature("Table binding");
    }

    public static QsiTableNode VisitJoinTableNode(JoinTableNode node)
    {
        var tableNode = new QsiJoinedTableNode();

        tableNode.Left.SetValue(VisitTableNode(node.LHS));
        tableNode.Right.SetValue(VisitTableNode(node.RHS));

        if (node.OnNode == null)
        {
            tableNode.IsComma = true;
        }
        else
        {
            tableNode.JoinType = $"{node.Type.ToString().ToUpper()} JOIN";
        }

        PTree.RawNode[tableNode] = node;

        return tableNode;
    }

    public static QsiTableNode VisitDerivedTableNode(DerivedTableNode node)
    {
        return TreeHelper.Create<QsiDerivedTableNode>(n =>
        {
            n.Columns.SetValue(TreeHelper.CreateAllColumnsDeclaration());
            n.Source.SetValue(VisitSelectStatement(node.Select));

            if (!string.IsNullOrEmpty(node.Alias))
            {
                n.Alias.SetValue(new QsiAliasNode
                {
                    Name = IdentifierVisitor.Visit(node)
                });
            }

            PTree.RawNode[n] = node;
        });
    }
}