namespace Qsi.Tree.Base
{
    public sealed class QsiDerivedTableNode : QsiTableNode, IQsiDerivedTableNode
    {
        public QsiTreeNodeProperty<QsiTableDirectivesNode> Directives { get; }

        public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

        public QsiTreeNodeProperty<QsiTableNode> Source { get; }

        public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

        #region Explicit
        IQsiTableDirectivesNode IQsiDerivedTableNode.Directives => Directives.GetValue();

        IQsiColumnsDeclarationNode IQsiDerivedTableNode.Columns => Columns.GetValue();

        IQsiTableNode IQsiDerivedTableNode.Source => Source.GetValue();

        IQsiAliasNode IQsiDerivedTableNode.Alias => Alias.GetValue();
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
