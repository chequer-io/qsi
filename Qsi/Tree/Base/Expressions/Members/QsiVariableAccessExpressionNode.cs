using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiVariableAccessExpressionNode : QsiExpressionNode, IQsiVariableAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
