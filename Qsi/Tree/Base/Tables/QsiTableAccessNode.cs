using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiTableAccessNode : QsiTableNode, IQsiTableAccessNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }
    }
}
