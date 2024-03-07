using System;

namespace Qsi.Parsing.Common;

public sealed class CommonScriptCursor
{
    public char Current => Value[Index];

    public char Next => HasNext ? Value[Index + 1] : default;

    public bool HasNext => Index + 1 < Length;

    public int Index { get; set; }

    public int Length { get; internal set; }

    public string Value { get; internal set; }

    public ReadOnlySpan<char> ValueSpan => Value;

    public CommonScriptCursor(in string value)
    {
        Value = value;
        Length = Value.Length;
        Index = 0;
    }

    public void Reset(string value)
    {
        Value = value;
        Length = Value.Length;
        Index = 0;
    }

    public bool StartsWith(ReadOnlySpan<char> value)
    {
        return StartsWith(value, Index);
    }

    public bool StartsWith(ReadOnlySpan<char> value, int start)
    {
        return Value.AsSpan(start).StartsWith(value);
    }

    public bool StartsWithIgnoreCase(in ReadOnlySpan<char> value)
    {
        return Value.AsSpan(Index).StartsWith(value, StringComparison.OrdinalIgnoreCase);
    }
}