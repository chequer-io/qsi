using Qsi.Tree;

namespace Qsi.PrimarSql.Tree
{
    public sealed class PrimarSqlDerivedTableNode : QsiDerivedTableNode
    {
        public SelectSpec SelectSpec { get; set; }
    }
}
