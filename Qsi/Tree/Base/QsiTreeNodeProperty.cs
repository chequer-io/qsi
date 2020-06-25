namespace Qsi.Tree.Base
{
    public class QsiTreeNodeProperty<TNode> where TNode : QsiTreeNode
    {
        public bool IsEmpty => _value == null;

        private TNode _value;
        private readonly QsiTreeNode _parent;

        public QsiTreeNodeProperty(QsiTreeNode parent)
        {
            _parent = parent;
        }

        public void SetValue(TNode value)
        {
            value.Parent = _parent;
            _value = value;
        }

        public TNode GetValue()
        {
            return _value;
        }
    }
}
