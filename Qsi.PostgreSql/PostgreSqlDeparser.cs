using PgQuery;
using Qsi.Data;
using Qsi.Parsing;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;

namespace Qsi.PostgreSql;

public partial class PostgreSqlDeparser : IQsiTreeDeparser
{
    public string Deparse(IQsiTreeNode node)
    {
        var result = Visitor.Visit(node);

        switch (node)
        {
            case PgMultipleAssignExpressionNode multipleAssignExpression:
                return DeparseMultipleAssignExpression(multipleAssignExpression);
        }

        return Parser.Deparse(result);
    }

    // (c1, c2, c3, c4) = (select * from t)
    //                     ---------------
    private string DeparseMultipleAssignExpression(PgMultipleAssignExpressionNode node)
    {
        return Deparse(node.Value.Value);
    }

    internal Node ConvertToPgNode(IQsiTreeNode node)
    {
        return Visitor.Visit(node);
    }

    #region IQsiTreeDeparser
    string IQsiTreeDeparser.Deparse(IQsiTreeNode node, QsiScript script) => Deparse(node);
    #endregion
}