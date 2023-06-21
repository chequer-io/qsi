using System.Collections.Generic;
using Qsi.Data;
using Qsi.Utilities;

namespace Qsi.Tree.Definition;

public class QsiTableDefinitionNode : QsiTreeNode, IQsiTableDefinitionNode
{
    public QsiQualifiedIdentifier Identifier { get; set; }

    public QsiDefinitionConflictBehavior ConflictBehavior { get; set; }

    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

    public QsiTreeNodeProperty<QsiTableNode> ColumnSource { get; }

    public QsiTreeNodeProperty<QsiTableNode> DataSource { get; }

    public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Columns, ColumnSource, DataSource);

    #region Explicit
    IQsiColumnsDeclarationNode IQsiTableDefinitionNode.Columns => Columns.Value;

    IQsiTableNode IQsiTableDefinitionNode.ColumnSource => ColumnSource.Value;

    IQsiTableNode IQsiTableDefinitionNode.DataSource => DataSource.Value;
    #endregion

    public QsiTableDefinitionNode()
    {
        Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
        ColumnSource = new QsiTreeNodeProperty<QsiTableNode>(this);
        DataSource = new QsiTreeNodeProperty<QsiTableNode>(this);
    }
}