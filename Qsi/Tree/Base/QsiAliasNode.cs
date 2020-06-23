using Qsi.Data;

namespace Qsi.Tree.Base
{
    public sealed class QsiAliasNode : QsiTreeNode, IQsiAliasNode
    {
        public QsiIdentifier Name { get; set; }
    }
}
