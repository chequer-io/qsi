using System.Collections.Generic;
using Qsi.Oracle.Common;
using Qsi.Tree;

namespace Qsi.Oracle.Tree;

public class OracleJsonColumnExpressionNode : QsiExpressionNode
{
    public QsiTreeNodeList<OracleJsonColumnNode> Columns { get; }

    public override IEnumerable<IQsiTreeNode> Children => Columns;

    public OracleJsonColumnExpressionNode()
    {
        Columns = new QsiTreeNodeList<OracleJsonColumnNode>(this);
    }
}

public class OracleJsonColumnNode : QsiDerivedColumnNode
{
    public string ReturnType { get; set; }

    public string JsonPath { get; set; }

    public string OnErrorBehavior { get; set; }

    public object OnErrorDefault { get; set; }

    public string OnEmptyBehavior { get; set; }

    public object OnEmptyDefault { get; set; }

    public bool IsTruncate { get; set; }
        
    public bool IsFormatted { get; set; }
        
    public bool IsOrdinality { get; set; }
        
    public QsiTreeNodeProperty<OracleJsonColumnExpressionNode> NestedColumn { get; }

    public OracleJsonColumnNode()
    {
        NestedColumn = new QsiTreeNodeProperty<OracleJsonColumnExpressionNode>(this);
    }
}