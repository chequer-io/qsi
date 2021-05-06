using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaLateralTableNode : QsiTableNode, IQsiDerivedTableNode
    {
        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Source.IsEmpty)
                    yield return Source.Value;

                if (!Alias.IsEmpty)
                    yield return Alias.Value;
            }
        }

        #region Explicit
        IQsiTableDirectivesNode IQsiDerivedTableNode.Directives => null;

        IQsiColumnsDeclarationNode IQsiDerivedTableNode.Columns => null;

        IQsiTableNode IQsiDerivedTableNode.Source => Source.Value;

        IQsiAliasNode IQsiDerivedTableNode.Alias => Alias.Value;

        IQsiWhereExpressionNode IQsiDerivedTableNode.Where => null;

        IQsiGroupingExpressionNode IQsiDerivedTableNode.Grouping => null;

        IQsiMultipleOrderExpressionNode IQsiDerivedTableNode.Order => null;

        IQsiLimitExpressionNode IQsiDerivedTableNode.Limit => null;
        #endregion

        public HanaLateralTableNode()
        {
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
