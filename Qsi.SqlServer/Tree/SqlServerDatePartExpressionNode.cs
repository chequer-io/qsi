using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.SqlServer.Tree;

public class SqlServerDatePartExpressionNode : QsiExpressionNode
{
    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

    public string DatePart { get; set; }
}
