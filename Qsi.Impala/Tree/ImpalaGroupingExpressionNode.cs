using Qsi.Impala.Common;
using Qsi.Tree;

namespace Qsi.Impala.Tree;

public class ImpalaGroupingExpressionNode : QsiGroupingExpressionNode
{
    public ImpalaGroupingSetsType GroupingSetsType { get; set; }
}