/* Generated by QSI

 Date: 2020-08-12
 Span: 1690:1 - 1697:17
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("AlterTableStmt")]
    internal class AlterTableStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_AlterTableStmt;

        public RangeVar[] relation { get; set; }

        public IPg10Node[] cmds { get; set; }

        public ObjectType? relkind { get; set; }

        public bool? missing_ok { get; set; }
    }
}