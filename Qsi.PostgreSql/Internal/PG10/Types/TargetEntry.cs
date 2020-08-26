/* Generated by QSI

 Date: 2020-08-12
 Span: 1365:1 - 1377:14
 File: src/postgres/include/nodes/primnodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("TargetEntry")]
    internal class TargetEntry : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_TargetEntry;

        public IPg10Node xpr { get; set; }

        public IPg10Node[] expr { get; set; }

        public short? resno { get; set; }

        public string resname { get; set; }

        public uint? ressortgroupref { get; set; }

        public uint? resorigtbl { get; set; }

        public short? resorigcol { get; set; }

        public bool? resjunk { get; set; }
    }
}