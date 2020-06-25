using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiUnknownMemberAcessExpressionNode : QsiExpressionNode, IQsiMemberAccessExpressionNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
