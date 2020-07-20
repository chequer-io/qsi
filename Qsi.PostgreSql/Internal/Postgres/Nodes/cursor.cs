// Generate from postgres/src/interfaces/ecpg/preproc/type.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class cursor
    {
        public string name { get; set; }

        public string function { get; set; }

        public string command { get; set; }

        public string connection { get; set; }

        public bool opened { get; set; }

        public arguments argsinsert { get; set; }

        public arguments argsinsert_oos { get; set; }

        public arguments argsresult { get; set; }

        public arguments argsresult_oos { get; set; }

        public cursor next { get; set; }
    }
}
