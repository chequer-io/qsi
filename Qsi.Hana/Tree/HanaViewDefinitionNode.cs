using Qsi.Tree;
using Qsi.Tree.Definition;

namespace Qsi.Hana.Tree;

public sealed class HanaViewDefinitionNode : QsiViewDefinitionNode
{
    public string Comment { get; set; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Parameters { get; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Associations { get; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Masks { get; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> ExpressionMacros { get; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Annotation { get; }

    public bool StructuredPrivilegeCheck { get; set; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Cache { get; }

    public bool Force { get; set; }

    public bool CheckOption { get; set; }

    public bool DdlOnly { get; set; }

    public bool ReadOnly { get; set; }

    public QsiTreeNodeProperty<QsiExpressionFragmentNode> Anonymization { get; }

    public HanaViewDefinitionNode()
    {
        Parameters = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        Associations = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        Masks = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        ExpressionMacros = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        Annotation = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        Cache = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
        Anonymization = new QsiTreeNodeProperty<QsiExpressionFragmentNode>(this);
    }
}