using System.Collections.Generic;
using System.Linq;
using Qsi.Data;

namespace Qsi.Tree
{
    public sealed class QsiVariableAccessExpressionNode : QsiExpressionNode, IQsiVariableAccessExpressionNode, IQsiTerminalNode
    {
        public QsiQualifiedIdentifier Identifier { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }
}
