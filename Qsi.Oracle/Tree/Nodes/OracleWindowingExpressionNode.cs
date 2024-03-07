using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Oracle.Common;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree;

public class OracleWindowingExpressionNode : QsiExpressionNode
{
    public OracleWindowingType Type { get; set; }

    public QsiTreeNodeList<OracleWindowingItemNode> Items { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Exclude { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Items, Exclude);

    public OracleWindowingExpressionNode()
    {
        Items = new QsiTreeNodeList<OracleWindowingItemNode>(this);
        Exclude = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}

public class OracleWindowingItemNode : QsiTreeNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Left { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Right { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Left, Right);

    public OracleWindowingItemNode()
    {
        Left = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Right = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}