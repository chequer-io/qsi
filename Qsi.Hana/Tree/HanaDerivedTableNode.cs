using System.Collections.Generic;
using Qsi.Tree;
using Qsi.Tree.Data;
using Qsi.Utilities;

namespace Qsi.Hana.Tree
{
    public interface IHanaDerivedTableNode : IQsiDerivedTableNode
    {
        QsiExpressionNode Top { get; }

        HanaResultSetOperation? Operation { get; }

        QsiExpressionFragmentNode Sampling { get; }

        HanaTableBehaviorNode Behavior { get; }

        QsiExpressionFragmentNode TimeTravel { get; }

        QsiExpressionFragmentNode Hint { get; }
    }

    public sealed class HanaDerivedTableNode : QsiDerivedTableNode, IHanaDerivedTableNode
    {
        // TOP 123
        public QsiTreeNodeProperty<QsiExpressionNode> Top { get; }

        // DISTINCT | ALL
        public HanaResultSetOperation? Operation { get; set; }

        // TABLESAMPLE [BERNOULLI | SYSTEM] (123)
        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Sampling { get; }

        // FOR ...
        public QsiTreeNodeProperty<HanaTableBehaviorNode> Behavior { get; }

        // AS OF COMMIT ID 123
        // AS OF UTCTIMESTAMP '123'
        public QsiTreeNodeProperty<QsiExpressionFragmentNode> TimeTravel { get; }

        // WITH HINT (ROUTE_TO(1, 2), ...)
        public QsiTreeNodeProperty<QsiExpressionFragmentNode> Hint { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                foreach (var child in base.Children)
                    yield return child;

                if (!Behavior.IsEmpty)
                    yield return Behavior.Value;
            }
        }

        #region Explicit
        QsiExpressionNode IHanaDerivedTableNode.Top => Top.Value;

        QsiExpressionFragmentNode IHanaDerivedTableNode.Sampling => Sampling.Value;

        HanaTableBehaviorNode IHanaDerivedTableNode.Behavior => Behavior.Value;

        QsiExpressionFragmentNode IHanaDerivedTableNode.TimeTravel => TimeTravel.Value;

        QsiExpressionFragmentNode IHanaDerivedTableNode.Hint => Hint.Value;
        #endregion

        public HanaDerivedTableNode()
        {
            Top = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            Sampling = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            Behavior = new QsiTreeNodeProperty<HanaTableBehaviorNode>(this);
            TimeTravel = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
            Hint = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        }
    }

    public readonly struct ImmutableHanaDerivedTableNode : IHanaDerivedTableNode
    {
        public IQsiTreeNode Parent { get; }

        public IQsiTableDirectivesNode Directives { get; }

        public IQsiColumnsDeclarationNode Columns { get; }

        public IQsiTableNode Source { get; }

        public IQsiAliasNode Alias { get; }

        public IQsiWhereExpressionNode Where { get; }

        public IQsiGroupingExpressionNode Grouping { get; }

        public IQsiMultipleOrderExpressionNode Order { get; }

        public IQsiLimitExpressionNode Limit { get; }

        public QsiExpressionNode Top { get; }

        public HanaResultSetOperation? Operation { get; }

        public QsiExpressionFragmentNode Sampling { get; }

        public HanaTableBehaviorNode Behavior { get; }

        public QsiExpressionFragmentNode TimeTravel { get; }

        public QsiExpressionFragmentNode Hint { get; }

        public IUserDataHolder UserData { get; }

        public IEnumerable<IQsiTreeNode> Children =>
            TreeHelper.YieldChildren(Directives, Columns, Source, Alias, Where, Grouping, Order, Limit);

        public ImmutableHanaDerivedTableNode(
            IQsiTreeNode parent,
            IQsiTableDirectivesNode directives,
            IQsiColumnsDeclarationNode columns,
            IQsiTableNode source,
            IQsiAliasNode alias,
            IQsiWhereExpressionNode @where,
            IQsiGroupingExpressionNode grouping,
            IQsiMultipleOrderExpressionNode order,
            IQsiLimitExpressionNode limit,
            QsiExpressionNode top,
            HanaResultSetOperation? operation,
            QsiExpressionFragmentNode sampling,
            HanaTableBehaviorNode behavior,
            QsiExpressionFragmentNode timeTravel,
            QsiExpressionFragmentNode hint,
            IUserDataHolder userData)
        {
            Parent = parent;
            Directives = directives;
            Columns = columns;
            Source = source;
            Alias = alias;
            Where = @where;
            Grouping = grouping;
            Order = order;
            Limit = limit;
            Top = top;
            Operation = operation;
            Sampling = sampling;
            Behavior = behavior;
            TimeTravel = timeTravel;
            Hint = hint;
            UserData = userData;
        }
    }
}
