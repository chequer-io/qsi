namespace Qsi.Tree.Base
{
    public class QsiTreeNodeProperty<TNode> where TNode : QsiTreeNode
    {
        public TNode Value => _value;

        public bool IsEmpty => _value == null;

        private TNode _value;
        private readonly QsiTreeNode _owner;

        public QsiTreeNodeProperty(QsiTreeNode owner)
        {
            _owner = owner;
        }

        public void SetValue(TNode value)
        {
            value.Parent = _owner;
            _value = value;
        }
    }
}
