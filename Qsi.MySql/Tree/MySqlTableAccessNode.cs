using Qsi.Data;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public sealed class MySqlTableAccessNode : QsiTableAccessNode
    {
        public QsiIdentifier[] Partitions { get; set; }
    }
}
