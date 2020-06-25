using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiFunctionAccessExpressionNode : QsiExpressionNode, IQsiFunctionAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
