using System;

namespace Qsi.Parsing.Common
{
    public sealed class CommonScriptCursor
    {
        public char Current => Value[Index];

        public char Next => HasNext ? Value[Index + 1] : default;

        public bool HasNext => Index + 1 < Length;

        public int Index { get; set; }

        public int Length { get; }

        public string Value { get; }

        public CommonScriptCursor(in string value)
        {
            Value = value;
            Length = Value.Length;
            Index = 0;
        }

        public bool StartsWith(in string value)
        {
            return StartsWith(value, Index);
        }

        public bool StartsWith(in ReadOnlySpan<char> value, int start)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (Index + i >= Length || Value[start + i] != value[i])
                    return false;
            }

            return true;
        }

        public bool StartsWithIgnoreCase(in ReadOnlySpan<char> value)
        {
            return Value.AsSpan(Index).StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
