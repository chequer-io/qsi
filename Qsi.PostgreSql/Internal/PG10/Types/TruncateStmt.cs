/* Generated by QSI

 Date: 2020-08-12
 Span: 2589:1 - 2595:15
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("TruncateStmt")]
    internal class TruncateStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_TruncateStmt;

        public IPg10Node[] relations { get; set; }

        public bool? restart_seqs { get; set; }

        public DropBehavior? behavior { get; set; }
    }
}