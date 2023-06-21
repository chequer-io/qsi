using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree;

public class OracleInvokeExpressionNode : QsiInvokeExpressionNode
{
    public OracleQueryBehavior? QueryBehavior { get; set; }
}