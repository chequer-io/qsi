using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgTableFunctionNode : QsiTableNode, IQsiTableFunctionNode
{
    public IQsiInvokeExpressionNode Function { get; }

    public IQsiFunctionExpressionNode Member => Function.Member;

    public IQsiParametersExpressionNode Parameters => Function.Parameters;

    public override IEnumerable<IQsiTreeNode> Children => Function.Children;

    public PgTableFunctionNode(IQsiInvokeExpressionNode function)
    {
        Function = function;
    }
}
