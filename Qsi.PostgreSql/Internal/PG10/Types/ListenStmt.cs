/* Generated by QSI

 Date: 2020-08-12
 Span: 2897:1 - 2901:13
 File: src/postgres/include/nodes/parsenodes.h

*/

using Qsi.PostgreSql.Internal.Serialization;

namespace Qsi.PostgreSql.Internal.PG10.Types
{
    [PgNode("ListenStmt")]
    internal class ListenStmt : IPg10Node
    {
        public virtual NodeTag Type => NodeTag.T_ListenStmt;

        public string conditionname { get; set; }
    }
}