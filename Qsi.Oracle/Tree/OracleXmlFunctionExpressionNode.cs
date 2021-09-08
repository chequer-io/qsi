using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Oracle.Tree
{
    public class OracleXmlFunctionExpressionNode : OracleInvokeExpressionNode
    {
        public QsiTreeNodeList<OracleXmlExpressionNode> Passings { get; }

        public override IEnumerable<IQsiTreeNode> Children => Passings;

        public OracleXmlFunctionExpressionNode()
        {
            Passings = new QsiTreeNodeList<OracleXmlExpressionNode>(this);
        }
    }
    
    public sealed class OracleXmlAttributesExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeList<OracleXmlColumnAttributeItemNode> Attributes { get; }

        public override IEnumerable<IQsiTreeNode> Children => Attributes;

        public OracleXmlAttributesExpressionNode()
        {
            Attributes = new QsiTreeNodeList<OracleXmlColumnAttributeItemNode>(this);
        }
    }

    public sealed class OracleXmlColumnAttributeItemNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public QsiAliasNode Alias { get; set; }
        
        public QsiTreeNodeProperty<QsiExpressionNode> EvalName { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleXmlColumnAttributeItemNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
            EvalName = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }

    public sealed class OracleXmlExpressionNode : QsiExpressionNode
    {
        public QsiTreeNodeProperty<QsiExpressionNode> Expression { get; }

        public QsiAliasNode Alias { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleXmlExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }
    
    public sealed class OracleXmlTableFunctionNode : OracleXmlFunctionExpressionNode
    {
        public OracleXmlNamespaceNode[] Namespaces { get; set; }

        public OracleXmlColumnDefinitionNode[] Columns { get; set; }
        
        public bool IsReturningSequenceByRef { get; set; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (Namespaces != null)
                {
                    foreach (var OracleNamespace in Namespaces)
                        yield return OracleNamespace;
                }

                if (Columns != null)
                {
                    foreach (var column in Columns)
                        yield return column;
                }
            }
        }
    }

    public sealed class OracleXmlNamespaceNode : QsiTreeNode
    {
        public string Url { get; }

        public string Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleXmlNamespaceNode(string url)
        {
            Url = url;
        }

        public OracleXmlNamespaceNode(string url, string alias) : this(url)
        {
            Alias = alias;
        }
    }

    public sealed class OracleXmlColumnDefinitionNode : QsiTreeNode
    {
        public QsiColumnNode Column { get; }

        public string Type { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public OracleXmlColumnDefinitionNode(QsiColumnNode column, string type)
        {
            Column = column;
            Type = type;
        }
    }
}
