using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree;

public class OracleNamedParameterExpressionNode : QsiExpressionNode
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

    public OracleNamedParameterExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}