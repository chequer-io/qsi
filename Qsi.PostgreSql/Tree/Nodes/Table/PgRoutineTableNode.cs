using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgRoutineTableNode : QsiCompositeTableNode
{
    public bool Ordinality { get; set; }

    public bool Lateral { get; set; }

    public bool IsRowsfrom { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}
