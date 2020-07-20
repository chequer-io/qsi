// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("CreatePLangStmt")]
    internal class CreatePLangStmt : Node
    {
        public bool replace { get; set; }

        public char plname { get; set; }

        public IPgTree[] plhandler { get; set; }

        public IPgTree[] plinline { get; set; }

        public IPgTree[] plvalidator { get; set; }

        public bool pltrusted { get; set; }
    }
}
