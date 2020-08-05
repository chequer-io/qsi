// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNode("CreatePLangStmt")]
    internal class CreatePLangStmt : IPgTree
    {
        public bool replace { get; set; }

        public string plname { get; set; }

        public IPgTree[] plhandler { get; set; }

        public IPgTree[] plinline { get; set; }

        public IPgTree[] plvalidator { get; set; }

        public bool pltrusted { get; set; }
    }
}
