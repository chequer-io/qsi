using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgCommonTableNode : QsiDerivedTableNode
{
    public CTEMaterialize Materialized { get; set; }
}
