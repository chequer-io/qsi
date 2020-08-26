/* Generated by QSI

 Date: 2020-08-12
 Span: 1484:1 - 1500:17
 File: src/postgres/include/nodes/primnodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("OnConflictExpr")]
    internal class OnConflictExpr : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_OnConflictExpr;

        public OnConflictAction? action { get; set; }

        public IPg10Node[] arbiterElems { get; set; }

        public IPg10Node[] arbiterWhere { get; set; }

        public uint? constraint { get; set; }

        public IPg10Node[] onConflictSet { get; set; }

        public IPg10Node[] onConflictWhere { get; set; }

        public int? exclRelIndex { get; set; }

        public IPg10Node[] exclRelTlist { get; set; }
    }
}