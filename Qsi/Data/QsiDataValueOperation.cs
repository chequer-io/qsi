using System;

namespace Qsi.Data
{
    [Flags]
    public enum QsiDataValueOperation
    {
        None = 0,
        Insert = 1,
        Duplicate = 2,
        Update = 4,
        Delete = 8
    }
}
