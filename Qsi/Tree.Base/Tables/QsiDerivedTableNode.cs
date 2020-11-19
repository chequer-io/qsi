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

        public QsiTreeNodeProperty<QsiWhereExpressionNode> WhereExpression { get; }

        public QsiTreeNodeProperty<QsiGroupingExpressionNode> GroupingExpression { get; }

        public QsiTreeNodeProperty<QsiMultipleOrderExpressionNode> OrderExpression { get; }

        public QsiTreeNodeProperty<QsiLimitExpressionNode> LimitExpression { get; }

        public override IEnumerable<IQsiTreeNode> Children =>
            TreeHelper.YieldChildren(Directives, Columns, Source, Alias, WhereExpression, GroupingExpression, OrderExpression, LimitExpression);

        #region Explicit
        IQsiTableDirectivesNode IQsiDerivedTableNode.Directives => Directives.Value;

        IQsiColumnsDeclarationNode IQsiDerivedTableNode.Columns => Columns.Value;

        IQsiTableNode IQsiDerivedTableNode.Source => Source.Value;

        IQsiAliasNode IQsiDerivedTableNode.Alias => Alias.Value;

        IQsiWhereExpressionNode IQsiDerivedTableNode.WhereExpression => WhereExpression.Value;

        IQsiGroupingExpressionNode IQsiDerivedTableNode.GroupingExpression => GroupingExpression.Value;

        IQsiMultipleOrderExpressionNode IQsiDerivedTableNode.OrderExpression => OrderExpression.Value;

        IQsiLimitExpressionNode IQsiDerivedTableNode.LimitExpression => LimitExpression.Value;
        #endregion

        public QsiDerivedTableNode()
        {
            Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
            WhereExpression = new QsiTreeNodeProperty<QsiWhereExpressionNode>(this);
            GroupingExpression = new QsiTreeNodeProperty<QsiGroupingExpressionNode>(this);
            OrderExpression = new QsiTreeNodeProperty<QsiMultipleOrderExpressionNode>(this);
            LimitExpression = new QsiTreeNodeProperty<QsiLimitExpressionNode>(this);
        }
    }
}
