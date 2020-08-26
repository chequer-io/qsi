using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiTypeAccessExpressionNode : QsiExpressionNode, IQsiMemberAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
