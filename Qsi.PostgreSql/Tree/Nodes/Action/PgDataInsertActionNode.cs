using PgQuery;
using Qsi.Tree;

namespace Qsi.PostgreSql.Tree.Nodes;

public class PgDataInsertActionNode : QsiDataInsertActionNode
{
    // NOTE: https://www.postgresql.org/docs/current/sql-insert.html
    //       Parameters > Inserting > alias
    //       It used when `ON CONFLICT DO UPDATE` clause.
    //
    // INSERT INTO distributors AS dist ...
    //                             ^^^^
    public QsiTreeNodeProperty<QsiAliasNode> Alias { get; }

    public QsiTreeNodeProperty<PgOnConflictNode> Conflict { get; }

    public OverridingKind Override { get; set; }

    public PgDataInsertActionNode()
    {
        Alias = new QsiTreeNodeProperty<QsiAliasNode>(this);
        Conflict = new QsiTreeNodeProperty<PgOnConflictNode>(this);
    }
}
