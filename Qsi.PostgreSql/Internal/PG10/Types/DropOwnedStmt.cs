/* Generated by QSI

 Date: 2020-08-12
 Span: 3319:1 - 3324:16
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("DropOwnedStmt")]
    internal class DropOwnedStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_DropOwnedStmt;

        public IPg10Node[] roles { get; set; }

        public DropBehavior? behavior { get; set; }
    }
}
