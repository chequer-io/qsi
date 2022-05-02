using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public class PostgreSqlDataUpdateActionNode : QsiDataUpdateActionNode, IQsiTableNode
{
    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Returning { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var node in base.Children)
            {
                yield return node;
            }

            yield return Returning.Value;
        }
    }

    public PostgreSqlDataUpdateActionNode()
    {
        Returning = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
    }
}
