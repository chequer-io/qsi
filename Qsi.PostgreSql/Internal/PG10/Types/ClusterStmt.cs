/* Generated by QSI

 Date: 2020-08-12
 Span: 3073:1 - 3079:14
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("ClusterStmt")]
    internal class ClusterStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_ClusterStmt;

        public RangeVar[] relation { get; set; }

        public string indexname { get; set; }

        public bool? verbose { get; set; }
    }
}