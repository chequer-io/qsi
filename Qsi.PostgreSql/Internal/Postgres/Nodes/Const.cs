// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class Const
    {
        public Expr xpr { get; set; }

        public string /* oid */ consttype { get; set; }

        public int consttypmod { get; set; }

        public string /* oid */ constcollid { get; set; }

        public int constlen { get; set; }

        public object /* Datum */ constvalue { get; set; }

        public bool constisnull { get; set; }

        public bool constbyval { get; set; }

        public int location { get; set; }
    }
}
