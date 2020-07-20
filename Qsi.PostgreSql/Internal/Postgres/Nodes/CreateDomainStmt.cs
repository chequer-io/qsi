// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreateDomainStmt")]
    internal class CreateDomainStmt : Node
    {
        public IPgTree[] domainname { get; set; }

        public TypeName typeName { get; set; }

        public CollateClause collClause { get; set; }

        public IPgTree[] constraints { get; set; }
    }
}
