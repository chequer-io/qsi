/* Generated by QSI

 Date: 2020-08-12
 Span: 71:9 - 81:2
 File: src/postgres/include/access/tupdesc.h

*/

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    internal class tupleDesc
    {
        public int? natts { get; set; }

        public FormData_pg_attribute[][] attrs { get; set; }

        public tupleConstr[] constr { get; set; }

        public uint? tdtypeid { get; set; }

        public int? tdtypmod { get; set; }

        public bool? tdhasoid { get; set; }

        public int? tdrefcount { get; set; }
    }
}