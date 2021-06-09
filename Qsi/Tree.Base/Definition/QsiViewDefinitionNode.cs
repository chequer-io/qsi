using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Definition
{
    public class QsiViewDefinitionNode : QsiTreeNode, IQsiViewDefinitionNode
    {
        public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

        public QsiDefinitionConflictBehavior ConflictBehavior { get; set; }

        public QsiQualifiedIdentifier Identifier { get; set; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Directives, Columns, Source);

        #region Explicit
        IQsiTableDirectivesNode IQsiViewDefinitionNode.Directives => Directives.Value;

        IQsiColumnsDeclarationNode IQsiViewDefinitionNode.Columns => Columns.Value;

        IQsiTableNode IQsiViewDefinitionNode.Source => Source.Value;
        #endregion

        public QsiViewDefinitionNode()
        {
            Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
        }
    }
}
