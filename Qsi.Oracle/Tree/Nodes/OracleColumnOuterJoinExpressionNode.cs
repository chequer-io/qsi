using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree;

public class OracleColumnOuterJoinExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiColumnReferenceNode> Column { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Column);

    public OracleColumnOuterJoinExpressionNode()
    {
        Column = new QsiTreeNodeProperty<QsiColumnReferenceNode>(this);
    }
}