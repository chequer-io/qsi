// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class assignment
    {
        public string variable { get; set; }

        public ECPGdtype value { get; set; }

        public assignment next { get; set; }
    }
}
