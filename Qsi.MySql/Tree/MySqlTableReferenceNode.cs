using Qsi.Data;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public sealed class MySqlTableReferenceNode : QsiTableReferenceNode
    {
        public QsiIdentifier[] Partitions { get; set; }
    }
}
