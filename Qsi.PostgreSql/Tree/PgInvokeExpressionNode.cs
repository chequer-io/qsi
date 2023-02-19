using System.Diagnostics.CodeAnalysis;
using PgQuery;
using Qsi.PostgreSql.Tree.Nodes;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public class PgInvokeExpressionNode : QsiInvokeExpressionNode
{
    public bool IsBuiltIn { get; set; }

    public CoercionForm FunctionFormat { get; set; } = CoercionForm.Undefined;

    public bool AggregateStar { get; set; }

    public bool AggregateDistinct { get; set; }

    public bool AggregateWithInGroup { get; set; }

    public bool FunctionVariadic { get; set; }

    public QsiTreeNodeList<QsiExpressionNode?> AggregateOrder { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> AggregateFilter { get; }

    public QsiTreeNodeProperty<PgWindowDefExpressionNode> Over { get; }

    public PgInvokeExpressionNode()
    {
        AggregateOrder = new QsiTreeNodeList<QsiExpressionNode?>(this);
        AggregateFilter = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Over = new QsiTreeNodeProperty<PgWindowDefExpressionNode>(this);
    }
}
