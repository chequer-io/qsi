using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Impala.Tree;

public class ImpalaInvokeExpressionNode: QsiExpressionNode, IQsiInvokeExpressionNode
{
    public QsiTreeNodeProperty<QsiFunctionExpressionNode> Member { get; }

    public ImpalaTreeNodeList<QsiExpressionNode> Parameters { get; }

    public override IEnumerable<IQsiTreeNode> Children
        => TreeHelper.YieldChildren(Member?.Value, _parameters);

    #region Explicit
    IQsiFunctionExpressionNode IQsiInvokeExpressionNode.Member => Member.Value;

    IQsiParametersExpressionNode IQsiInvokeExpressionNode.Parameters => _parameters;
    #endregion

    private readonly ImpalaParametersExpressionNode _parameters;

    public ImpalaInvokeExpressionNode()
    {
        Member = new QsiTreeNodeProperty<QsiFunctionExpressionNode>(this);

        _parameters = new ImpalaParametersExpressionNode
        {
            Parent = this
        };

        Parameters = _parameters.Expressions;
    }
}