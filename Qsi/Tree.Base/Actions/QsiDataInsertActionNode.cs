using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree;

public class QsiDataInsertActionNode : QsiActionNode, IQsiDataInsertActionNode
{
    public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

    public QsiTreeNodeProperty<QsiTableReferenceNode> Target { get; }

    public QsiQualifiedIdentifier[] Partitions { get; set; }

    public QsiQualifiedIdentifier[] Columns { get; set; }

    public QsiTreeNodeList<QsiRowValueExpressionNode> Values { get; }

    public QsiTreeNodeList<QsiSetColumnExpressionNode> SetValues { get; }

    public QsiTreeNodeProperty<QsiTableNode> ValueTable { get; }
    
    public QsiIdentifier FileValue { get; set; }

    public QsiDataConflictBehavior ConflictBehavior { get; set; }

    public QsiTreeNodeProperty<QsiDataConflictActionNode> ConflictAction { get; }

    public override IEnumerable<IQsiTreeNode> Children =>
        TreeHelper.YieldChildren(Directives, Target)
            .Concat(Values)
            .Concat(SetValues)
            .Concat(TreeHelper.YieldChildren(ValueTable, ConflictAction));

    #region Explicit
    IQsiTableDirectivesNode IQsiDataInsertActionNode.Directives => Directives.Value;

    IQsiTableReferenceNode IQsiDataInsertActionNode.Target => Target.Value;

    IQsiRowValueExpressionNode[] IQsiDataInsertActionNode.Values => Values.Cast<IQsiRowValueExpressionNode>().ToArray();

    IQsiSetColumnExpressionNode[] IQsiDataInsertActionNode.SetValues => SetValues.Cast<IQsiSetColumnExpressionNode>().ToArray();

    IQsiTableNode IQsiDataInsertActionNode.ValueTable => ValueTable.Value;

    IQsiDataConflictActionNode IQsiDataInsertActionNode.ConflictAction => ConflictAction.Value;
    #endregion

    public QsiDataInsertActionNode()
    {
        Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
        Target = new QsiTreeNodeProperty<QsiTableReferenceNode>(this);
        Values = new QsiTreeNodeList<QsiRowValueExpressionNode>(this);
        SetValues = new QsiTreeNodeList<QsiSetColumnExpressionNode>(this);
        ValueTable = new QsiTreeNodeProperty<QsiTableNode>(this);
        ConflictAction = new QsiTreeNodeProperty<QsiDataConflictActionNode>(this);
    }
}