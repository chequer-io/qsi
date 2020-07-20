// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("RangeTblEntry")]
    internal class RangeTblEntry : Node
    {
        public RTEKind rtekind { get; set; }

        public string /* oid */ relid { get; set; }

        public char relkind { get; set; }

        public int rellockmode { get; set; }

        public TableSampleClause tablesample { get; set; }

        public Query subquery { get; set; }

        public bool security_barrier { get; set; }

        public JoinType jointype { get; set; }

        public int joinmergedcols { get; set; }

        public IPgTree[] joinaliasvars { get; set; }

        public IPgTree[] joinleftcols { get; set; }

        public IPgTree[] joinrightcols { get; set; }

        public IPgTree[] functions { get; set; }

        public bool funcordinality { get; set; }

        public TableFunc tablefunc { get; set; }

        public IPgTree[] values_lists { get; set; }

        public string ctename { get; set; }

        public index ctelevelsup { get; set; }

        public bool self_reference { get; set; }

        public IPgTree[] coltypes { get; set; }

        public IPgTree[] coltypmods { get; set; }

        public IPgTree[] colcollations { get; set; }

        public string enrname { get; set; }

        public double enrtuples { get; set; }

        public Alias alias { get; set; }

        public Alias eref { get; set; }

        public bool lateral { get; set; }

        public bool inh { get; set; }

        public bool inFromCl { get; set; }

        public uint /* AclMode */ requiredPerms { get; set; }

        public string /* oid */ checkAsUser { get; set; }

        public Bitmapset selectedCols { get; set; }

        public Bitmapset insertedCols { get; set; }

        public Bitmapset updatedCols { get; set; }

        public Bitmapset extraUpdatedCols { get; set; }

        public IPgTree[] securityQuals { get; set; }
    }
}
