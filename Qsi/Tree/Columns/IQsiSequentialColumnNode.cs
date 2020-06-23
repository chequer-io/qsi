namespace Qsi.Tree
{
    /// <summary>
    /// Specifies the column defined in IQsiDerivedTable using the ordinal.
    /// </summary>
    public interface IQsiSequentialColumnNode : IQsiColumnNode
    {
        /// <summary>
        /// Get the column ordinal.
        /// </summary>
        int Ordinal { get; }

        /// <summary>
        /// Get the declared alias.
        /// </summary>
        IQsiAliasNode Alias { get; }
    }
}
