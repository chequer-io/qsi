/* Generated by QSI

 Date: 2020-08-12
 Span: 3297:1 - 3302:14
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("ExecuteStmt")]
    internal class ExecuteStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_ExecuteStmt;

        public string name { get; set; }

        public IPg10Node[] @params { get; set; }
    }
}