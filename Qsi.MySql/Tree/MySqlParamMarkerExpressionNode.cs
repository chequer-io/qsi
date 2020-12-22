using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public class MySqlParamMarkerExpressionNode : QsiExpressionNode
    {
        public override IEnumerable<IQsiTreeNode> Children { get; } = Enumerable.Empty<IQsiTreeNode>();
    }
}
