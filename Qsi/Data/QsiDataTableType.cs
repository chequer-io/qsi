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
        Join = 1 << 4,
        Union = 1 << 5
    }
}
