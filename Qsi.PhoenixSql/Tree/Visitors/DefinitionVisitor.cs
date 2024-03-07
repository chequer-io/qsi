using System.Linq;
using PhoenixSql;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PhoenixSql.Tree;

internal static class DefinitionVisitor
{
    public static IQsiDefinitionNode VisitCreateViewStatement(CreateTableStatement statement)
    {
        var node = new PViewDefinitionNode
        {
            Identifier = IdentifierVisitor.Visit(statement.TableName)
        };

        if (statement.ColumnDefs.Any())
        {
            node.DynamicColumns = new QsiColumnsDeclarationNode();
            node.DynamicColumns.Columns.AddRange(statement.ColumnDefs.Select(TableVisitor.VisitDynamicColumn));
        }

        QsiTableNode tableNode = TreeHelper.Create<QsiTableReferenceNode>(n =>
        {
            n.Identifier = IdentifierVisitor.Visit(statement.BaseTableName);
            PTree.RawNode[n] = statement.BaseTableName;
        });

        if (statement.WhereClause != null)
        {
            var derivedTableNode = new QsiDerivedTableNode();
            derivedTableNode.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();
            derivedTableNode.Where.Value = ExpressionVisitor.VisitWhere(statement.WhereClause);
            derivedTableNode.Source.Value = tableNode;

            node.Source.Value = derivedTableNode;
        }
        else
        {
            node.Source.Value = tableNode;
        }

        PTree.RawNode[node] = statement;

        return node;
    }
}