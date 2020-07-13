using System;
using System.Collections.Generic;

namespace Qsi.Diagnostics
{
    public interface IRawTree
    {
        Type TreeType { get; }
        
        IEnumerable<IRawTree> Children { get; }
    }
}
