namespace Qsi.Tests.Models;

public abstract class ReferenceView
{
    public string RefId { get; }

    protected ReferenceView(string refId)
    {
        RefId = refId;
    }
}
