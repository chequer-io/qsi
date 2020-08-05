// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreateDomainStmt")]
    internal class CreateDomainStmt : IPgTree
    {
        public IPgTree[] domainname { get; set; }

        public TypeName typeName { get; set; }

        public CollateClause collClause { get; set; }

        public IPgTree[] constraints { get; set; }
    }
}
