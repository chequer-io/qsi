using System.Collections.Generic;
using Qsi.Utilities;

namespace Qsi.Tree
{
    public sealed class QsiDerivedTableNode : QsiTableNode, IQsiDerivedTableNode
    {
        public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children =>
            TreeHelper.YieldChildren(Directives, Columns, Source, Alias);

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
