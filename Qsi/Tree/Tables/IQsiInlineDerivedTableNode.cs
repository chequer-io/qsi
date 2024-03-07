namespace Qsi.Tree;

public interface IQsiInlineDerivedTableNode : IQsiTableNode
{
    /// <summary>
    /// Get the delcared alias.
    /// </summary>
    IQsiAliasNode Alias { get; }

    /// <summary>
    /// Get the columns declaration.
    /// </summary>
    IQsiColumnsDeclarationNode Columns { get; }

    /// <summary>
    /// Get the rows declaration.
    /// </summary>
    IQsiRowValueExpressionNode[] Rows { get; }
}