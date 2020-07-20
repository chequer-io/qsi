// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateTrigStmt")]
    internal class CreateTrigStmt : Node
    {
        public char trigname { get; set; }

        public RangeVar relation { get; set; }

        public IPgTree[] funcname { get; set; }

        public IPgTree[] args { get; set; }

        public bool row { get; set; }

        public short timing { get; set; }

        public short events { get; set; }

        public IPgTree[] columns { get; set; }

        public Node whenClause { get; set; }

        public bool isconstraint { get; set; }

        public IPgTree[] transitionRels { get; set; }

        public bool deferrable { get; set; }

        public bool initdeferred { get; set; }

        public RangeVar constrrel { get; set; }
    }
}
