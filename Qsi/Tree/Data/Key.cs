namespace Qsi.Tree.Data;

public sealed class Key<T>
{
    public string Text { get; }

    public Key(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return $"Key: {Text}";
    }
}