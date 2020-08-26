/* Generated by QSI

 Date: 2020-08-12
 Span: 95:1 - 95:12
 File: src/postgres/include/executor/executor.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("Path")]
    internal class Path : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_Path;

        public NodeTag pathtype { get; set; }

        public RelOptInfo[] parent { get; set; }

        public PathTarget[] pathtarget { get; set; }

        public ParamPathInfo[] param_info { get; set; }

        public bool? parallel_aware { get; set; }

        public bool? parallel_safe { get; set; }

        public int? parallel_workers { get; set; }

        public double? rows { get; set; }

        public double? startup_cost { get; set; }

        public double? total_cost { get; set; }

        public IPg10Node[] pathkeys { get; set; }
    }
}