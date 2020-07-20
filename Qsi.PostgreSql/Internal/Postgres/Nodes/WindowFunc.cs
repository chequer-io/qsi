// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class WindowFunc
    {
        public Expr xpr { get; set; }

        public string /* oid */ winfnoid { get; set; }

        public string /* oid */ wintype { get; set; }

        public string /* oid */ wincollid { get; set; }

        public string /* oid */ inputcollid { get; set; }

        public IPgTree[] args { get; set; }

        public Expr aggfilter { get; set; }

        public index winref { get; set; }

        public bool winstar { get; set; }

        public bool winagg { get; set; }

        public int location { get; set; }
    }
}
