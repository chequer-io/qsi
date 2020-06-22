﻿namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the column defined in IQsiDerivedTable using the ordinal.
    /// </summary>
    public interface IQsiSequentialColumn : IQsiAliasedColumn
    {
        int Ordinal { get; }
    }
}
