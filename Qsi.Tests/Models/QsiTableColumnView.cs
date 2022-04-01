using Qsi.Data.Object;

namespace Qsi.Tests.Models;

public sealed class QsiTableColumnView : ReferenceView
{
    public string Parent { get; }

    public string Name { get; }

    public string[] References { get; }

    public QsiObject[] ObjectReferences { get; }

    public bool IsVisible { get; }

    public bool IsBinding { get; }

    public bool IsDynamic { get; }

    public string Default { get; }

    public bool IsExpression { get; }

    public QsiTableColumnView(string refId, string parent, string name, string[] references, QsiObject[] objectReferences, bool isVisible, bool isBinding, bool isDynamic, string @default, bool isExpression) : base(refId)
    {
        Parent = parent;
        Name = name;
        References = references;
        ObjectReferences = objectReferences;
        IsVisible = isVisible;
        IsBinding = isBinding;
        IsDynamic = isDynamic;
        Default = @default;
        IsExpression = isExpression;
    }
}
