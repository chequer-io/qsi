using System.Collections.Generic;
using System.Linq;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiInlineDerivedTableNode : QsiTableNode, IQsiInlineDerivedTableNode
    {
        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public QsiTreeNodeList<QsiRowValueExpressionNode> Rows { get; }

        public override IEnumerable<IQsiTreeNode> Children
            => TreeHelper.YieldChildren(Alias, Columns).Concat(Rows);

        #region Explicit
        IQsiAliasNode IQsiInlineDerivedTableNode.Alias => Alias.Value;

        IQsiColumnsDeclarationNode IQsiInlineDerivedTableNode.Columns => Columns.Value;

        IQsiRowValueExpressionNode[] IQsiInlineDerivedTableNode.Rows => Rows.Cast<IQsiRowValueExpressionNode>().ToArray();
        #endregion

        public QsiInlineDerivedTableNode()
        {
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            Rows = new QsiTreeNodeList<QsiRowValueExpressionNode>(this);
        }
    }
}
