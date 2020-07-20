// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class descriptor
    {
        public char name { get; set; }

        public char connection { get; set; }

        public descriptor next { get; set; }
    }
}
