using System.Collections.Generic;
using Qsi.Tree;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgTypeExpressionNode : QsiTypeExpressionNode
{
    public bool Setof { get; set; }

    public bool PctType { get; set; }

    public QsiTreeNodeList<QsiExpressionNode> TypMods { get; }

    public QsiTreeNodeList<QsiExpressionNode> ArrayBounds { get; }

    public override IEnumerable<IQsiTreeNode> Children
    {
        get
        {
            foreach (var baseChild in base.Children)
                yield return baseChild;

            foreach (var typeMod in TypMods)
                yield return typeMod;

            foreach (var arrayBound in ArrayBounds)
                yield return arrayBound;
        }
    }

    public PgTypeExpressionNode()
    {
        TypMods = new QsiTreeNodeList<QsiExpressionNode>(this);
        ArrayBounds = new QsiTreeNodeList<QsiExpressionNode>(this);
    }
}
