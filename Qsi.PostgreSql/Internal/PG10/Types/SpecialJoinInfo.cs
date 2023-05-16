/* Generated by QSI

 Date: 2020-08-12
 Span: 1929:1 - 1944:18
 File: src/postgres/include/nodes/relation.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("SpecialJoinInfo")]
    internal class SpecialJoinInfo : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_SpecialJoinInfo;

        public Bitmapset[] min_lefthand { get; set; }

        public Bitmapset[] min_righthand { get; set; }

        public Bitmapset[] syn_lefthand { get; set; }

        public Bitmapset[] syn_righthand { get; set; }

        public JoinType? jointype { get; set; }

        public bool? lhs_strict { get; set; }

        public bool? delay_upper_joins { get; set; }

        public bool? semi_can_btree { get; set; }

        public bool? semi_can_hash { get; set; }

        public IPg10Node[] semi_operators { get; set; }

        public IPg10Node[] semi_rhs_exprs { get; set; }
    }
}