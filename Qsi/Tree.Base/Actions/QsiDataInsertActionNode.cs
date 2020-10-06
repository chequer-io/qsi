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

        public QsiTreeNodeList<QsiRowValueExpressionNode> Rows { get; }

        public QsiTreeNodeList<QsiAssignExpressionNode> Elements { get; }

        public QsiDataConflictAction ConflictAction { get; set; }

        public QsiTreeNodeList<QsiDataConflictActionNode> ConflictActions { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Directives, Target, Columns).Concat(Rows).Concat(Elements).Concat(ConflictActions);

        #region Explicit
        IQsiTableDirectivesNode IQsiDataInsertActionNode.Directives => Directives.Value;

        IQsiTableAccessNode IQsiDataInsertActionNode.Target => Target.Value;

        IQsiColumnsDeclarationNode IQsiDataInsertActionNode.Columns => Columns.Value;

        IQsiRowValueExpressionNode[] IQsiDataInsertActionNode.Rows => Rows.Cast<IQsiRowValueExpressionNode>().ToArray();

        IQsiAssignExpressionNode[] IQsiDataInsertActionNode.Elements => Elements.Cast<IQsiAssignExpressionNode>().ToArray();

        IQsiDataConflictActionNode[] IQsiDataInsertActionNode.ConflictActions => ConflictActions.Cast<IQsiDataConflictActionNode>().ToArray();
        #endregion

        public QsiDataInsertActionNode()
        {
            Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
            Target = new QsiTreeNodeProperty<QsiTableAccessNode>(this);
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            Rows = new QsiTreeNodeList<QsiRowValueExpressionNode>(this);
            Elements = new QsiTreeNodeList<QsiAssignExpressionNode>(this);
            ConflictActions = new QsiTreeNodeList<QsiDataConflictActionNode>(this);
        }
    }
}
