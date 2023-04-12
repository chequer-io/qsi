using System.Collections.Generic;
using System.Linq;
using Qsi.PostgreSql.Extensions;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

// XMLTABLE ( <rowexpr> PASSING <docexpr> COLUMNS <xml_col_list> )
// XMLTABLE ( XMLNAMESPACES ( <xml_namespaces> ), <rowexpr> PASSING <docexpr> COLUMNS <xml_col_list> )
public class PgXmlTableNode : QsiTableNode
{
    public bool Lateral { get; set; }

    public QsiTreeNodeProperty<QsiExpressionNode> RowExpr { get; }

    public QsiTreeNodeProperty<QsiExpressionNode> DocExpr { get; }

    public QsiTreeNodeList<QsiExpressionNode> Namespaces { get; }

    public QsiTreeNodeProperty<QsiColumnsDeclarationNode> Columns { get; }

    public override IEnumerable<IQsiTreeNode> Children =>
        Namespaces.OfType<IQsiTreeNode>()
            .Append(RowExpr.Value)
            .Append(DocExpr.Value)
            .Append(Columns.Value)
            .WhereNotNull();

    public PgXmlTableNode()
    {
        RowExpr = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        DocExpr = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        Namespaces = new QsiTreeNodeList<QsiExpressionNode>(this);
        Columns = new QsiTreeNodeProperty<QsiColumnsDeclarationNode>(this);
    }
}
