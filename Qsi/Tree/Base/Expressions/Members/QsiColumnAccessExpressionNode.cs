using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiColumnAccessExpressionNode : QsiExpressionNode, IQsiColumnAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }

        public bool IsAll { get; set; }
    }
}
