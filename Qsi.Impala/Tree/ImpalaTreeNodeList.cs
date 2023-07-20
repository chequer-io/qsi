using Qsi.Tree;

namespace Qsi.Impala.Tree;

public class ImpalaTreeNodeList<TNode> : QsiTreeNodeList<TNode> where TNode : QsiTreeNode
{
    public string Option { get; set; }

    public ImpalaTreeNodeList(QsiTreeNode owner) : base(owner)
    {
    }
}