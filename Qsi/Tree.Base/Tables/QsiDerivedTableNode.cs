using System.Collections.Generic;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public class QsiDerivedTableNode : QsiTableNode, IQsiDerivedTableNode
    {
        public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public QsiTreeNodeProperty<QsiWhereExpressionNode> Where { get; }

        public QsiTreeNodeProperty<QsiGroupingExpressionNode> Grouping { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> Order { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> Limit { get; }

        public override IEnumerable<IQsiTreeNode> Children =>
            TreeHelper.YieldChildren(Directives, Columns, Source, Alias, Where, Grouping, Order, Limit);

        #region Explicit
        IQsiTableDirectivesNode IQsiDerivedTableNode.Directives => Directives.Value;

        IQsiColumnsDeclarationNode IQsiDerivedTableNode.Columns => Columns.Value;

        IQsiTableNode IQsiDerivedTableNode.Source => Source.Value;

        IQsiAliasNode IQsiDerivedTableNode.Alias => Alias.Value;

        IQsiWhereExpressionNode IQsiDerivedTableNode.Where => Where.Value;

        IQsiGroupingExpressionNode IQsiDerivedTableNode.Grouping => Grouping.Value;

        IQsiMultipleOrderExpressionNode IQsiDerivedTableNode.Order => Order.Value;

        IQsiLimitExpressionNode IQsiDerivedTableNode.Limit => Limit.Value;
        #endregion

        public QsiDerivedTableNode()
        {
            Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
            Where = new QsiTreeNodeProperty<QsiWhereExpressionNode>(this);
            Grouping = new QsiTreeNodeProperty<QsiGroupingExpressionNode>(this);
            Order = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            Limit = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}
