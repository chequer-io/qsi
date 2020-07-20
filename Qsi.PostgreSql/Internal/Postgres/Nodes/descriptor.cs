// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class descriptor
    {
        public string name { get; set; }

        public string connection { get; set; }

        public descriptor next { get; set; }
    }
}
