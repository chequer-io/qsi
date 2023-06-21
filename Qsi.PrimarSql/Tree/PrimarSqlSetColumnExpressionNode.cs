using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PrimarSql.Tree;

public sealed class PrimarSqlSetColumnExpressionNode : QsiSetColumnExpressionNode
{
    public QsiTreeNodeList<QsiExpressionNode> Accessors { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var node in Accessors)
                yield return node;

            if (!Value.IsEmpty)
                yield return Value.Value;
        }
    }

    public PrimarSqlSetColumnExpressionNode()
    {
        Accessors = new QsiTreeNodeList<QsiExpressionNode>(this);
    }
}