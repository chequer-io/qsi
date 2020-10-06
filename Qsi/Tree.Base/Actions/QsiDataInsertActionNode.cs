using System.Linq;
using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiDataInsertActionNode : QsiActionNode, IQsiDataInsertActionNode
    {
        public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

        public QsiTreeNodeProperty<QsiTableAccessNode> Target { get; }

        public QsiQualifiedIdentifier[] Partitions { get; set; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public QsiTreeNodeList<QsiRowValueExpressionNode> Values { get; }

        public QsiTreeNodeList<QsiAssignExpressionNode> SetValues { get; }

        public QsiTreeNodeProperty<QsiTableNode> ValueTable { get; }

        public QsiDataConflictBehavior ConflictBehavior { get; set; }

        public QsiTreeNodeProperty<QsiDataConflictActionNode> ConflictAction { get; }

        public override IEnumerable<IQsiTreeNode> Children =>
            TreeHelper.YieldChildren(Directives, Target, Columns)
                .Concat(Values)
                .Concat(SetValues)
                .Concat(TreeHelper.YieldChildren(ValueTable, ConflictAction));

        #region Explicit
        IQsiTableDirectivesNode IQsiDataInsertActionNode.Directives => Directives.Value;

        IQsiTableAccessNode IQsiDataInsertActionNode.Target => Target.Value;

        IQsiColumnsDeclarationNode IQsiDataInsertActionNode.Columns => Columns.Value;

        IQsiRowValueExpressionNode[] IQsiDataInsertActionNode.Values => Values.Cast<IQsiRowValueExpressionNode>().ToArray();

        IQsiAssignExpressionNode[] IQsiDataInsertActionNode.SetValues => SetValues.Cast<IQsiAssignExpressionNode>().ToArray();

        IQsiTableNode IQsiDataInsertActionNode.ValueTable => ValueTable.Value;

        IQsiDataConflictActionNode IQsiDataInsertActionNode.ConflictAction => ConflictAction.Value;
        #endregion

        public QsiDataInsertActionNode()
        {
            Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
            Target = new QsiTreeNodeProperty<QsiTableAccessNode>(this);
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            Values = new QsiTreeNodeList<QsiRowValueExpressionNode>(this);
            SetValues = new QsiTreeNodeList<QsiAssignExpressionNode>(this);
            ValueTable = new QsiTreeNodeProperty<QsiTableNode>(this);
            ConflictAction = new QsiTreeNodeProperty<QsiDataConflictActionNode>(this);
        }
    }
}
