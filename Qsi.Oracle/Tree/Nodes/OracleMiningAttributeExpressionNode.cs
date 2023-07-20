using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;
using Qsi.Utilities;

namespace Qsi.Oracle.Tree;

public class OracleMiningAttributeExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Columns);

    public OracleMiningAttributeExpressionNode()
    {
        Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
    }
}