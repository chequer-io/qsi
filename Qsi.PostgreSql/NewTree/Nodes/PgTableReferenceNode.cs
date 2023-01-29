using Qsi.PostgreSql.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgTableReferenceNode : QsiTableReferenceNode
{
    public Relpersistence Relpersistence { get; set; }
}
