using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree;

public class OracleSetColumnsExpressionNode : QsiExpressionNode
{
    public QsiQualifiedIdentifier[] Targets { get; set; }

    public QsiTreeNodeProperty<QsiTableNode> Value { get; }

    #region Explicit
    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Value);
    #endregion

    public OracleSetColumnsExpressionNode()
    {
        Value = new QsiTreeNodeProperty<QsiTableNode>(this);
    }
}