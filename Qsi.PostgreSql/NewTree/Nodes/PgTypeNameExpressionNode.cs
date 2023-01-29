using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgTypeNameExpressionNode : QsiExpressionNode
{
    public override IEnumerable<IQsiTreeNode> Children { get; }
}
