namespace Qsi.Data;

public enum QsiDefinitionConflictBehavior
{
    /// <summary>
    /// CREATE ...
    /// </summary>
    None,

    /// <summary>
    /// CREATE IF NOT EXISTS ...
    /// </summary>
    Ignore,

    /// <summary>
    /// CREATE OR REPLACE ...
    /// </summary>
    Replace
}