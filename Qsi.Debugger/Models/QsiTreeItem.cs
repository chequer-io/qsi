namespace Qsi.Debugger.Models;

public abstract class QsiTreeItem
{
    public string Header { get; set; }
}

public class QsiPropertyItem : QsiTreeItem
{
    public object Value { get; set; }
}

public class QsiElementItem : QsiTreeItem
{
    public QsiTreeItem[] Items { get; set; }
}

public class QsiChildElementItem : QsiTreeItem
{
    public QsiTreeItem[] Items { get; set; }
}