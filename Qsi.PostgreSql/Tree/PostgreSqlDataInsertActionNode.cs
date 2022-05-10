using System.Collections.Generic;
using Qsi.PostgreSql.Data;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree;

public class PostgreSqlDataInsertActionNode : QsiDataInsertActionNode, IQsiTableNode
{
    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Returning { get; }
    
    public OverridingOption OverridingOption { get; set; }
    
    public bool IsDefaultValues { get; set; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var node in base.Children)
            {
                yield return node;
            }

            if (!Returning.IsEmpty)
            {
                yield return Returning.Value;    
            }
        }
    }

    public PostgreSqlDataInsertActionNode()
    {
        Returning = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
    }
}
