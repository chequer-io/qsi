/* Generated by QSI

 Date: 2020-08-12
 Span: 2049:1 - 2060:13
 File: src/postgres/include/nodes/execnodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("LimitState")]
    internal class LimitState : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_LimitState;

        public PlanState ps { get; set; }

        public ExprState[] limitOffset { get; set; }

        public ExprState[] limitCount { get; set; }

        public int? offset { get; set; }

        public int? count { get; set; }

        public bool? noCount { get; set; }

        public LimitStateCond? lstate { get; set; }

        public int? position { get; set; }

        public TupleTableSlot[] subSlot { get; set; }
    }
}