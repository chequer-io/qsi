using Qsi.Data;
using Qsi.Oracle.Tree;
using Qsi.Oracle.Tree.Visitors;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Oracle.Internal.OracleParserInternal;

namespace Qsi.Oracle.Utilities
{
    internal static class OracleHelper
    {
        public static QsiDerivedTableNode CreateDerivedTableWithPath(QsiTableNode source, QsiQualifiedIdentifier path)
        {
            var node = new OracleDerivedTableNode();
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

        public static QsiDerivedTableNode CreateDerivedTable(QsiTableNode source, TAliasContext alias)
        {
            return CreateDerivedTable(source, IdentifierVisitor.VisitAlias(alias));
        }

        public static OracleJoinedTableNode CreateJoinedTable(QsiTableNode left, QsiTableNode right, QsiExpressionNode onCondition)
        {
            var node = new OracleJoinedTableNode();
            node.Left.SetValue(left);
            node.Right.SetValue(right);
            node.JoinType = "JOIN";
            node.OnCondition.Value = onCondition;

            return node;
        }
    }
}
