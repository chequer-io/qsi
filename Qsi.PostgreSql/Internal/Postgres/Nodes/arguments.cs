// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class arguments
    {
        public variable variable { get; set; }

        public variable indicator { get; set; }

        public arguments next { get; set; }
    }
}
