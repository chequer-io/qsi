using Qsi.Tree;

namespace Qsi.Impala.Tree;

public class ImpalaWhereExpressionNode : QsiWhereExpressionNode
{
    public string PlanHints { get; set; }
}