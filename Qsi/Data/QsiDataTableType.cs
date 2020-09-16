using System;

namespace Qsi.Data
{
    [Flags]
    public enum QsiDataTableType
    {
        Table = 1 << 0,
        View = 1 << 1,
        MaterializedView = 1 << 2,

        Derived = 1 << 3,
        Inline = 1 << 4,
        Join = 1 << 5,
        Union = 1 << 6
    }
}
