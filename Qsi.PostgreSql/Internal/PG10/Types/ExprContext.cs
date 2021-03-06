/* Generated by QSI

 Date: 2020-08-12
 Span: 192:1 - 229:14
 File: src/postgres/include/nodes/execnodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("ExprContext")]
    internal class ExprContext : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_ExprContext;

        public TupleTableSlot[] ecxt_scantuple { get; set; }

        public TupleTableSlot[] ecxt_innertuple { get; set; }

        public TupleTableSlot[] ecxt_outertuple { get; set; }

        public MemoryContext ecxt_per_query_memory { get; set; }

        public MemoryContext ecxt_per_tuple_memory { get; set; }

        public ParamExecData[] ecxt_param_exec_vals { get; set; }

        public ParamListInfoData[] ecxt_param_list_info { get; set; }

        public uint[] ecxt_aggvalues { get; set; }

        public bool[] ecxt_aggnulls { get; set; }

        public uint? caseValue_datum { get; set; }

        public bool? caseValue_isNull { get; set; }

        public uint? domainValue_datum { get; set; }

        public bool? domainValue_isNull { get; set; }

        public EState[] ecxt_estate { get; set; }

        public ExprContext_CB[] ecxt_callbacks { get; set; }
    }
}
