/* Generated by QSI

 Date: 2020-08-12
 Span: 3191:1 - 3197:11
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("LockStmt")]
    internal class LockStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_LockStmt;

        public IPg10Node[] relations { get; set; }

        public int? mode { get; set; }

        public bool? nowait { get; set; }
    }
}
