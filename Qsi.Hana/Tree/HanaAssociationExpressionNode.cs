using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree;

public sealed class HanaAssociationExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeList<HanaAssociationReferenceNode> References { get; }

    public override IEnumerable<IQsiTreeNode> Children => References;

    public HanaAssociationExpressionNode()
    {
        References = new QsiTreeNodeList<HanaAssociationReferenceNode>(this);
    }
}