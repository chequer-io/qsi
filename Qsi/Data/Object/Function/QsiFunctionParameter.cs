namespace Qsi.Data.Object.Function;

public class QsiFunctionParameter
{
    internal QsiFunctionObject Parent { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public bool IsDefault { get; set; }

    public QsiFunctionParameter()
    {
    }

    public QsiFunctionParameter(bool isDefault)
    {
        Name = string.Empty;
        Type = string.Empty;
        IsDefault = isDefault;
    }

    public QsiFunctionParameter(string name, string type, bool isDefault)
    {
        Name = name;
        Type = type;
        IsDefault = isDefault;
    }
}
