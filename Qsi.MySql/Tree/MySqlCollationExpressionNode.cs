using System.Collections.Generic;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.MySql.Tree;

public class MySqlCollationExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

    public QsiIdentifier Collate { get; set; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Expression.IsEmpty)
                yield return Expression.Value;
        }
    }

    public MySqlCollationExpressionNode()
    {
        Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}