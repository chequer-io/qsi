using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiDeclaredColumnNode : QsiColumnNode, IQsiDeclaredColumnNode
    {
        public QsiQualifiedIdentifier Name { get; set; }
    }
}
