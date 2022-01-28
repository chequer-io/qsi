using System.Text.RegularExpressions;

namespace Qsi.Athena.Common;

public readonly struct AthenaString
{
    public string Value { get; }

    public char UnicodeEscapeChar { get; }

    public AthenaString(string value, char unicodeEscapeChar)
    {
        Value = value;
        UnicodeEscapeChar = unicodeEscapeChar;
    }

    public override string ToString()
    {
        var pattern = new Regex($@"{Regex.Escape(UnicodeEscapeChar.ToString())}(\d+)");
        return pattern.Replace(Value, match => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value)));
    }
}
