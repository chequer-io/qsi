namespace Qsi.Tree
{
    public class QsiTreeNodeProperty<TNode> : IQsiTreeNodeProperty<TNode>
        where TNode : QsiTreeNode
    {
        public TNode Value
        {
            get => _value;
            set
            {
                value.Parent = _owner;
                _value = value;
            }
        }

        public bool IsEmpty => _value is null;

        private readonly QsiTreeNode _owner;
        private TNode _value;

        public QsiTreeNodeProperty(QsiTreeNode owner)
        {
            _owner = owner;
        }

        public void SetValue(TNode value)
        {
            Value = value;
        }
    }
}
