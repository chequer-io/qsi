using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree;

public class QsiLiteralExpressionNode : QsiExpressionNode, IQsiLiteralExpressionNode, IQsiTerminalNode
{
    public object Value { get; set; }

    public QsiDataType Type { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}