using Antlr4.Runtime;
using Qsi.Data;
using Qsi.Oracle.Tree;
using Qsi.Oracle.Tree.Visitors;
using Qsi.Tree;
using Qsi.Utilities;
using static Qsi.Oracle.Internal.OracleParserInternal;

namespace Qsi.Oracle.Utilities;

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

    public static QsiJoinedTableNode CreateJoinedTable(QsiTableNode left, QsiTableNode right, QsiExpressionNode pivotExpression)
    {
        var node = new QsiJoinedTableNode();
        node.Left.Value = left;
        node.Right.Value = right;
        node.JoinType = "JOIN";
        node.PivotExpression.Value = pivotExpression;

        return node;
    }

    public static OracleNamedParameterExpressionNode CreateNamedParameter(ParserRuleContext context, string name)
    {
        var node = OracleTree.CreateWithSpan<OracleNamedParameterExpressionNode>(context);
        node.Identifier = new QsiQualifiedIdentifier(new QsiIdentifier(name, false));

        return node;
    }
}