// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateStmt")]
    internal class CreateStmt : Node
    {
        public RangeVar relation { get; set; }

        public IPgTree[] tableElts { get; set; }

        public IPgTree[] inhRelations { get; set; }

        public PartitionBoundSpec partbound { get; set; }

        public PartitionSpec partspec { get; set; }

        public TypeName ofTypename { get; set; }

        public IPgTree[] constraints { get; set; }

        public IPgTree[] options { get; set; }

        public OnCommitAction oncommit { get; set; }

        public string tablespacename { get; set; }

        public string accessMethod { get; set; }

        public bool if_not_exists { get; set; }
    }
}
