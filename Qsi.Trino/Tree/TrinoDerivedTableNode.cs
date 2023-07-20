using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Trino.Tree;

public sealed class TrinoDerivedTableNode : QsiDerivedTableNode
{
    public TrinoSetQuantifier? SetQuantifier { get; set; }

    public QsiTreeNodeProperty<TrinoWindowExpressionNode> Window { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(base.Children, Window);

    public TrinoDerivedTableNode()
    {
        Window = new QsiTreeNodeProperty<TrinoWindowExpressionNode>(this);
    }
}