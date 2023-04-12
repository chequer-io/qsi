using System.Collections.Generic;
using System.Linq;
using Qsi.PostgreSql.Extensions;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgRoutineTableNode : QsiCompositeTableNode
{
    public bool Ordinality { get; set; }

    public bool Lateral { get; set; }

    public bool IsRowsfrom { get; set; }

    public QsiTreeNodeList<PgColumnDefinitionNode?> ColumnDefinitions { get; }

    public override IEnumerable<IQsiTreeNode> Children => Sources.Cast<IQsiTreeNode>()
        .ConcatWhereNotNull(ColumnDefinitions);

    public PgRoutineTableNode()
    {
        ColumnDefinitions = new QsiTreeNodeList<PgColumnDefinitionNode?>(this);
    }
}
