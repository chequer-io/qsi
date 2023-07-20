using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Oracle.Tree;

public sealed class OracleLateralTableNode : QsiTableNode
{
    public QsiTreeNodeProperty<QsiTableNode> Source { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Source.IsEmpty)
                yield return Source.Value;
        }
    }

    public OracleLateralTableNode()
    {
        Source = new QsiTreeNodeProperty<QsiTableNode>(this);
    }
}