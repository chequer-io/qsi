namespace Qsi.Tree
{
    public interface IQsiTreeNodeProperty<out TNode> where TNode : QsiTreeNode
    {
        TNode Value { get; }

        bool IsEmpty { get; }
    }
}
