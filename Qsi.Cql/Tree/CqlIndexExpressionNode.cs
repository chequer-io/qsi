using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Cql.Tree;

public sealed class CqlIndexExpressionNode : QsiExpressionNode, IQsiMemberExpressionNode, IQsiTerminalNode
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
}