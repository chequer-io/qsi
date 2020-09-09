using System.Collections.Generic;

namespace Qsi.Tree.Base
{
    public sealed class QsiDerivedTableNode : QsiTableNode, IQsiDerivedTableNode
    {
        public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (!Directives.IsEmpty)
                    yield return Directives.Value;

                if (!Columns.IsEmpty)
                    yield return Columns.Value;

                if (!Source.IsEmpty)
                    yield return Source.Value;

                if (!Alias.IsEmpty)
                    yield return Alias.Value;
            }
        }

        #region Explicit
        IQsiTableDirectivesNode IQsiDerivedTableNode.Directives => Directives.Value;

        IQsiColumnsDeclarationNode IQsiDerivedTableNode.Columns => Columns.Value;

        IQsiTableNode IQsiDerivedTableNode.Source => Source.Value;

        IQsiAliasNode IQsiDerivedTableNode.Alias => Alias.Value;
        #endregion

        public QsiDerivedTableNode()
        {
            Directives = new QsiTreeNodeProperty<QsiTableDirectivesNode>(this);
            Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
            Source = new QsiTreeNodeProperty<QsiTableNode>(this);
            Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        }
    }
}
