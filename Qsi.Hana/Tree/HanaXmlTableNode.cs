using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaXmlTableNode : QsiTableNode
    {
        public QsiIdentifier Identifier { get; set; }

        public HanaXmlNamespaceNode DefaultNamespace { get; set; }

        public HanaXmlNamespaceNode[] Namespaces { get; set; }

        public string RowPattern { get; set; }

        public string Argument { get; set; }

        public HanaXmlColumnDefinitionNode[] Columns { get; set; }

        public override IEnumerable<IQsiTreeNode> Children
        {
            get
            {
                if (DefaultNamespace != null)
                    yield return DefaultNamespace;

                if (Namespaces != null)
                {
                    foreach (var hanaNamespace in Namespaces)
                        yield return hanaNamespace;
                }

                if (Columns != null)
                {
                    foreach (var column in Columns)
                        yield return column;
                }
            }
        }
    }

    public sealed class HanaXmlNamespaceNode : QsiTreeNode
    {
        public string Url { get; }

        public string Alias { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public HanaXmlNamespaceNode(string url)
        {
            Url = url;
        }

        public HanaXmlNamespaceNode(string url, string alias) : this(url)
        {
            Alias = alias;
        }
    }

    public sealed class HanaXmlColumnDefinitionNode : QsiTreeNode
    {
        public QsiIdentifier Identifier { get; }

        public string Type { get; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();

        public HanaXmlColumnDefinitionNode(QsiIdentifier identifier, string type)
        {
            Identifier = identifier;
            Type = type;
        }
    }
}
