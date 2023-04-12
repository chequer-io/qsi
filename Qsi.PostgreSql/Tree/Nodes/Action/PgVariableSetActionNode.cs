using System.Collections.Generic;
using System.Linq;
using PgQuery;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgVariableSetActionNode : QsiActionNode
{
    public QsiIdentifier? Name { get; set; }

    public QsiTreeNodeList<QsiExpressionNode> Arguments { get; }

    public bool IsLocal { get; set; }

    public VariableSetKind Kind { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Arguments.OfType<IQsiTreeNode>();

    public PgVariableSetActionNode()
    {
        Arguments = new QsiTreeNodeList<QsiExpressionNode>(this);
    }
}
