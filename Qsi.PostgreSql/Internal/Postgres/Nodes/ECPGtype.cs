// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class ECPGtype
    {
        public ECPGttype type { get; set; }

        public string type_name { get; set; }

        public string size { get; set; }

        public string struct_sizeof { get; set; }

        public ECPGtype element { get; set; }

        public ECPGstruct_member members { get; set; }

        public int counter { get; set; }
    }
}
