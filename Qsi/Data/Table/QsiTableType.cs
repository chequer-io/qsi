using System;

namespace Qsi.Data
{
    [Flags]
    public enum QsiTableType
    {
        Table = 1 << 0,
        View = 1 << 1,
        MaterializedView = 1 << 2,
        Prepared = 1 << 3,

        Derived = 1 << 4,
        Inline = 1 << 5,
        Join = 1 << 6,
        Union = 1 << 7
    }
}
