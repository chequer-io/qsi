using System.Collections.Generic;
using System.Linq;
using Qsi.Data;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public sealed class HanaJsonTableNode : QsiTableNode, IHanaJsonColumnDefinitionNode
    {
        public QsiIdentifier Identifier { get; set; }

        public string Argument { get; set; }

        public QsiQualifiedIdentifier ArgumentColumnReference { get; set; }

        public string Path { get; set; }

        public IHanaJsonColumnDefinitionNode[] Columns { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Columns ?? Enumerable.Empty<IQsiTreeNode>();
    }

    public interface IHanaJsonColumnDefinitionNode : IQsiTreeNode
    {
    }

    public interface IHanaJsonNamedColumnDefinitionNode : IHanaJsonColumnDefinitionNode
    {
        QsiIdentifier Identifier { get; }
    }

    public sealed class HanaOrdinalityJsonColumnDefinitionNode : QsiTreeNode, IHanaJsonNamedColumnDefinitionNode
    {
        public QsiIdentifier Identifier { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }

    public sealed class HanaJsonColumnDefinitionNode : QsiTreeNode, IHanaJsonNamedColumnDefinitionNode
    {
        public QsiIdentifier Identifier { get; set; }

        public string Type { get; set; }

        public bool FormatJson { get; set; }

        // UTF8 | UTF16 | UTF32
        public string Encoding { get; set; }

        public string Path { get; set; }

        public string WrapperBehavior { get; set; }

        public string EmptyBehavior { get; set; }

        public string ErrorBehavior { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }

    public sealed class HanaNestedJsonColumnDefinitionNode : QsiTreeNode, IHanaJsonColumnDefinitionNode
    {
        public string Path { get; set; }

        public IHanaJsonColumnDefinitionNode[] Columns { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Columns ?? Enumerable.Empty<IQsiTreeNode>();
    }
}
