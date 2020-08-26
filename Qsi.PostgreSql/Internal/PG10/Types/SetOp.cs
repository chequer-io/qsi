/* Generated by QSI

 Date: 2020-08-12
 Span: 884:1 - 896:8
 File: src/postgres/include/nodes/plannodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("SetOp")]
    internal class SetOp : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_SetOp;

        public Plan plan { get; set; }

        public SetOpCmd? cmd { get; set; }

        public SetOpStrategy? strategy { get; set; }

        public int? numCols { get; set; }

        public short[] dupColIdx { get; set; }

        public uint[] dupOperators { get; set; }

        public short? flagColIdx { get; set; }

        public int? firstFlag { get; set; }

        public int? numGroups { get; set; }
    }
}