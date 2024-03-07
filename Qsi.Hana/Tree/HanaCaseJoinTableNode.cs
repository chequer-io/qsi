using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.Hana.Tree;

public sealed class HanaCaseJoinTableNode : QsiTableNode, IQsiDerivedTableNode
{
    public QsiTreeNodeProperty<QsiTableNode> Source { get; }

    public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

    public QsiTreeNodeList<HanaCaseJoinWhenTableNode> WhenSources { get; }

    public QsiTreeNodeProperty<HanaCaseJoinElseTableNode> ElseSource { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Source.IsEmpty)
                yield return Source.Value;

            if (!Alias.IsEmpty)
                yield return Alias.Value;

            foreach (var source in WhenSources)
                yield return source;

            if (!ElseSource.IsEmpty)
                yield return ElseSource.Value;
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

    public HanaCaseJoinTableNode()
    {
        Source = new QsiTreeNodeProperty<QsiTableNode>(this);
        Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        WhenSources = new QsiTreeNodeList<HanaCaseJoinWhenTableNode>(this);
        ElseSource = new QsiTreeNodeProperty<HanaCaseJoinElseTableNode>(this);
    }
}

public abstract class HanaCaseJoinItemTableNode : QsiTableNode
{
    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

    public QsiTreeNodeProperty<QsiTableNode> Source { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> Predicate { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Columns.IsEmpty)
                yield return Columns.Value;

            if (!Source.IsEmpty)
                yield return Source.Value;

            if (!Predicate.IsEmpty)
                yield return Predicate.Value;
        }
    }

    protected HanaCaseJoinItemTableNode()
    {
        Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
        Source = new QsiTreeNodeProperty<QsiTableNode>(this);
        Predicate = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}

public sealed class HanaCaseJoinWhenTableNode : HanaCaseJoinItemTableNode
{
    public QsiTreeNodeProperty<QsiExpressionNode> Condition { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            if (!Condition.IsEmpty)
                yield return Condition.Value;

            foreach (var child in base.Children)
                yield return child;
        }
    }

    public HanaCaseJoinWhenTableNode()
    {
        Condition = new QsiTreeNodeProperty<QsiExpressionNode>(this);
    }
}

public sealed class HanaCaseJoinElseTableNode : HanaCaseJoinItemTableNode
{
}