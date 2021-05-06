using System.Collections.Generic;
using System.Linq;
using Qsi.Tree;

namespace Qsi.Hana.Tree
{
    public abstract class HanaTableBehaviorNode : QsiTreeNode
    {
    }

    public sealed class HanaTableShareLockBehaviorNode : HanaTableBehaviorNode
    {
        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }

    public sealed class HanaTableUpdateBehaviorNode : HanaTableBehaviorNode
    {
        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }

    public sealed class HanaTableSerializeBehaviorNode : HanaTableBehaviorNode
    {
        public HanaTableSerializeType Type { get; set; }

        public Dictionary<string, string> Options { get; } = new();

        public string ReturnType { get; set; }

        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }

    public sealed class HanaTableSystemTimeBehaviorNode : HanaTableBehaviorNode
    {
        public override IEnumerable<IQsiTreeNode> Children => Enumerable.Empty<IQsiTreeNode>();
    }

    public enum HanaTableSerializeType
    {
        Json,
        Xml
    }
}
