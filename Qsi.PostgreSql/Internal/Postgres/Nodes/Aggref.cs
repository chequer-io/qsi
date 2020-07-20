// Generate from postgres/src/include/nodes/primnodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    internal class Aggref
    {
        public Expr xpr { get; set; }

        public int /* oid */ aggfnoid { get; set; }

        public int /* oid */ aggtype { get; set; }

        public int /* oid */ aggcollid { get; set; }

        public int /* oid */ inputcollid { get; set; }

        public int /* oid */ aggtranstype { get; set; }

        public IPgTree[] aggargtypes { get; set; }

        public IPgTree[] aggdirectargs { get; set; }

        public IPgTree[] args { get; set; }

        public IPgTree[] aggorder { get; set; }

        public IPgTree[] aggdistinct { get; set; }

        public Expr aggfilter { get; set; }

        public bool aggstar { get; set; }

        public bool aggvariadic { get; set; }

        public char aggkind { get; set; }

        public index agglevelsup { get; set; }

        public AggSplit aggsplit { get; set; }

        public int location { get; set; }
    }
}
