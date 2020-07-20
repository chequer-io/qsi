// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class WindowFunc
    {
        public Expr xpr { get; set; }

        public int /* oid */ winfnoid { get; set; }

        public int /* oid */ wintype { get; set; }

        public int /* oid */ wincollid { get; set; }

        public int /* oid */ inputcollid { get; set; }

        public IPgTree[] args { get; set; }

        public Expr aggfilter { get; set; }

        public index winref { get; set; }

        public bool winstar { get; set; }

        public bool winagg { get; set; }

        public int location { get; set; }
    }
}
