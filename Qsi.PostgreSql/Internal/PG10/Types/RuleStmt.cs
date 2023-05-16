/* Generated by QSI

 Date: 2020-08-12
 Span: 2870:1 - 2880:11
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("RuleStmt")]
    internal class RuleStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_RuleStmt;

        public RangeVar[] relation { get; set; }

        public string rulename { get; set; }

        public IPg10Node[] whereClause { get; set; }

        public CmdType? @event { get; set; }

        public bool? instead { get; set; }

        public IPg10Node[] actions { get; set; }

        public bool? replace { get; set; }
    }
}