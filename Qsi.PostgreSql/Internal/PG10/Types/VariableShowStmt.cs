/* Generated by QSI

 Date: 2020-08-12
 Span: 1983:1 - 1987:19
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("VariableShowStmt")]
    internal class VariableShowStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_VariableShowStmt;

        public string name { get; set; }
    }
}
