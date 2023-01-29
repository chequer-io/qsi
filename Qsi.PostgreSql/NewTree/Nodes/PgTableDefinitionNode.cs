using PgQuery;
using Qsi.PostgreSql.Data;
using Qsi.Tree.Definition;

namespace Qsi.PostgreSql.NewTree.Nodes;

public class PgTableDefinitionNode : QsiTableDefinitionNode
{
    public bool IsCreateTableAs { get; set; }

    public Relpersistence Relpersistence { get; set; }

    public string? AccessMethod { get; set; }

    public OnCommitAction OnCommit { get; set; }

    #region [SETOF] TYPE_NAME
    public bool IsSetOf { get; set; }

    public string? TypeName { get; set; }
    #endregion
}
