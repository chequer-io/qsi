using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;
using Qsi.Utilities;

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

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression, EvalName);

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

        public override IEnumerable<IQsiTreeNode> Children => TreeHelper.YieldChildren(Expression);

        public OracleXmlExpressionNode()
        {
            Expression = new QsiTreeNodeProperty<QsiExpressionNode>(this);
        }
    }

    public sealed class OracleXmlTableFunctionNode : OracleXmlFunctionExpressionNode
    {
        public QsiTreeNodeList<OracleXmlNamespaceNode> Namespaces { get; }

        public QsiTreeNodeList<OracleXmlColumnDefinitionNode> Columns { get; }

        public bool IsReturningSequenceByRef { get; set; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                foreach (var passing in Passings)
                    yield return passing;

                foreach (var OracleNamespace in Namespaces)
                    yield return OracleNamespace;

                foreach (var column in Columns)
                    yield return column;
            }
        }

        public OracleXmlTableFunctionNode()
        {
            Namespaces = new QsiTreeNodeList<OracleXmlNamespaceNode>(this);
            Columns = new QsiTreeNodeList<OracleXmlColumnDefinitionNode>(this);
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

    public sealed class OracleXmlColumnDefinitionNode : QsiColumnExpressionNode
    {
        public string Type { get; }

        public OracleXmlColumnDefinitionNode(string type)
        {
            Type = type;
        }
    }
}
