using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PrimarSql.Tree;

public class PrimarSqlStartKeyExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> HashKey { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> SortKey { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!HashKey.IsEmpty)
                yield return HashKey.Value;

            if (!SortKey.IsEmpty)
                yield return SortKey.Value;
        }
    }

    public PrimarSqlStartKeyExpressionNode()
    {
        HashKey = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        SortKey = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}