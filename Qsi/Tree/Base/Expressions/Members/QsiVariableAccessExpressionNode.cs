using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiVariableAccessExpressionNode : QsiTreeNode, IQsiVariableAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
