using Qsi.Tree;

namespace Qsi.Cql.Tree
{
    public sealed class CqlDerivedTableNode : QsiDerivedTableNode
    {
        public bool IsJson { get; set; }

        public bool AllowFiltering { get; set; }

        public int? PerPartitionLimit { get; set; }
    }
}
