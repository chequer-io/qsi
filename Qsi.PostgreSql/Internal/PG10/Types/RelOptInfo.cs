/* Generated by QSI

 Date: 2020-08-12
 Span: 518:1 - 595:13
 File: src/postgres/include/nodes/relation.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("RelOptInfo")]
    internal class RelOptInfo : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_RelOptInfo;

        public RelOptKind? reloptkind { get; set; }

        public Bitmapset[] relids { get; set; }

        public double? rows { get; set; }

        public bool? consider_startup { get; set; }

        public bool? consider_param_startup { get; set; }

        public bool? consider_parallel { get; set; }

        public PathTarget[] reltarget { get; set; }

        public IPg10Node[] pathlist { get; set; }

        public IPg10Node[] ppilist { get; set; }

        public IPg10Node[] partial_pathlist { get; set; }

        public Path[] cheapest_startup_path { get; set; }

        public Path[] cheapest_total_path { get; set; }

        public Path[] cheapest_unique_path { get; set; }

        public IPg10Node[] cheapest_parameterized_paths { get; set; }

        public Bitmapset[] direct_lateral_relids { get; set; }

        public Bitmapset[] lateral_relids { get; set; }

        public uint? relid { get; set; }

        public uint? reltablespace { get; set; }

        public RTEKind? rtekind { get; set; }

        public short? min_attr { get; set; }

        public short? max_attr { get; set; }

        public Bitmapset[][] attr_needed { get; set; }

        public int[] attr_widths { get; set; }

        public IPg10Node[] lateral_vars { get; set; }

        public Bitmapset[] lateral_referencers { get; set; }

        public IPg10Node[] indexlist { get; set; }

        public IPg10Node[] statlist { get; set; }

        public uint? pages { get; set; }

        public double? tuples { get; set; }

        public double? allvisfrac { get; set; }

        public PlannerInfo[] subroot { get; set; }

        public IPg10Node[] subplan_params { get; set; }

        public int? rel_parallel_workers { get; set; }

        public uint? serverid { get; set; }

        public uint? userid { get; set; }

        public bool? useridiscurrent { get; set; }

        public FdwRoutine[] fdwroutine { get; set; }

        public object[] fdw_private { get; set; }

        public IPg10Node[] unique_for_rels { get; set; }

        public IPg10Node[] non_unique_for_rels { get; set; }

        public IPg10Node[] baserestrictinfo { get; set; }

        public QualCost baserestrictcost { get; set; }

        public uint? baserestrict_min_security { get; set; }

        public IPg10Node[] joininfo { get; set; }

        public bool? has_eclass_joins { get; set; }

        public Bitmapset[] top_parent_relids { get; set; }
    }
}