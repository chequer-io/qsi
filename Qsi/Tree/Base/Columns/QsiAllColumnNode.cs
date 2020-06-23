using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiAllColumnNode : QsiColumnNode, IQsiAllColumnNode
    {
        public QsiQualifiedIdentifier Path { get; set; }
    }
}
