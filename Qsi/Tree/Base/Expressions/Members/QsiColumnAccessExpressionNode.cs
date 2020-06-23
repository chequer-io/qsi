using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiColumnAccessExpressionNode : QsiTreeNode, IQsiColumnAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
