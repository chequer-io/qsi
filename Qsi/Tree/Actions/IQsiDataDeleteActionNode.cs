using System;
using Qsi.Data;

namespace Qsi.Tree
{
    public interface IQsiDataDeleteActionNode : IQsiActionNode
    {
        IQsiTableNode Target { get; }

        [Obsolete]
        QsiQualifiedIdentifier[] Columns { get; }
    }
}
