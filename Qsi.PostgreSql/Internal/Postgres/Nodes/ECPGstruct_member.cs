// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class ECPGstruct_member
    {
        public char name { get; set; }

        public ECPGtype type { get; set; }

        public ECPGstruct_member next { get; set; }
    }
}
