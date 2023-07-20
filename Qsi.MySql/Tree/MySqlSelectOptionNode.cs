using System.Collections.Generic;
using Qsi.MySql.Data;
using Qsi.Tree;

namespace Qsi.MySql.Tree;

public sealed class MySqlSelectOptionNode : QsiTreeNode
{
    public MySqlSelectOption Option { get; set; }

    public QsiTreeNodeProperty<QsiLiteralExpressionNode> MaxStatementTime { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!MaxStatementTime.IsEmpty)
                yield return MaxStatementTime.Value;
        }
    }

    public MySqlSelectOptionNode()
    {
        MaxStatementTime = new QsiTreeNodeProperty<QsiLiteralExpressionNode>(this);
    }
}