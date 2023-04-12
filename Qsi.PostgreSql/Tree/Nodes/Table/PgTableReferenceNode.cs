using Qsi.PostgreSql.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgTableReferenceNode : QsiTableReferenceNode
{
    public Relpersistence Relpersistence { get; set; }
    
    public bool IsInherit { get; set; }
}
