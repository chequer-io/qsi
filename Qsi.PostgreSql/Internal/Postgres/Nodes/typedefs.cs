// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class typedefs
    {
        public string name { get; set; }

        public this_type type { get; set; }

        public ECPGstruct_member struct_member_list { get; set; }

        public int brace_level { get; set; }

        public typedefs next { get; set; }
    }
}
