using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgInferExpressionNode : QsiExpressionNode
{
    public string Name { get; set; } = string.Empty;

    public QsiTreeNodeList<QsiExpressionNode?> IndexElems { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Where { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(IndexElems, Where);

    public PgInferExpressionNode()
    {
        IndexElems = new QsiTreeNodeList<QsiExpressionNode?>(this);
        Where = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}
