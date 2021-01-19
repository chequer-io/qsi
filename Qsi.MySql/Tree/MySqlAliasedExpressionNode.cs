using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.MySql.Tree
{
    public class MySqlAliasedExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Expression.IsEmpty)
                    yield return Expression.Value;

                if (!Alias.IsEmpty)
                    yield return Alias.Value;
            }
        }

        public MySqlAliasedExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
