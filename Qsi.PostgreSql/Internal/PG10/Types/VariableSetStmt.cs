/* Generated by QSI

 Date: 2020-08-12
 Span: 1970:1 - 1977:18
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("VariableSetStmt")]
    internal class VariableSetStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_VariableSetStmt;

        public VariableSetKind? kind { get; set; }

        public string name { get; set; }

        public IPg10Node[] args { get; set; }

        public bool? is_local { get; set; }
    }
}
