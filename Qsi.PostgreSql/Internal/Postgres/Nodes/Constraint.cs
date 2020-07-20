// Generate from postgres/src/include/nodes/parsenodes.h

namespace Qsi.PostgreSql.Internal.Postgres.Nodes
{
    [PgNodeAttribute("Constraint")]
    internal class Constraint : Node
    {
        public ConstrType contype { get; set; }

        public string conname { get; set; }

        public bool deferrable { get; set; }

        public bool initdeferred { get; set; }

        public int location { get; set; }

        public bool is_no_inherit { get; set; }

        public Node raw_expr { get; set; }

        public string cooked_expr { get; set; }

        public char generated_when { get; set; }

        public IPgTree[] keys { get; set; }

        public IPgTree[] including { get; set; }

        public IPgTree[] exclusions { get; set; }

        public IPgTree[] options { get; set; }

        public string indexname { get; set; }

        public string indexspace { get; set; }

        public bool reset_default_tblspc { get; set; }

        public string access_method { get; set; }

        public Node where_clause { get; set; }

        public RangeVar pktable { get; set; }

        public IPgTree[] fk_attrs { get; set; }

        public IPgTree[] pk_attrs { get; set; }

        public char fk_matchtype { get; set; }

        public char fk_upd_action { get; set; }

        public char fk_del_action { get; set; }

        public IPgTree[] old_conpfeqop { get; set; }

        public string /* oid */ old_pktable_oid { get; set; }

        public bool skip_validation { get; set; }

        public bool initially_valid { get; set; }
    }
}
