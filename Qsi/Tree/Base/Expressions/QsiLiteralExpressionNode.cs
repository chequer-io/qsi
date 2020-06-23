using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiLiteralExpressionNode : QsiExpressionNode, IQsiLiteralExpressionNode
    {
        public object Value { get; set; }

        public QsiLiteralType Type { get; set; }
    }
}
