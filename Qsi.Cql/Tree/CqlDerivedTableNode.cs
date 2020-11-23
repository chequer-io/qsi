using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDerivedTableNode : QsiDerivedTableNode
    {
        public bool IsJson { get; set; }

        public bool AllowFiltering { get; set; }

        public QsiExpressionNode PerPartitionLimit
        {
            get => _perPartitionLimit;
            set
            {
                if (value != null)
                    value.Parent = this;

                _perPartitionLimit = value;
            }
        }

        private QsiExpressionNode _perPartitionLimit;
    }
}
