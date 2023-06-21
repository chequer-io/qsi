using Qsi.Tree.Definition;

namespace Qsi.SqlServer.Tree;

public sealed class SqlServerViewDefinitionNode : QsiViewDefinitionNode
{
    public bool IsAlter { get; set; }

    public bool WithCheckOption { get; set; }

    public bool IsMaterialiazed { get; set; }

    public string[] ViewOptions { get; set; }
}