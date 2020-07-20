// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class cursor
    {
        public char name { get; set; }

        public char function { get; set; }

        public char command { get; set; }

        public char connection { get; set; }

        public bool opened { get; set; }

        public arguments argsinsert { get; set; }

        public arguments argsinsert_oos { get; set; }

        public arguments argsresult { get; set; }

        public arguments argsresult_oos { get; set; }

        public cursor next { get; set; }
    }
}
