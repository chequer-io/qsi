using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiFunctionAccessExpressionNode : QsiTreeNode, IQsiFunctionAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
