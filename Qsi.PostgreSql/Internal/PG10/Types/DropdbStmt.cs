/* Generated by QSI

 Date: 2020-08-12
 Span: 3052:1 - 3057:13
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("DropdbStmt")]
    internal class DropdbStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_DropdbStmt;

        public string dbname { get; set; }

        public bool? missing_ok { get; set; }
    }
}
