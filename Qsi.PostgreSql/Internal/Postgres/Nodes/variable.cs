// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class variable
    {
        public string name { get; set; }

        public ECPGtype type { get; set; }

        public int brace_level { get; set; }

        public variable next { get; set; }
    }
}
