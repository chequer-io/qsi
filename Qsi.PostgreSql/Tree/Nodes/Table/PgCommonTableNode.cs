using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgCommonTableNode : QsiDerivedTableNode
{
    public CTEMaterialize Materialized { get; set; }
}
