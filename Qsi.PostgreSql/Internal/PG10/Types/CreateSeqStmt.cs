/* Generated by QSI

 Date: 2020-08-12
 Span: 2467:1 - 2475:16
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("CreateSeqStmt")]
    internal class CreateSeqStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_CreateSeqStmt;

        public RangeVar[] sequence { get; set; }

        public IPg10Node[] options { get; set; }

        public uint? ownerId { get; set; }

        public bool? for_identity { get; set; }

        public bool? if_not_exists { get; set; }
    }
}
