﻿namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the column defined in IQsiDerivedTable using the ordinal.
    /// </summary>
    public interface IQsiSequentialColumnNode : IQsiColumnNode, IQsiAliasedNode
    {
        int Ordinal { get; }
    }
}
