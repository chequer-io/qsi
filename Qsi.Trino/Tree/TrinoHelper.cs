using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree
{
    public class TrinoHelper
    {
        public static QsiDerivedTableNode CreateDerivedTableWithPath(QsiTableNode source, QsiQualifiedIdentifier path)
        {
            var node = new QsiDerivedTableNode();
            node.Source.Value = source;

            var columns = new QsiColumnsDeclarationNode();

            columns.Columns.Add(new QsiAllColumnNode
            {
                Path = path
            });

            node.Columns.Value = columns;

            return node;
        }

        public static QsiDerivedTableNode CreateDerivedTable(QsiTableNode source, QsiAliasNode alias = null)
        {
            var node = new QsiDerivedTableNode();
            node.Source.Value = source;
            node.Columns.Value = TreeHelper.CreateAllColumnsDeclaration();

            if (alias is not null)
                node.Alias.Value = alias;

            return node;
        }

        public static TrinoJoinedTableNode CreateJoinedTable(QsiTableNode left, QsiTableNode right, QsiExpressionNode onCondition)
        {
            var node = new TrinoJoinedTableNode();
            node.Left.Value = left;
            node.Right.Value = right;
            node.JoinType = "JOIN";
            node.OnCondition.Value = onCondition;

            return node;
        }
    }
}
