﻿namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the column defined in IQsiDerivedTable using the ordinal.
    /// </summary>
    public interface IQsiSequentialColumn : IQsiColumn, IQsiAliased
    {
        int Ordinal { get; }
    }
}
