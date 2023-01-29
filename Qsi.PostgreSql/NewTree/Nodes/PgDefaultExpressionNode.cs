using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgDefaultExpressionNode : QsiExpressionNode
{
    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}
